using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models
{

    public class Review
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public string UserID { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsCorrect { get; set; }
        public int Level { get; set; }


    }
}