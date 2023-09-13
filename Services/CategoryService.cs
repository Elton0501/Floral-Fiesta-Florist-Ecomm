using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService
    {
        #region singleton
        public static CategoryService Instance
        {
            get
            {
                if (instance == null) instance = new CategoryService();
                return instance;
            }
        }
        private static CategoryService instance { get; set; }

        public CategoryService()
        {
        }
        #endregion

        public IEnumerable<Category> GetAllActiveCategory()
        {
            using(var context = new SVIContext())
            {
                return context.Categories.Where(x => x.Status == true).ToList();
            }
        }
        public IEnumerable<SubCategory> GetAllActiveSubCat(int? catId)
        {
            using(var context = new SVIContext())
            {
                return context.SubCategories.Where(x => x.CategoryId == catId).ToList();
            }
        }
        public string CategoryName(int? catId)
        {
            using (var context = new SVIContext())
            {
                return context.Categories.FirstOrDefault(x => x.Id == catId).Name;
            }
        }

        public Category Category(int? catId)
        {
            using (var context = new SVIContext())
            {
                return context.Categories.FirstOrDefault(x => x.Id == catId);
            }
        }
    }
}
