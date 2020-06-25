namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StudentLesson
    {
        public int ID { get; set; }

        public int? StudentID { get; set; }

        public int? LessonID { get; set; }

        public int NumberCorrect { get; set; }

        public int NumberOfTrials { get; set; }

        [Column(TypeName = "date")]
        public DateTime NextReview { get; set; }

        public int SRSLevel { get; set; }

        public virtual Lesson Lesson { get; set; }

        public virtual Student Student { get; set; }
    }
}
