using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chearn.Models.dtos
{
    public class EdgeDTO
    {
        public string Parent { get; set; }
        public string Child { get; set; }

        public Edge ToEdge() => new Edge() { ChildID =  int.Parse(Child.Substring("lesson".Length)), 
            ParentID = int.Parse(Parent.Substring("lesson".Length)) };
    }
}