using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using InventoryOrders.Core.Entities;
using InventoryOrders.Core.Entities.BaseEntities;
using InventoryOrders.Core.Entities.EntityLogs;
using InventoryOrders.Infrastructure.Extensions;
using InventoryOrders.Infrastructure.Healper.Acls;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace InventoryOrders.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    private readonly ISignInHelper _signInHelper;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ISignInHelper signInHelper)
        : base(options)
    {
        _signInHelper = signInHelper;
    }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RouteLog> RouteLogs => Set<RouteLog>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasIndex(p => p.SKU)
                  .IsUnique();

            entity.Property(p => p.Name)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(p => p.SKU)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(p => p.Price)
                  .HasPrecision(10, 2);

            entity.Property(p => p.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.CustomerName)
                  .HasMaxLength(150);

            entity.Property(o => o.OrderDate)
                  .HasDefaultValueSql("GETDATE()");

            entity.Property(o => o.TotalAmount)
                  .HasPrecision(10, 2);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.UnitPrice)
                  .HasPrecision(10, 2);

            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(oi => oi.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.RelationConvetion();
        modelBuilder.DateTimeConvention();
        modelBuilder.DecimalConvention();
        modelBuilder.ConfigureDecimalProperties();
        modelBuilder.PluralzseTableNameConventions();
        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

   
    }

    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.LogTo(message => WriteSqlQueryLog(message));
        optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() }));
    }



    public override int SaveChanges()
    {
        Audit();      // Track changes for auditing
        AuditTrail(); // Log detailed changes
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Audit();
            AuditTrail();
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
      
    }

    private static void WriteSqlQueryLog(string query, StoreType storeType = StoreType.Output)
    {
        if (storeType == StoreType.Output)
            Debug.WriteLine(query);
        else if (storeType == StoreType.Db)
        {
            // store in db
        }
        else if (storeType == StoreType.File)
        {
            // store & append in file
            //new StreamWriter("mylog.txt", append: true);
        }

    }

    private void Audit()
    {
        long userId = 0;
        var now = DateTimeOffset.UtcNow;

        if (_signInHelper.IsAuthenticated)
            userId = (long)_signInHelper.UserId;

        foreach (var entry in base
            .ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State == EntityState.Added
                     || e.State == EntityState.Modified))
        {
            if (entry.State != EntityState.Added)
            {
                entry.Entity.ModifiedDate ??= now;
                entry.Entity.ModifiedBy ??= userId;
            }
            else
            {
                entry.Entity.CreatedBy = entry.Entity.CreatedBy != 0 ? entry.Entity.CreatedBy : userId;
                entry.Entity.CreatedDate = entry.Entity.CreatedDate == DateTimeOffset.MinValue ? now : entry.Entity.CreatedDate;
            }
        }
    }

    private void AuditTrail()
    {
        long userId = 0;

        if (_signInHelper.IsAuthenticated)
            userId = (long)_signInHelper.UserId;

        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity
                || entry.Entity is AuditLog
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAuditLog());
        }
    }
}
public enum StoreType
{
    Db,
    File,
    Output
}
