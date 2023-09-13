using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string BannerImage { get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual List<SubCategory> SubCategories { get; set; }
        [NotMapped]
        public string oldbimg { get; set; }
        [NotMapped]
        public string oldcimg { get; set; }
    }

}
