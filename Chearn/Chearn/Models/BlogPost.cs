using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chearn.Models
{
    public class BlogPost
    {
        public int ID { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string FormattedDate => PublishDate.ToString("MM/dd/yyyy");
        [Required]
        [StringLength(8000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        public string Title { get; set; }
    }
}