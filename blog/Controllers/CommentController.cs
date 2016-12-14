using blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace blog.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Comment/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: Comment/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(int? id, Comment comment)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            using (var context = new BlogDbContext())
            {
                var article = context.Articles.FirstOrDefault(a => a.Id == id);

                var userEmail = context.Users
                   .Where(u => u.UserName == this.User.Identity.Name)
                   .First()
                   .Email;

                if (userEmail == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (article == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                comment.Article = article;
                comment.ArticleId = article.Id;
                comment.UeserEmail = userEmail;

                article.Comments.Add(comment);

                context.SaveChanges();

                return RedirectToAction("Details", "Article", new { id = id });
            }
        }

        //
        // POST: Comment/Delete
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {
       
                var comment = database.Comments
                    .Where(c => c.Id == id)
                    .FirstOrDefault();

                database.Comments.Remove(comment);

                database.SaveChanges();

                return RedirectToAction("Details", "Article", new { id = comment.ArticleId });
            }  
        }
    }
}