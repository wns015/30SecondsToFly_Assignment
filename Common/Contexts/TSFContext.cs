using Common.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Contexts
{
    public class TSFContext : DbContext
    {
        public TSFContext(DbContextOptions<TSFContext> options) : base(options) { }

        public virtual DbSet<BookingTableModel> bookingTableModels { get; set; }
        public virtual DbSet<FlightTableModel> flightTableModels { get; set; }
        public virtual DbSet<BookingTransactionTableModel> transactionTableModels { get; set; }
        public virtual DbSet<PassengerDetailTableModel> passengerDetailTableModels { get; set; }

    }
}
