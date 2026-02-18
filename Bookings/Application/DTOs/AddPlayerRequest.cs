using System.ComponentModel;

namespace Bookings.Application.DTOs.Requests;

public record AddPlayerRequest(

    [property: DefaultValue("123e4567-e89b-12d3-a456-426614174000")]
    Guid PlayerId,

    [property: DefaultValue("Intermediate")]
    string PlayerRank);
