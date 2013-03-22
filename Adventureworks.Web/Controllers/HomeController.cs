using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Adventureworks.Web.Models;

namespace Adventureworks.Web.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to AdventureWorks cycle store";

            var featuredProducts = new FeaturedProducts();
            var products = featuredProducts.Load();

            return View(products);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
