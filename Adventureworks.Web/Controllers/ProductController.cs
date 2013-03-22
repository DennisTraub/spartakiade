using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adventureworks.Domain;

namespace Adventureworks.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly AdventureWorks2008R2Entities _db = new AdventureWorks2008R2Entities();


        public ActionResult Index(int subcategoryId)
        {
            IQueryable<Product> products = GetProductsByCategory(subcategoryId);

            ViewBag.TotalCount = products.Count();
            ViewBag.SubcategoryName = GetProductSubcategoryById(subcategoryId).Name;
            ViewBag.ProductSubcategoryId = subcategoryId;

            return View();
        }

        private ProductSubcategory GetProductSubcategoryById(int subcategoryId)
        {
                return _db.ProductSubcategories.Single<ProductSubcategory>(cat => cat.ProductSubcategoryID == subcategoryId);
        }

        public ActionResult ProductGrid(int subcategoryId, int? page)
        {
            int currentPage = page.GetValueOrDefault(1);
            IQueryable<Product> products = GetProductsByCategory(subcategoryId);

            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalCount = products.Count();
            ViewBag.ProductSubcategoryId = subcategoryId;

            return PartialView(products.Skip((currentPage - 1) * 3).Take(3));
        }

        //
        // GET: /Product/Details/id

        public ActionResult Details(int id)
        {
            Product product = GetProductById(id);

            return View(product);
        }

        public JsonResult JsonDetails(int id)
        {
            Product product = GetProductById(id);

            return Json(new
                        {
                            Id = product.ProductID,
                            product.Name,
                            LargeUrl = VirtualPathUtility.ToAbsolute("~/Image/ProductThumbnail?productPhotoID=" + product.ProductProductPhotoes.FirstOrDefault<ProductProductPhoto>().ProductPhotoID)
                        }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsProductName_Available(string name)
        {

            if (!ProductExists(name))
                return Json(true, JsonRequestBehavior.AllowGet);

            string suggestedUID = String.Format(CultureInfo.InvariantCulture,
                "{0} is not available.", name);

            for (int i = 1; i < 100; i++)
            {
                string altCandidate = name + i.ToString();
                if (!ProductExists(altCandidate))
                {
                    suggestedUID = String.Format(CultureInfo.InvariantCulture,
                   "{0} is not available. Try {1}.", name, altCandidate);
                    break;
                }
            }
            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
        }

        public Product GetProductById(int productID)
        {
            Product product = _db.Products.Where<Product>(p => p.ProductID == productID).FirstOrDefault<Product>();
            return product;
        }

        public IQueryable<Product> GetProductsByCategory(int productSubcategoryID)
        {
                return
                    _db.Products.Include("ProductProductPhotoes")
                      .OrderBy("it.ProductID")
                      .Where(p => p.ProductSubcategoryID == productSubcategoryID);
        }

        public bool ProductExists(string productName)
        {
                Product product = _db.Products.Where<Product>(p => p.Name == productName).FirstOrDefault<Product>();
                return product != null;
        }

    }
}
