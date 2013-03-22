using System;
using System.ComponentModel.Composition;
using System.Linq;
using Adventureworks.Domain;
using Adventureworks.Domain.Interfaces;

namespace Adventureworks.SQLRepository
{
    [Export(typeof(IShoppingCartRepository))]
    internal class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly AdventureWorks2008R2Entities _db = new AdventureWorks2008R2Entities();

        public void AddToCart(string shoppingCartID, int productID, int quantity)
        {
            ShoppingCartItem myItem =
                (from c in _db.ShoppingCartItems
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
                _db.ShoppingCartItems.AddObject(cartadd);
            }
            else
            {
                myItem.Quantity += quantity;
            }

            _db.SaveChanges();
        }

        public IQueryable<ShoppingCartItem> GetCartItemsByID(string shoppingCartID)
        {
            var cartItems = from cart in _db.ShoppingCartItems
                            where cart.ShoppingCartID == shoppingCartID
                            select cart;

            return cartItems;
        }



    }
}