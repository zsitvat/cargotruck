namespace Cargotruck.Shared.Model.Dto
{
    public class CurrentUser
    {
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Id { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    }
}
