using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Adventureworks.Domain;
using Adventureworks.Web.Models;

namespace Adventureworks.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {

        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrowseProducts()
        {
            var featuredProducts = new FeaturedProducts();
            IQueryable<Product> products = featuredProducts.Load();

            return PartialView(products);
        }

        public ActionResult CreateProduct()
        {
            Product product = new Product();

            return View(product);
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            product.ProductNumber = "FR-R12U-48";
            product.MakeFlag = true;
            product.FinishedGoodsFlag = true;
            product.SafetyStockLevel = 500;
            product.ReorderPoint = 375;
            product.StandardCost = 288;
            product.ListPrice = 388;
            product.DaysToManufacture = 1;
            product.SellStartDate = new DateTime(2005, 7, 1);
            product.rowguid = Guid.NewGuid();
            product.ModifiedDate = DateTime.Now;

            CreateProductInDatabase(product);

            return RedirectToAction("Index");
        }

        public void CreateProductInDatabase(Product product)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                db.Products.AddObject(product);
                db.SaveChanges();
            }
        }

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = 100;
            var totalPages = (int) Math.Ceiling(totalRecords/(float) pageSize);

            try
            {
                ParameterExpression param = Expression.Parameter(typeof (Product), "product");
                Func<Product, object> func = Expression.Lambda<Func<Product, object>>(
                    Expression.Convert(
                        Expression.Property(param, typeof (Product).GetProperty(sidx)), typeof (object)
                        ), param).Compile();


                IEnumerable<Product> products = null;
                switch (sord)
                {
                    case "asc":
                        products = GetTop100Products().OrderBy(func)
                            .Skip(pageIndex*pageSize).Take(pageSize).AsEnumerable();
                        break;
                    case "desc":
                        products = GetTop100Products().OrderByDescending(func)
                            .Skip(pageIndex*pageSize).Take(pageSize).AsEnumerable();
                        break;
                    default:
                        break;
                }


                var dataRows = (from product in products
                                select new
                                           {
                                               id = product.ProductID,
                                               product.Name,
                                               salable = product.FinishedGoodsFlag,
                                               size = product.Size
                                           }).ToArray();


                var jsonData = new
                                   {
                                       total = totalPages,
                                       page = pageIndex,
                                       records = totalRecords,
                                       rows = dataRows
                                   };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IQueryable<Product> GetTop100Products()
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                return db.Products.Top("100");
            }
        }
    }
}