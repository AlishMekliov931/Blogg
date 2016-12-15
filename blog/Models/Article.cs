using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace blog.Models
{
    public class Article
    {
        private ICollection<Comment> comments;

        private ICollection<Tags> tags;

        public Article()
        {
            this.DateAdded = DateTime.Now;
            this.tags = new HashSet<Tags>();
            this.comments = new HashSet<Comment>();
        }

       

        public Article(string authorId, string title, string content, int categoryId)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Content = content;
            this.CategoryId = categoryId;
            this.DateAdded = DateTime.Now;
            this.tags = new HashSet<Tags>();
            this.comments = new HashSet<Comment>();
            this.Likes = 0;

        }


        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

        public int Likes { get; set; }
    //
    //   public ICollection<string> UserEmail
    //   {
    //       get { return this.comments; }
    //       set { this.comments = value; }
    //   }

        [ForeignKey("Author")]
        public string AuthorId { get; set;}

        public virtual ApplicationUser Author { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Comment> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }

        public virtual ICollection<Tags> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }

        public bool IsAuthorOfComment(string name)
        {
            return this.Author.UserName.Equals(name);
        }

        public bool IsLiked(string name, int id)
        {
            using ( var db = new BlogDbContext())
            {
                bool chek = false;

                    foreach (var like in db.Likes)
                    {
                        if (like.ArticleId.Equals(id) && like.Author.Equals(name))
                        {
                            chek = true;
                        }

                }
                return chek;
            }
        }
    }
}