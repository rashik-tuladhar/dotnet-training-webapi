using System;

namespace DlmsWebApi.Repository.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => !IsExpired && !IsUsed && !IsRevoked;
    }
}
