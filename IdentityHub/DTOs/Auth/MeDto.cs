namespace IdentityHub.DTOs.Auth
{
    public class MeDto
    {
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool isEmailConfirmed { get; set; } = false;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
