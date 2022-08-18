using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestRooms6_Model;

namespace QuestRooms6_DataAccess.Data
{
    public class QuestRoomsContextDb : IdentityDbContext
    {
        public QuestRoomsContextDb(DbContextOptions<QuestRoomsContextDb> options) : base(options)
        {
        }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<AplicationUser> AplicationUser { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

        public object FirstOrDefaultAsync(Func<object, object> value)
        {
            throw new NotImplementedException();
        }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Room>().ToTable("Rooms");
        //    modelBuilder.Entity<OrderHeader>().ToTable("OrderHeader");
        //    modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
        //    modelBuilder.Entity<AplicationUser>().ToTable("AplicationUser");
        //}

    }
}
