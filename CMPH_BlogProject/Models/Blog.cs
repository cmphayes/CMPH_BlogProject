using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMPH_BlogProject.Models
{
    public class Blog
    {
        //list out the class members/properties
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        [AllowHtml]
        public string Title { get; set; }
        [AllowHtml]
        public string Abstract { get; set; }
        public string Slug { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string MediaURL { get; set; }
        public bool Published { get; set; }

        public virtual ICollection<Comment> Comments {get; set;}

        public Blog()
        {
            Comments = new HashSet<Comment>();
        }
    }
}