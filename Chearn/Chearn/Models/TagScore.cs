namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TagScore
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(24)]
        public string Tag0 { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(24)]
        public string Tag1 { get; set; }

        public int? Score { get; set; }
    }
}
