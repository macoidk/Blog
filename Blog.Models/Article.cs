using System;
using System.Collections.Generic;

namespace BlogSystem.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        public ICollection<Comment> Comments { get; set; }
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}