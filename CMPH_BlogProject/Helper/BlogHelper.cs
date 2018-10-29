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

        //Im going to change this to a switch statement
        public static string GetUploadIcon(int blogId)
        {
            var defaultIcon = "/img/if_url_65946.png";
            var blogAttachmentPath = db.Blog.Find(blogId).MediaURL;

            string fileExt = System.IO.Path.GetExtension(blogAttachmentPath);

            if (fileExt == ".jpeg")
            {
                return "/img/if_jpeg_65908.png";
            }
            else if (fileExt == ".pdf")
            {
                return "/img/if_pdf_65920.png";
            }
            else if (fileExt == ".doc")
            {
                return "/img/if_docx_win_65892.png";
            }
            else if (fileExt == ".png")
            {
                return "/img/if_png_65922.png";
            }
            else if (fileExt == ".rar")
            {
                return "/img/if_rar_65936.png";
            }
            else
            {
                return defaultIcon;
            }
        }



    }
}