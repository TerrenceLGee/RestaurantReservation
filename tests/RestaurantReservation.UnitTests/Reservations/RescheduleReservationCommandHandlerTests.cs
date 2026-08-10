using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Reservations.Command.Reschedule;

namespace RestaurantReservation.UnitTests.Reservations;

public class RescheduleReservationCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<RescheduleReservationCommandHandler> _logger =
        Substitute.For<ILogger<RescheduleReservationCommandHandler>>();

    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Guid _userId = Guid.CreateVersion7();
}