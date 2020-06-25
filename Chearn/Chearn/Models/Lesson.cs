namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public partial class Lesson
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lesson()
        {
            Edges = new HashSet<Edge>();
            Edges1 = new HashSet<Edge>();
            Questions = new HashSet<Question>();
            StudentLessons = new HashSet<StudentLesson>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(512)]
        public string Title { get; set; }
        [Required]
        [StringLength(8000)]
        [DataType(DataType.MultilineText)]
        public string MaterialA { get; set; }

        [Required]
        [StringLength(8000)]
        [DataType(DataType.MultilineText)]
        public string MaterialB { get; set; }

        
        [StringLength(3000)]
        public string ImageLink { get; set; }

        
        [StringLength(512)]
        public string VideoLink { get; set; }
        public int? CourseID { get; set; }
        //public int XPosition { get; set; }
        //public int YPosition { get; set; }
        public virtual Cours Cours { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Edge> Edges { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Edge> Edges1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentLesson> StudentLessons { get; set; }
    }
}
