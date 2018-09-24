using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMPH_BlogProject.Models;
using Microsoft.AspNet.Identity;

namespace CMPH_BlogProject.Controllers
{
    [RequireHttps]

    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comment.Include(c => c.Author).Include(c => c.Blog);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        //[Authorize(Roles = "Admin")]
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
        //    ViewBag.BlogId = new SelectList(db.Blogs, "Id", "Title");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogId,Body")] Comment comment, string CommentBody, string Slug)
        {
            if (ModelState.IsValid)
            {
                comment.AuthorId = User.Identity.GetUserId();
                comment.Body = CommentBody;
                comment.Created = DateTimeOffset.Now;
                db.Comment.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Blogs", new { Slug });
            }
            //comment.Created = DateTimeOffset.Now;
            //comment.AuthorId = User.Identity.GetUserId();
            //comment.Body = CommentBody;
            //db.SaveChanges();
            return RedirectToAction("PublishedBlogs", "Blogs");
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.BlogId = new SelectList(db.Blog, "Id", "Title", comment.BlogId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BlogId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                comment.Updated = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("PublishedBlogs", "Blogs");
            }
            comment.Updated = DateTimeOffset.Now;
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.BlogId = new SelectList(db.Blog, "Id", "Title", comment.BlogId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id, string slug)
        {
            ViewBag.Slug = slug;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comment.Find(id);
            db.Comment.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("PublishedBlogs", "Blogs");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
