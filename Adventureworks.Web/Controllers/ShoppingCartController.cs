using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adventureworks.Domain;
using Adventureworks.Domain.Interfaces;

namespace Adventureworks.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        [ImportingConstructor]
        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            this._shoppingCartRepository = shoppingCartRepository;
            this._productRepository = productRepository;
        }

        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            var cartItems =
                this._shoppingCartRepository.GetCartItemsByID(this.HttpContext.User.Identity.Name).AsEnumerable();
            ViewBag.CartTotal = GetTotal(this.HttpContext.User.Identity.Name);

            return View(cartItems);
        }

        private decimal GetTotal(string shoppingCartID)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                decimal? total =
                (from cartItems in db.ShoppingCartItems
                 where cartItems.ShoppingCartID == shoppingCartID
                 select (int?)cartItems.Quantity * cartItems.Product.ListPrice)
                .Sum();

                return total ?? decimal.Zero;
            }
        }

        public ActionResult AddToCart(int id)
        {
            this._shoppingCartRepository.AddToCart(this.HttpContext.User.Identity.Name, id, 1);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult JsonAddToCart(int id)
        {
            this._shoppingCartRepository.AddToCart(this.HttpContext.User.Identity.Name, id, 1);

            Product product = _productRepository.GetProductById(id);

            return Json(new
            {
                Id = product.ProductID,
                product.Name,
                LargeUrl = VirtualPathUtility.ToAbsolute("~/Image/ProductThumbnail?productPhotoID=" + product.ProductProductPhotoes.FirstOrDefault<ProductProductPhoto>().ProductPhotoID)
            });
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Display the confirmation message
            var results = new {
                Message = Server.HtmlEncode(id.ToString()) +
                    " has been removed from your shopping cart.",
                CartTotal = GetTotal(this.HttpContext.User.Identity.Name),
                CartCount = GetCount(this.HttpContext.User.Identity.Name),
                ItemCount = RemoveFromCart(this.HttpContext.User.Identity.Name, id),
                DeleteId = id
            };

            return Json(results);
        }

        private int GetCount(string shoppingCartID)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                int? count = (from cartItems in db.ShoppingCartItems
                              where cartItems.ShoppingCartID == shoppingCartID
                              select (int?) cartItems.Quantity).Sum();

                return count ?? 0;
            }
        }

        private int RemoveFromCart(string shoppingCartId, int cartItemId)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                int itemCount = 0;
                //Get the cart
                var cartItem = db.ShoppingCartItems.Single(
                    cart => cart.ShoppingCartID == shoppingCartId
                            && cart.ShoppingCartItemID == cartItemId);

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        itemCount = cartItem.Quantity;
                    }
                    else
                    {
                        db.ShoppingCartItems.DeleteObject(cartItem);
                    }
                    db.SaveChanges();
                }

                return itemCount;
            }
        }

        public JsonResult GetCartItems()
        {
            var cartItems =
                this._shoppingCartRepository.GetCartItemsByID(this.HttpContext.User.Identity.Name).AsEnumerable();

            var dataRows = (cartItems.Select(cartItem => new {
                                                                 product = new {
                                                                                 Id = cartItem.ProductID,
                                                                                 cartItem.Product.Name, 
                                                                                 LargeUrl = VirtualPathUtility.ToAbsolute("~/Image/ProductThumbnail?productPhotoID=" + cartItem.Product.ProductProductPhotoes.FirstOrDefault<ProductProductPhoto>().ProductPhotoID) },
                                                                 date = cartItem.DateCreated,
                                                                 quantity = cartItem.Quantity
                                                             })).ToArray();

            return Json(dataRows, JsonRequestBehavior.AllowGet);
        }
    }
}
