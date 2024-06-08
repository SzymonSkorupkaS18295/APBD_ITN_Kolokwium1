using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APBD_Kolokwium1.Models;

public partial class S18295Context : DbContext
{
    public S18295Context()
    {
    }

    public S18295Context(DbContextOptions<S18295Context> options)
        : base(options)
    {
    }
    public virtual DbSet<Client> Client { get; set; }

    public virtual DbSet<Discount> Discount { get; set; }

    public virtual DbSet<Payment> Payment { get; set; }
    
    public virtual DbSet<Sale> Sale { get; set; }
    
    public virtual DbSet<Subscription> Subscription { get; set; }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db-mssql16.pjwstk.edu.pl;Database=s18295;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.IdSale);
        });
       
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("Client_pk");

            entity.ToTable("Client");

            entity.Property(e => e.IdClient).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(100);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.IdDiscount).HasName("Discount_pk");

            entity.ToTable("Discount");

            entity.Property(e => e.IdDiscount).ValueGeneratedNever();

            entity.HasOne(d => d.IdSubscriptionNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.IdSubscription)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Discount_Subscription");
        });
        
        
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment).HasName("Payment_pk");

            entity.ToTable("Payment");

            entity.Property(e => e.IdPayment).ValueGeneratedNever();

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Client");

            entity.HasOne(d => d.IdSubscriptionNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdSubscription)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Subscription");
        });

       
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.IdSubscription).HasName("Subscription_pk");

            entity.ToTable("Subscription");

            entity.Property(e => e.IdSubscription).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("money");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
