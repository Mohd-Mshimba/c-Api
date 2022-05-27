using Microsoft.EntityFrameworkCore;
using ZanMalipo.Models;

namespace ZanMalipo.Data
{
    public class ZanMalipoDbContext : DbContext{
        public ZanMalipoDbContext(DbContextOptions<ZanMalipoDbContext> options): base(options){
        }
        public DbSet<User> Users { get; set; }
        public DbSet<BillService> BillServices { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Reconciliation> Reconciliations { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}