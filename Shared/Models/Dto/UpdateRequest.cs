namespace Cargotruck.Shared.Models.Request
{
    public class UpdateRequest
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; } = "User";

        public string? Img { get; set; } = "/img/profile.png";
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

    }
}
