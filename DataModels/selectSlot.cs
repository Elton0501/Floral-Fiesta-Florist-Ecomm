using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class selectSlot
    {
        public string date { get; set; }
        public List<Key> slot { get; set; }
        public bool result { get; set; }
        public string PId { get; set; }
    }
}
