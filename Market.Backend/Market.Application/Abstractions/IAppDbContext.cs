using Market.Domain.Entities.CustomerRelationship;
using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Sales;

namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    DbSet<AdminNewsEntity> AdminNews { get; }
    DbSet<LoyaltyProgrammeEntity> LoyaltyProgrammes { get; }
    DbSet<ReviewEntity> Reviews { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }
    DbSet<EventNewsEntity> EventNews { get; }
    DbSet<EventTypeEntity> EventTypes { get; }
    DbSet<GenreEntity> Genres { get; }
    DbSet<OrganizerEntity> Organizers { get; }
    DbSet<PerformerEntity> Performers { get; }
    DbSet<PerformerEventEntity> PerformerEvents { get; }
    DbSet<CityEntity> Cities { get; }
    DbSet<CountryEntity> Countries { get; }
    DbSet<LocationEntity> Locations { get; }
    DbSet<PointOfSaleEntity> PointsOfSale { get; }
    DbSet<VenueEntity> Venues { get; }
    DbSet<PersonEntity> Persons { get; }
    DbSet<WalletEntity> Wallets { get; }
    DbSet<CartItemEntity> CartItems { get; }
    DbSet<OrderEntity> Orders { get; }
    DbSet<OrderItemEntity> OrderItems { get; }
    DbSet<TicketsEntity> Tickets { get; }
    DbSet<TicketTypeEntity> TicketTypes { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}