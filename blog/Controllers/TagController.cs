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
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {

                // Get article from db
                var articles = db.Tags
                    .Include(t => t.Articles.Select(a => a.Tags))
                    .Include(t => t.Articles.Select(a => a.Author))
                    .FirstOrDefault(t => t.Id == id)
                    .Articles
                    .ToList();

                // Return the view
                return View(articles);
            }
        }
    }
}