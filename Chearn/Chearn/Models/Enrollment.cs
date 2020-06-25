namespace Chearn.Models
{
    using Microsoft.Ajax.Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Enrollment
    {
        public int ID { get; set; }

        public int? StudentID { get; set; }

        public int? CourseID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public bool CanBeEnrolledBy(User user) {

            if (user.IsStudent)
                return true;
            if (user.IsAdmin)
                return true;


            return false;
        }

    }

    public class User
    { 
        public bool IsStudent { get; set; }
        public bool IsAdmin { get; set; }

    }
}
