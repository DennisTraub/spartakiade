using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adventureworks.Domain;

namespace Adventureworks.Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            var cartItems =
                GetCartItemsByID(this.HttpContext.User.Identity.Name).AsEnumerable();
            ViewBag.CartTotal = GetTotal(this.HttpContext.User.Identity.Name);

            return View(cartItems);
        }

        public IQueryable<ShoppingCartItem> GetCartItemsByID(string shoppingCartID)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                var cartItems = from cart in db.ShoppingCartItems
                                where cart.ShoppingCartID == shoppingCartID
                                select cart;

                return cartItems;
            }
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
            AddToCart(this.HttpContext.User.Identity.Name, id, 1);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult JsonAddToCart(int id)
        {
            AddToCart(this.HttpContext.User.Identity.Name, id, 1);

            Product product = GetProductById(id);

            return Json(new
            {
                Id = product.ProductID,
                product.Name,
                LargeUrl = VirtualPathUtility.ToAbsolute("~/Image/ProductThumbnail?productPhotoID=" + product.ProductProductPhotoes.FirstOrDefault<ProductProductPhoto>().ProductPhotoID)
            });
        }

        public void AddToCart(string shoppingCartID, int productID, int quantity)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                ShoppingCartItem myItem =
                    (from c in db.ShoppingCartItems
                     where c.ShoppingCartID == shoppingCartID && c.ProductID == productID
                     select c).FirstOrDefault();
                if (myItem == null)
                {
                    var cartadd = new ShoppingCartItem
                        {
                            ShoppingCartID = shoppingCartID,
                            Quantity = quantity,
                            ProductID = productID,
                            DateCreated = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        };
                    db.ShoppingCartItems.AddObject(cartadd);
                }
                else
                {
                    myItem.Quantity += quantity;
                }

                db.SaveChanges();
            }
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
                GetCartItemsByID(this.HttpContext.User.Identity.Name).AsEnumerable();

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

        public Product GetProductById(int productID)
        {
            using (var db = new AdventureWorks2008R2Entities())
            {
                Product product = db.Products.Where<Product>(p => p.ProductID == productID).FirstOrDefault<Product>();
                return product;
            }
        }
    }
}
