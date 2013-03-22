using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventureworks.Domain.Interfaces
{
    public interface IShoppingCartRepository
    {
        void AddToCart(string shoppingCartID, int productID, int quantity);
        IQueryable<ShoppingCartItem> GetCartItemsByID(string shoppingCartID);
    }
}
