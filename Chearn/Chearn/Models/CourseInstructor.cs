namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CourseInstructor
    {
        public int ID { get; set; }

        public int? InstructorID { get; set; }

        public int? CourseID { get; set; }
    }
}
