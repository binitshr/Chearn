namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Categories")]
    public partial class Category
    {
        public int ID { get; set; }

        [Required]
        [StringLength(32)]
        public string Name { get; set; }
    }
}
