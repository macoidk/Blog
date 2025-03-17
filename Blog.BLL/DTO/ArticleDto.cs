using System;
using System.Collections.Generic;

namespace BlogSystem.BLL.DTO
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        public int UserId { get; set; }
        public string AuthorName { get; set; }
        
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
        public int CommentCount { get; set; }
    }
}