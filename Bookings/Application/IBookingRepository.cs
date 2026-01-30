using Bookings.Domain;

namespace Bookings.Application;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}
