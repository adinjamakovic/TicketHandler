using Market.Domain.Entities.Identity;


namespace Market.Domain.Entities.Sales
{
    /// <summary>
    /// Used for cart system
    /// </summary>
    public class CartEntity
    {
        /// <summary>
        /// Identifier of the person whom the cart belongs to
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// The person whom the cart belongs to
        /// </summary>
        public PersonEntity Person { get; set; }
        /// <summary>
        /// Identifier for a cart item
        /// </summary>
        public int CartItemId { get; set; }
        /// <summary>
        /// A cart item
        /// </summary>
        public CartItemEntity CartItem { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime LastUpdatedAtUtc { get; set; }
    }
}
