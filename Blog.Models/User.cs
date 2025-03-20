using System;
using System.Collections.Generic;

namespace BlogSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime RegistrationDate { get; set; }
        public UserRole Role { get; set; }
        
        public ICollection<Article> Articles { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
    
    public enum UserRole
    {
        Reader,
        Author,
        Admin
    }
}