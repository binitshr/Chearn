using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.ViewModels
{
    public class courseIndexData
    {
        public IEnumerable<Cours> Courses { get; set; }
        
        public IEnumerable<Lesson> Lessons { get; set; }
    }
}