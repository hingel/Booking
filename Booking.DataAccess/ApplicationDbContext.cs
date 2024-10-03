using Microsoft.EntityFrameworkCore;
using Booking.DataAccess.Models;

namespace Booking.DataAccess;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Models.Booking> Bookings { get; set; } = null!;
	public DbSet<Table> Tables { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//modelBuilder.Entity<Table>().HasQueryFilter(t => t.CompanyId == )
		//modelBuilder.Entity<Booking>().HasQueryFilter(t => t.CompanyId == )

		modelBuilder.Entity<Table>().HasKey(t => t.Id);
		modelBuilder.Entity<Table>(t =>
		{
			t.Property(p => p.Name).IsRequired().HasMaxLength(255);
			t.Property(p => p.CompanyId).IsRequired();
		});

		modelBuilder.Entity<Table>().OwnsMany(t => t.Bookings,
			b =>
			{
				b.HasKey(t => t.Id);
				b.WithOwner(p => p.Table);
				b.Property(p => p.DateTime).IsRequired();
				b.Property(p => p.NoOfPersons).IsRequired();
				b.Property(p => p.CompanyId).IsRequired();
				b.Property(b => b.Duration).IsRequired().HasPrecision(2,1);
				b.OwnsOne(b => b.Contact, c =>
				{
					c.Property(p => p.Name).IsRequired().HasMaxLength(255);
					c.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(255);
					c.Property(p => p.Email).IsRequired().HasMaxLength(255);
				});
			});
	}
}
