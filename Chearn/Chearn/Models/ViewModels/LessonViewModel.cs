using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.ViewModels
{
    public class LessonViewModel
    {
        public Lesson lesson { get; set; }
        public List<Question> questions { get; set; }

    }
}