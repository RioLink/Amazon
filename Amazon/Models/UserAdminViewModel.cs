namespace Amazon.Models
{
    public class UserAdminViewModel
    {
        public string Id          { get; set; } = string.Empty;
        public string UserName    { get; set; } = string.Empty;
        public string Email       { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
        public List<string> Roles { get; set; } = new();
        public int AddressCount   { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        public bool IsLocked => LockoutEnd != null && LockoutEnd > DateTimeOffset.UtcNow;
        public bool IsAdmin  => Roles.Contains("Admin");
    }
}
