using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Payments.Database {

  public class PaymentsContext : DbContext {

    public PaymentsContext (DbContextOptions<PaymentsContext> options): base(options){}

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<Payment>().
      ToTable("payments");
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess) {
      foreach (var entry in ChangeTracker.Entries()) {
        if (entry.State == EntityState.Added) {
          entry.Property("DateCreated").CurrentValue = DateTime.UtcNow;
        }
      }

      return base.SaveChanges(acceptAllChangesOnSuccess);
    }
  }

  public class Payment {
    [Key]
    public int Id { get; set; }
    
    //[StringLength(25, ErrorMessage = "Name value cannot exceed 4 characters. ")]  
    [StringLength(255, ErrorMessage = "Name value cannot exceed 4 characters. ")]  
    public string Name { get; set; }

    [StringLength(20, ErrorMessage = "Type value cannot exceed 20 characters. ")]  
    public string Type { get; set; }
    
    [StringLength(20, ErrorMessage = "Number value cannot exceed 20 characters. ")]  
    public string Number { get; set; }

    [StringLength(5, ErrorMessage = "Exp value cannot exceed 5 characters. ")]  
    public string Exp { get; set; }

    // [StringLength(3, ErrorMessage = "CVV value cannot exceed 3 characters. ")]  
    [StringLength(5, ErrorMessage = "CVV value cannot exceed 5 characters. ")]  
    public string CVV { get; set; }

    [Range(0,20000, ErrorMessage = "Amount value cannot exceed 20000")]  
    public double Amount { get; set; }
   
    public DateTime DateCreated { get; set; }
  }
}