using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace blog.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string Author { get; set; }

        public bool IsLiked(string name, int id)
        {
            return this.Author.Equals(name) && this.ArticleId.Equals(id);
        }

    }
}