using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart
{
    // A cart always belongs to exactly one person, so every cart handler starts
    // by resolving the logged-in person id.
    internal static class CartPerson
    {
        public static int RequireId(IAppCurrentUser appCurrentUser)
        {
            if (appCurrentUser.UserId is not int personId)
                throw new MarketBusinessRuleException("CART_NO_USER", "You must be logged in to use a cart.");

            return personId;
        }
    }
}
