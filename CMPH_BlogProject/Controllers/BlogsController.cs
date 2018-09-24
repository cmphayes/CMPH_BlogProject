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
using PagedList;
using PagedList.Mvc;

namespace CMPH_BlogProject.Controllers
{
    [RequireHttps]

    public class BlogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blogs
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr; var blogList = IndexSearch(searchStr);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public IQueryable<Blog> IndexSearch(string searchStr)
        {
            var result = db.Blog.Where(b => b.Published).AsQueryable();
            if (searchStr != null)
            {
                result = result.Where(p => p.Title.Contains(searchStr) 
                || p.Body.Contains(searchStr) 
                || p.Comments.Any(c => c.Body.Contains(searchStr) 
                || c.Author.FirstName.Contains(searchStr) 
                || c.Author.LastName.Contains(searchStr) 
                || c.Author.DisplayName.Contains(searchStr) 
                || c.Author.Email.Contains(searchStr)));
            }
            else
            {
            result = db.Blog.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }

        // GET: Blogs
        public ActionResult PublishedBlogs(int? page, string searchStr)
        {
            ViewBag.Search = searchStr; var blogList = PublishedBlogsSearch(searchStr);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize));
        }


        public IQueryable<Blog> PublishedBlogsSearch(string searchStr)
        {
            var result = db.Blog.Where(b => b.Published).AsQueryable();
            if (searchStr != null)
            {
                result = result.Where(p => p.Title.Contains(searchStr)
                || p.Body.Contains(searchStr)
                || p.Abstract.Contains(searchStr)
                || p.Comments.Any(c => c.Body.Contains(searchStr)
                || c.Author.FirstName.Contains(searchStr)
                || c.Author.LastName.Contains(searchStr)
                || c.Author.DisplayName.Contains(searchStr)
                || c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.Blog.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UnpublishedBlogs(int? page, string searchStr)
        {
            ViewBag.Search = searchStr; var blogList = UnpublishedBlogsSearch(searchStr);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public IQueryable<Blog> UnpublishedBlogsSearch(string searchStr)
        {
            var result = db.Blog.Where(b => b.Published == false).AsQueryable();
            if (searchStr != null)
            {
                result = result.Where(p => p.Title.Contains(searchStr)
                || p.Body.Contains(searchStr)
                || p.Comments.Any(c => c.Body.Contains(searchStr)
                || c.Author.FirstName.Contains(searchStr)
                || c.Author.LastName.Contains(searchStr)
                || c.Author.DisplayName.Contains(searchStr)
                || c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.Blog.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }


        // GET: Blogs/Create
        [Authorize(Roles = "Admin,Mod")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mod")]
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
                if (db.Blog.Any(p => p.Slug == slug))
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
                db.Blog.Add(blog);
                db.SaveChanges();
                return RedirectToAction("PublishedBlogs", "Blogs");
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
            Blog blog = db.Blog.FirstOrDefault(p => p.Slug == slug);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        [Authorize(Roles = "Admin,Mod")]
        public ActionResult Edit(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blog.FirstOrDefault(p => p.Slug == slug);
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
        [Authorize(Roles = "Admin,Mod")]
        public ActionResult Edit([Bind(Include = "Id,Title,Created,Body,Slug,MediaURL,Published")] Blog blog, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                //The Title and slug must be unique so what happens if we edit the Title and the resulting slug is already present?

                var slug = StringUtilities.URLFriendly(blog.Title);
                if (slug != blog.Slug)
                {
                    if (String.IsNullOrWhiteSpace(slug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blog);
                    }
                    if (db.Blog.Any(p => p.Slug == slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blog);
                    }
                }
                

                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blog.MediaURL = "/Uploads/" + fileName;
                }
                blog.Slug = slug;
                blog.Updated = DateTimeOffset.Now;
                db.Blog.Add(blog);
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PublishedBlogs");
            }

            return View(blog);
        }

        // GET: Blogs/Delete/5
        [Authorize(Roles = "Admin,Mod")]
        public ActionResult Delete(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blog.FirstOrDefault(p => p.Slug == slug);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [Authorize(Roles = "Admin,Mod")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string slug)
        {
            Blog blog = db.Blog.FirstOrDefault(p => p.Slug == slug);
            db.Blog.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("PublishedBlogs");
        }

        public ActionResult feature()
        {
            //var random = new Random();
            //var allblogs = db.Blogs.ToList(); 
           // allblogs[random.Next(0, allblogs.Count())]
            return View();
        }

        public ActionResult Gallery()
        {
            return View(db.Blog.ToList());
        }

        //public ActionResult ViewComment(int? id)
        //{
        //    if (id != null)
        //    {
        //        return RedirectToAction("Details", "Comments", id);
        //    }
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    return View();
        //}

        //public ActionResult Comments(int? id)
        //{
        //    var comments = db.Comment.Where(x => x.BlogID == id).ToArray();
        //    return Json(comments, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Comment(Comment data)
        //{
        //    db.Comment.Add(data);
        //    db.SaveChanges();
        //    var options = new PusherOptions();
        //    options.Cluster = "XXX_APP_CLUSTER";
        //    var pusher = new Pusher("XXX_APP_ID", "XXX_APP_KEY", "XXX_APP_SECRET", options);
        //    ITriggerResult result = await pusher.TriggerAsync("asp_channel", "asp_event", data);
        //    return Content("ok");
        //}

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

