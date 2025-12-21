using Market.Application.Abstractions;
using Market.Domain.Entities.CustomerRelationship;
using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Sales;

namespace Market.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    public DbSet<AdminNewsEntity> AdminNews => Set<AdminNewsEntity>();
    public DbSet<LoyaltyProgrammeEntity> LoyaltyProgrammes => Set<LoyaltyProgrammeEntity>();
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
    public DbSet<EventEntity> Events => Set<EventEntity>();
    public DbSet<EventNewsEntity> EventNews => Set<EventNewsEntity>();
    public DbSet<EventTypeEntity> EventTypes => Set<EventTypeEntity>();
    public DbSet<GenreEntity> Genres => Set<GenreEntity>();
    public DbSet<OrganizerEntity> Organizers => Set<OrganizerEntity>();
    public DbSet<PerformerEntity> Performers => Set<PerformerEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
    public DbSet<PerformerEventEntity> PerformerEvents => Set<PerformerEventEntity>();
    public DbSet<CityEntity> Cities => Set<CityEntity>();
    public DbSet<CountryEntity> Countries => Set<CountryEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<PointOfSaleEntity> PointsOfSale => Set<PointOfSaleEntity>();
    public DbSet<VenueEntity> Venues => Set<VenueEntity>();
    public DbSet<PersonEntity> Persons => Set<PersonEntity>();
    public DbSet<WalletEntity> Wallets => Set<WalletEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<TicketsEntity> Tickets => Set<TicketsEntity>();
    public DbSet<TicketTypeEntity> TicketTypes => Set<TicketTypeEntity>();

    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
}