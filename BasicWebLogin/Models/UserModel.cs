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

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Is email confirmed?")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Password")]
        public string Pwd { get; set; }

        public bool ResetPassword { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
    }
}
