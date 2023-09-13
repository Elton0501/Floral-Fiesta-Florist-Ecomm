using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public List<Testemonial> Tests { get; set; }
    }
}
