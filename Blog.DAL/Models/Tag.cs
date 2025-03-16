using System.Collections.Generic;

namespace BlogSystem.DAL.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
    }
}