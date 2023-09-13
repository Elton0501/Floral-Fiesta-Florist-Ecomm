using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [AllowHtml]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        [Required]
        public bool Isfeatured { get; set; }
        public bool Seasonal { get; set; }
        public bool isWhatsapp { get; set; }
        public bool Status { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int SubCategoryId { get; set; }
        [Required]
        public string SeoTags { get; set; }
        [Required]
        public int? Rating { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual List<ProductImage> ProductImages { get; set; }
        public decimal Price{ get; set; }
        public decimal DiscountPrice { get; set; }
        [NotMapped]
        public string PImageName { get; set; }
        [NotMapped]
        public string OldPImageName { get; set; }
        public string imgdefault { get; set; }
        [NotMapped]
        public virtual Category Category { get; set; }
        [NotMapped]
        public string DefaultImage { get; set; }
        [NotMapped]
        public bool isAvailCart { get; set; }
        [NotMapped]
        public int? cartId { get; set; }
        [NotMapped]
        public int? BestSellerCount { get; set; }
    }
}
