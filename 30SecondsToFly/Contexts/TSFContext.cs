using Booking.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace _30SecondsToFly.Contexts
{
    public class TSFContext : DbContext
    {
        public TSFContext(DbContextOptions<TSFContext> options) : base(options) { }

        public DbSet<BookingTableModel> bookingTableModels { get; set; }
        public DbSet<FlightTableModel> flightTableModels { get; set; }
        public DbSet<TransactionTableModel> transactionTableModels { get; set; }
        
    }
}
