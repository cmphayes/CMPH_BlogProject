using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMPH_BlogProject.Models
{
    public class Comment
    {
        //pk
        public int Id { get; set; }
        //fk - points to a specific BlogId pk
        public int BlogId { get; set; }
        //fk - points to ASPNetUser table pk
        public string AuthorId { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        [AllowHtml]
        public string UpdateReason { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual Blog Blog { get; set; }

        public Comment()
        {

        }
    }
}