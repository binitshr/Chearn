using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.ViewModels
{
    public class EditStudentViewModel
    {
        public int ID { get; set; }
        public int CUserID { get; set; }
        public int YearsOfEducation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
    }
}