using blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blog.Controllers
{
    public class LikeController : Controller
    {
        // Post: Like
        public ActionResult Post(string name, int id)
        {

            using (var db = new BlogDbContext())
            {
                bool chek = true;

                foreach (var like in db.Likes)
                {
                    if (like.ArticleId.Equals(id) && like.Author.Equals(name))
                    {
                        chek = false;
                    }
                }

                Like likes = new Like();

                var article = db.Articles
                  .Where(a => a.Id == id)
                   .First();

                if (chek)
                {

                    article.Likes++;

                    likes.ArticleId = id;
                    likes.Author = name;

                    db.Likes.Add(likes);
                }
                else
                {

                     likes = db.Likes.Where(a => a.Author.Equals(name) && a.ArticleId.Equals(id)).First();

                    article.Likes--;

                    db.Likes.Remove(likes);
                }
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", "Article", new { id = id});
            }

            }               
        }
    }