using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Testemonial
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Testimonial { get; set; }
        public DateTime Created { get; set; }
    }
}
