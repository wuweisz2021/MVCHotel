using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCHotel.Areas.Identity.Data;

namespace MVCHotel.Motels;

public class MVCHotelContext : IdentityDbContext<MVCHotelUser>
{
    public MVCHotelContext(DbContextOptions<MVCHotelContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        // base.OnModelCreating(modelBuilder);
        optionsBuilder.UseSqlite(@"Data source=Hotel.db");

    }






}
