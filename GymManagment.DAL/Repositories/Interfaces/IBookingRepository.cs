using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default);

    }
}
