namespace Ventas.Application.Entities.Users.Login
{
    public class LoginServiceOutput
    {
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public string UserName { get; set; } = "";
    }
}
