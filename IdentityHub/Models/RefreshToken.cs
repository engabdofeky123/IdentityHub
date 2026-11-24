namespace IdentityHub.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime RevokedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
