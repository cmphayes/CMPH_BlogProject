using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMPH_BlogProject.Helper;
using CMPH_BlogProject.Models;

namespace CMPH_BlogProject.Controllers
{
    public class BlogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blogs
        public ActionResult Index()
        {
            return View(db.Blogs.ToList());
        }

        // GET: Blogs/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,Published")] Blog blog, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blog.Title);
                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blog);
                }
                if (db.Blogs.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blog);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blog.MediaURL = "/Uploads/" + fileName;
                }

                blog.Slug = slug;
                blog.Created = DateTimeOffset.Now;
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // GET: Blogs/Details/5
        public ActionResult Details(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.FirstOrDefault(p => p.Slug == slug);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.FirstOrDefault(p => p.Slug == slug);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,MediaURL,Published")] Blog blog, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                //The Title and slug must be unique so what happens if we edit the Title and the resulting slug is already present?

                var slug = StringUtilities.URLFriendly(blog.Title);

                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blog);
                }
                if (blog.Slug != slug && db.Blogs.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blog);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blog.MediaURL = "/Uploads/" + fileName;
                }


                blog.Slug = slug;
                blog.Updated = DateTimeOffset.Now;
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // GET: Blogs/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.FirstOrDefault(p => p.Slug == slug);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string slug)
        {
            Blog blog = db.Blogs.FirstOrDefault(p => p.Slug == slug);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Feature()
        {
            var random = new Random();
            var allBlogs = db.Blogs.ToList();
            return View(allBlogs[random.Next(0, allBlogs.Count())]);
        }
        public ActionResult Gallery()
        {
            return View(db.Blogs.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Unpublished()
        {
            return View(db.Blogs.ToList());
        }
    }
}

