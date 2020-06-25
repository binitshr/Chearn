using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.ViewModels
{
    public class IndividualStatisticsViewModel
    {
        public DateTime StartDate { get; set; }
        public string FormattedStartDate { get; set; }
        public double AverageReviewsPerDay { get; set; }
        public double AverageNewLessonsPerDay { get; set; }
        public DateTime ProjectedCompletionDate { get; set; }
        public string FormattedProjectedCompletionDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ID { get; set; }
        public string Email { get; set; }
    }
}