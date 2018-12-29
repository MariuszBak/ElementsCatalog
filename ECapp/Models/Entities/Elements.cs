using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECapp.Models.Entities
{
    public class Element
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Container { get; set; }
        public string Package { get; set; }
        public string Desc { get; set; }
        public string Status { get; set; }
        public long Quantity { get; set; }
    }

    public class ElementShort
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Container { get; set; }
        public string Package { get; set; }
        public long Quantity { get; set; }
    }
}
