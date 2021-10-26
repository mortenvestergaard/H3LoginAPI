using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CRLogAPI.Models;

namespace CRLogAPI
{
    // This class represents DallManager!
    public class DalManager
    {
        //Generates a salt value for password hashing
        public string GenerateSalt()
        {
            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                byte[] buff = new byte[8];
                randomGenerator.GetBytes(buff);

                return Convert.ToBase64String(buff);
            }
        }

        /// <summary>
        /// Checks if the inputted username exists in the database
        /// </summary>
        /// <param 'name="username">' The inputted username</param>
        /// <param 'name="context">' The DBContext for finding the username in the database</param>
        /// <returns></returns>
        public bool CheckUsername(string username, LoginDBContext context)
        {
            var user = context.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (user != null)
            {
                string name = user.Username;
                if (username == name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a hash value using the inputted password and salt from the user using SHA256
        /// </summary>
        /// <param 'name="passwrd">' The inputted password value from the user</param>
        /// <param 'name="salt">' A generated salt value for hash generation</param>
        /// <returns></returns>
        public string CreateHashedPassword(string passwrd, string salt)
        {
            byte[] pwdWithSalt = Encoding.ASCII.GetBytes(string.Concat(passwrd, salt));
            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(pwdWithSalt));
            }
        }

        /// <summary>
        /// Gets the salt from the database fomr the given username
        /// </summary>
        /// <param 'name="username">' The user username to find the salt value</param>
        /// <param 'name="context">' The DBContext used to reach the database</param>
        /// <returns></returns>
        public string GetSaltFromDB(string username, LoginDBContext context)
        {
            var user = context.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            return user.Salt;
        }

        /// <summary>
        /// Get the hashed password from the database
        /// </summary>
        /// <param 'name="username">' The username inputted by the user to find the hashed password value</param>
        /// <param 'name="context">' The DBContext that is used to make database changes</param>
        /// <returns></returns>
        public string GetHashedPasswordFromDB(string username, LoginDBContext context)
        {
            var user = context.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            return user.HashedPassword;
        }

        /// <summary>
        /// Validates a password in the database that is tied to the username inputted by the user with the password the user also inputted
        /// </summary>
        /// <param 'name="password">' The user password from the website</param>
        /// <param 'name="username">' The users username from the website</param>
        /// <param 'name="salt">' The generated salt for password hashing</param>
        /// <param 'name="HashedPassword">' The hashed password to validate the hashed password in the database from the given username</param>
        /// <param 'name="context">' The DBContext that is used to make changes in the database, using EntityFramework</param>
        /// <returns></returns>
        public bool ValidatePassword(string password, string username, string salt, string HashedPassword)
        {
            string tempPwd = "";
            byte[] pwdWithSaltFromDB = Encoding.ASCII.GetBytes(string.Concat(password, salt));
            using (var sha256 = SHA256.Create())
            {
                tempPwd = Convert.ToBase64String(sha256.ComputeHash(pwdWithSaltFromDB));
            }
            if (tempPwd == HashedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LogDbLogin(string username, string userChoice, string token, string statusMessage, LoginDBContext context)
        {
            if (!String.IsNullOrEmpty(username))
            {
                UserLog log = new UserLog();

                // Adding data to database!
                log.Username = username;
                log.UserChoice = userChoice;
                log.Token = token;
                log.StatusMessage = statusMessage;
                log.LogTime = DateTime.Now.ToString("HH:mm:ss");
                context.UserLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}
