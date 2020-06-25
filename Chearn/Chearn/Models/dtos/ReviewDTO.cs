using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.dtos
{
    public class ReviewDTO
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public int LessonID { get; set; }
        public bool IsCorrect { get; set; }

    }
}