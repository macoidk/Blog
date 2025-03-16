using System;
using System.Collections.Generic;

namespace BlogSystem.DAL.Entities
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
        
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
    
    public enum UserRole
    {
        Reader,
        Author,
        Admin
    }
}