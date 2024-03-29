namespace Chearn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserRole
    {
        public int ID { get; set; }

        public int? UserID { get; set; }

        public int? RoleID { get; set; }

        public virtual CUser CUser { get; set; }

        public virtual Role Role { get; set; }
    }
}
