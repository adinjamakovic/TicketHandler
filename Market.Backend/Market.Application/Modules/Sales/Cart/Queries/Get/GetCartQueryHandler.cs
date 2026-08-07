using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Queries.Get
{
    public sealed class GetCartQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser, IImageStorage imageStorage)
        : IRequestHandler<GetCartQuery, GetCartQueryDto>
    {
        public async Task<GetCartQueryDto> Handle(GetCartQuery req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var items = await ctx.CartItems
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .OrderBy(x => x.CreatedAtUtc)
                .Select(x => new GetCartQueryDtoItem
                {
                    TicketId = x.TicketId,
                    Event = new GetCartQueryDtoEvent
                    {
                        Id = x.Ticket.EventId,
                        Name = x.Ticket.Event.Name,
                        ScheduledDate = x.Ticket.Event.ScheduledDate,
                        VenueName = x.Ticket.Event.Venue.Name,
                        Image = x.Ticket.Event.Image
                    },
                    TicketType = new GetCartQueryDtoTicketType
                    {
                        Id = x.Ticket.TicketTypeId,
                        Name = x.Ticket.TicketType.Name
                    },
                    Quantity = x.Quantity,
                    UnitPrice = x.Ticket.UnitPrice,
                    Subtotal = x.Quantity * x.Ticket.UnitPrice,
                    QuantityInStock = x.Ticket.QuantityInStock,
                    Benefits = x.Ticket.Benefits,
                    AddedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync(ct);

            items.ApplyPublicImagePaths(
                imageStorage,
                ImageStorageCategory.Events,
                x => x.Event.Image,
                (x, path) => x.Event.Image = path);

            return new GetCartQueryDto
            {
                Items = items,
                LineCount = items.Count,
                TotalQuantity = items.Sum(x => x.Quantity),
                TotalAmount = Math.Round(items.Sum(x => x.Subtotal), 2, MidpointRounding.AwayFromZero)
            };
        }
    }
}
