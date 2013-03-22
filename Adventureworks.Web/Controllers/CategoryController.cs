using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adventureworks.Domain;

namespace Adventureworks.Web.Controllers
{
    public class CategoryController : Controller
    {

        [OutputCache(Duration = 10)]
        [ChildActionOnly]
        public ActionResult Index(int? subcategoryId)
        {
            ProductSubcategory prodSubcat = GetProductSubcategoryById(subcategoryId.GetValueOrDefault(1));
            var productCategories = GetProductCategories();

            ViewBag.CurrentProductCategoryId = prodSubcat.ProductCategoryID;
            ViewBag.CurrentProductSubcategoryId = prodSubcat.ProductSubcategoryID;

            return PartialView(productCategories);
        }

        private ProductSubcategory GetProductSubcategoryById(int subcategoryId)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                return db.ProductSubcategories.Single<ProductSubcategory>(cat => cat.ProductSubcategoryID == subcategoryId);
            }
            
        }

        private IList<ProductCategory> GetProductCategories()
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                return db.ProductCategories.Include("ProductSubcategories").ToList();
            }
        }

        // GET: /Category/Browse/5
        public ActionResult Browse(string id)
        {
            return View();
        }
    }
}
