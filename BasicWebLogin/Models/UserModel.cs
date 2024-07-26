using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebLogin.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Token { get; set; }

        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Pwd { get; set; }

        public bool ResetPassword { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
    }
}
