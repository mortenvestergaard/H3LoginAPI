using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRLogAPI;
using CRLogAPI.Models;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace CRLogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly LoginDBContext data;
        private DalManager manager = new DalManager();

        public LoginController(LoginDBContext context)
        {
            data = context;
        }

        //HTTP post for user registration!
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] User user)
        {
            //If the user object from the parameter is equal to null!
            if (user != null && !String.IsNullOrEmpty(user.Username) && !String.IsNullOrEmpty(user.Password))
            {
                if (manager.CheckUsername(user.Username, data))
                {
                    manager.LogDbLogin(user.Username, "Register", "No token", "Register denied", data);
                    return Conflict("User already exists");
                }
                //New instance of the user class that will be mapped to the database using Entity Framework CodeFirst approach!
                DbUser dbUser = new DbUser();

                //Sets the values of the dbUser!
                dbUser.Username = user.Username;
                dbUser.Salt = manager.GenerateSalt();
                dbUser.HashedPassword = manager.CreateHashedPassword(user.Password, dbUser.Salt);

                manager.LogDbLogin(user.Username, "Register", "No token", "Register successfull", data);

                //Adds the dbUser instance for database insertion, and saves changes made afterwards!
                data.Users.Add(dbUser);
                data.SaveChanges();
                return Ok();
            }

            //If something went wrong, a 400 Bad Request is sent to the user!
            else
            {
                manager.LogDbLogin(user.Username, "Register", "Bad request", "Register invalid", data);
                return BadRequest();
            }
        }

        //HTTP get for user login and verification!
        [HttpGet, Route("Login")]
        public IActionResult Login(string username, string password)
            {

            //If the user input is null or empty, a 401 Unauthorized is returned, specifying the login failed!
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                manager.LogDbLogin(username, "Login", "No token", "Login Denied", data);
                return Unauthorized("Login denied");
            }

            //Checks if the username exists in the database!
            if (manager.CheckUsername(username, data))
            {

                //Generates a salt to hash the password!
                string salt = manager.GetSaltFromDB(username, data);

                //Gets hashed password from database for validation!
                string dbHashedPass = manager.GetHashedPasswordFromDB(username, data);

                //Checks if the hash value of the inputtet password by the user is equal
                //to the password that is associated with the inputted username!
                if (manager.ValidatePassword(password, username, salt, dbHashedPass) == true)
                {
                    //If it is, then a new Json Web Token is created with a secret key, credentials and some token options!
                    // Using Add.Minutes() to set an expire time for how long a user can be loged on!
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey@345"));
                    var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokenOptions = new JwtSecurityToken(issuer: "http://localhost:38338", audience: "http://localhost:38338", claims: new List<Claim>(), expires: DateTime.Now.AddMinutes(30), signingCredentials: signingCredentials);

                    //The token is written as a string, and is then sent to the user!
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                    manager.LogDbLogin(username, "Login", tokenString, "Login successfull", data);

                    return Ok(new { Token = tokenString });
                }
            }
            manager.LogDbLogin(username, "Login", "No token", "Login Invalid", data);

            //If something goes wrong, a 400 Bad Request is sent to the user with the "Invalied request" text¨!
            return BadRequest("Invalid request");
        }
    }
}

