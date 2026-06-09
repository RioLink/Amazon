namespace Domain.Entities
{
    public class Address
    {
        public int    Id         { get; set; }
        public string UserId     { get; set; } = string.Empty;
        public User   User       { get; set; } = null!;

        public string FullName   { get; set; } = string.Empty;
        public string Phone      { get; set; } = string.Empty;
        public string City       { get; set; } = string.Empty;
        public string Street     { get; set; } = string.Empty;
        public string Building   { get; set; } = string.Empty;
        public string? Apartment { get; set; }
        public string? PostalCode{ get; set; }
        public bool   IsDefault  { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
