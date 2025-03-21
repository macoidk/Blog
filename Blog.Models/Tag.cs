using System.Collections.Generic;

namespace BlogSystem.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
    }
}