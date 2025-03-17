using System;
using System.Collections.Generic;

namespace BlogSystem.BLL.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        public int UserId { get; set; }
        public string AuthorName { get; set; }
        
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        
        public int? ParentCommentId { get; set; }
        public List<CommentDto> ChildComments { get; set; } = new List<CommentDto>();
    }
}