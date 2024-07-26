namespace BasicWebLogin.Models
{
    public class LogIn : UserModel
    {
        public string? ConfirmPassword { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
