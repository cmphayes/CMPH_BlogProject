using CMPH_BlogProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMPH_BlogProject.Helper
{
    public class BlogHelper
    {
        private UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetBlogImagePath(int blogId)
        {
        //    var defaultPath = "/Uploads/default.jpg";
        //    if (string.IsNullOrEmpty(blogId))
        //        return defaultPath;

            var blogImagePath = db.Blog.Find(blogId).MediaURL;
            if (string.IsNullOrEmpty(blogImagePath))
                return blogImagePath;

            return blogImagePath;
        }
    }
}