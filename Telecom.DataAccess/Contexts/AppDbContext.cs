using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Telecom.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Telecom.DataAccess.Contexts;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ticket Configuration
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CustomerNo).IsRequired().HasMaxLength(50);
            
            // Relation: Ticket -> AssignedTechnician (AppUser)
            entity.HasOne(t => t.AssignedTechnician)
                  .WithMany(u => u.AssignedTickets)
                  .HasForeignKey(t => t.AssignedTechnicianId)
                  .OnDelete(DeleteBehavior.SetNull); // Teknisyen silinirse biletleri sahipsiz kalır
        });

        // AuditLog Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActionType).IsRequired().HasMaxLength(100);
            
            // Relation: AuditLog -> User
            entity.HasOne(a => a.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relation: AuditLog -> Ticket
            entity.HasOne(a => a.Ticket)
                  .WithMany()
                  .HasForeignKey(a => a.TicketId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // AppUser Configuration
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
        });
    }

    // CreatedAt vb. denetim alanlarını SaveChanges üzerinde otomatik atama
    public override int SaveChanges()
    {
        SetBaseProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetBaseProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetBaseProperties()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                if (entry.Entity.Id == Guid.Empty)
                {
                    entry.Entity.Id = Guid.NewGuid();
                }
            }
        }
    }
}
