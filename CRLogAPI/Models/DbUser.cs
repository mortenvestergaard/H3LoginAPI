using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CRLogAPI.Models
{
    // This class represents DbUser!
    public class DbUser
    {
        // The Key attribute is the PrimaryKey!
        [Key]
        [Column(TypeName = "nvarchar(255)")]
        public string Username { get; set; }

        // Using the Required attribute to require that TempComment is not null!
        // Using the Column attritute to Specific a particular Typename  - in this case to be nvarchar(255)!
        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public string HashedPassword { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Salt { get; set; }
    }
}
