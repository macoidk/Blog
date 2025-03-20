using System;
using System.Collections.Generic;

namespace BlogSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        
        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public ICollection<Comment> ChildComments { get; set; }
    }
}