using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors.Infrastructure;
using App.Models;

namespace App.Data;

public partial class AutoRepairWorkshopContext : DbContext
{
    public AutoRepairWorkshopContext()
    {
    }

    public AutoRepairWorkshopContext(DbContextOptions<AutoRepairWorkshopContext> options)
    : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarService> CarServices { get; set; }

    public virtual DbSet<CarStatus> CarStatus { get; set; }

    public virtual DbSet<Mechanic> Mechanics { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RepairOrder> RepairOrders { get; set; }

    public virtual DbSet<CachedDataService> Services { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CachedDataService>().HasKey(c => c.ServiceId);
        modelBuilder.Entity<CarStatus>().HasKey(cs => cs.StatusId);
        modelBuilder.Entity<RepairOrder>().HasKey(ro => ro.OrderId);

        // Связь Car и RepairOrder
        modelBuilder.Entity<Car>()
            .HasMany(c => c.RepairOrders)
            .WithOne(ro => ro.Car)
            .HasForeignKey(ro => ro.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь RepairOrder и CarStatus
        modelBuilder.Entity<RepairOrder>()
            .HasOne(ro => ro.Status)
            .WithMany(cs => cs.RepairOrders)
            .HasForeignKey(ro => ro.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь RepairOrder и Mechanic
        modelBuilder.Entity<RepairOrder>()
            .HasOne(ro => ro.Mechanic)
            .WithMany(m => m.RepairOrders)
            .HasForeignKey(ro => ro.MechanicId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь CarService и RepairOrder
        modelBuilder.Entity<CarService>()
            .HasOne(cs => cs.Order)
            .WithMany(ro => ro.CarServices)
            .HasForeignKey(cs => cs.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь CarService и CachedDataService
        modelBuilder.Entity<CarService>()
            .HasOne(cs => cs.Service)
            .WithMany(s => s.CarServices)
            .HasForeignKey(cs => cs.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        OnModelCreatingPartial(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }


}
