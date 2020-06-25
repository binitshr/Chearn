namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Edge
    {
        public int ID { get; set; }

        public int? ChildID { get; set; }

        public int? ParentID { get; set; }

        public virtual Lesson Lesson { get; set; }

        public virtual Lesson Lesson1 { get; set; }
    }
}
