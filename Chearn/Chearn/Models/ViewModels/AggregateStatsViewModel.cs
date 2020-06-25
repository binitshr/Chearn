using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.ViewModels
{
    public class AggregateStatsViewModel
    {
        public int ID { get; set; }
        public string CourseName { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        /// <summary>
        /// Number of back days to calculate SMA and EMA
        /// </summary>
        public int ReviewDays { get; set; }
        public double SMAReviewsPerDay { get; set; }
        public double EMAReviewsPerDay { get; set; }
        public double CorrectRate { get; set; }
        public List<Question> ChallengeQuestion { get; set; }


    }
}