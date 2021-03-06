﻿using blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace blog.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Article/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                // Get Article from database
                var articles = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .ToList();

                return View(articles);
            }
        }


   // public ActionResult Like(string email, int? id)
   // {
   //     using (var db = new BlogDbContext())
   //     {
   //
   //         var article = db.Articles
   //         .Where(a => a.Id == id)
   //          .First();
   //
   //         if (!article.UserEmail.Contains(email))
   //         {
   //             article.UserEmail.Add(email);
   //             article.Likes++;
   //         }
   //         else
   //         {
   //             article.UserEmail.Remove(email);
   //             article.Likes--;
   //         }
   //         db.Entry(article).State = EntityState.Modified;
   //         db.SaveChanges();
   //
   //         return RedirectToAction("Details", "Article", new { id = id });
   //     }
   // }


        public ActionResult Search(string ask)
        {
            using (var db = new BlogDbContext())
            {
                var articles = db.Articles
                  .Include(a => a.Author)
                  .Include(a => a.Tags)
                  .ToList();

                if (!String.IsNullOrEmpty(ask))
                {
                    articles = articles.Where(s => s.Title.ToLower().Contains(ask.ToLower())).ToList();
                }


                return View(articles);
                
            }
        }

        //
        // GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id ==  null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get tha article from database

                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(a => a.Comments)
                    .First();

                if (article == null)
                {
                    return HttpNotFound();
                }
                return View(article);

               
            }
        }

        //
        // GET Article/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var db = new BlogDbContext())
            {

                var model = new ArticleViewModel();
                model.Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .ToList();
               

                return View(model);

            }
        }

        //
        // POST: Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(ArticleViewModel model)
        {
                using (var database = new BlogDbContext())
                {
                if (ModelState.IsValid && model.Tags != null)
                {

                    // Get author id
                    var authorId = database.Users
                            .Where(u => u.UserName == this.User.Identity.Name)
                            .First()
                            .Id;

                    // Set articles author

                    var article = new Article(authorId, model.Title, model.Content, model.CategoryId);

                    this.SetArticleTags(article, model, database);

                    // Save article in DB
                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }

                     model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        //
        // GEt: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from database
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Category)
                    .First();

                if (! IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                ViewBag.TagsString = string.Join(",", article.Tags.Select(t => t.Name));

                // Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                // Pass article in view
                return View(article);
            }
        }

        //
        // Post: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from db
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                // Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                // Remove article from db
                database.Articles.Remove(article);
                database.SaveChanges();

                // Redirect to index page
                return RedirectToAction("Index");
            }
        }

        //
        // Get: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from db
                var article = database.Articles
                    .Where(a => a.Id == id) 
                    .First();

                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                // Create the view model
                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;
                model.CategoryId = article.CategoryId;
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                model.Tags = string.Join(", ", article.Tags.Select(t => t.Name));

                // Pass the view model to view
                return View(model);
            }
        }

        //
        // Post: Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            // Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    // Get article from db
                    var article = database.Articles
                        .FirstOrDefault(a => a.Id == model.Id);

                    // Set article properties
                    article.Title = model.Title;
                    article.Content = model.Content;
                    article.CategoryId = model.CategoryId;
                    this.SetArticleTags(article, model, database);

                    // Save article state in db
                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();

                    // Redirect to the index page
                    return RedirectToAction("Index");
                }
            }

            // If model state is valid, return the same  view
            return View(model);
        }

        private void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext db)
        {
            //Split tags
            var tagsStrings = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            //Clear current article tags
            article.Tags.Clear();

            //Set new tags
            foreach (var tagString in tagsStrings)
            {

                // Get tag from db by its name
                Tags tag = db.Tags.FirstOrDefault(t => t.Name.Equals(tagString));

                // If the tag is null, create new tag
                if (tag == null)
                {
                    tag = new Tags() { Name = tagString };
                    db.Tags.Add(tag);
                }

                // Add tag to article tags
                article.Tags.Add(tag);

            }
        }

        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

    }
}