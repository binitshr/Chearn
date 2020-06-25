namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Question
    {
        public int ID { get; set; }

        public int? Points { get; set; }
        public int? LessonID { get; set; }

        [Required]
        [StringLength(4000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

       
        [Required]
        [StringLength(256)]
        public string Option1 { get; set; }
        [Required]
        [StringLength(256)]
        public string Option2 { get; set; }
        
        [StringLength(256)]
        public string Option3 { get; set; }

       
        [StringLength(256)]
        public string Option4 { get; set; }

        [Required]
        [StringLength(256)]
        public string Answer { get; set; }


        public virtual Lesson Lesson { get; set; }
    }
}