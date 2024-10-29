
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace AssetManagement.Data
{
    public class AssetManagementContext : DbContext
    {
        public AssetManagementContext(DbContextOptions<AssetManagementContext> options)
            : base(options)
        {
        }

        public DbSet<User> tbl_ictams_users { get; set; } = default!;
        public DbSet<Status> tbl_ictams_status { get; set; }
        public DbSet<ProfileAccess> tbl_ictams_profileaccess { get; set; }
        public DbSet<Profile> tbl_ictams_profiles { get; set; }
        public DbSet<Module> tbl_ictams_modules { get; set; }
        public DbSet<Parameter> tbl_ictams_parameters { get; set; }
        public DbSet<Category> tbl_ictams_category { get; set; }
        public DbSet<Store> tbl_ictams_stores { get; set; }
        public DbSet<UserStore> tbl_user_stores { get; set; }
        public DbSet<UserDepartment> tbl_ictams_userdept { get; set; }
        public DbSet<Department> tbl_ictams_department { get; set; }
        public DbSet<Owner> tbl_ictams_owners { get; set; }
        public DbSet<LaptopInventory> tbl_ictams_laptopinv { get; set; }
        public DbSet<LaptopAllocation> tbl_ictams_laptopalloc { get; set; }
        public DbSet<Location> tbl_ictams_location { get; set; }
        public DbSet<Brand> tbl_ictams_brand { get; set; }
        public DbSet<Capacity> tbl_ictams_capacity { get; set; }
        public DbSet<CPU> tbl_ictams_cpu { get; set; }
        public DbSet<HardDisk> tbl_ictams_hardisk { get; set; }
        public DbSet<Level> tbl_ictams_level { get; set; }
        public DbSet<Memory> tbl_ictams_memory { get; set; }
        public DbSet<Model> tbl_ictams_model { get; set; }
        public DbSet<OS> tbl_ictams_os { get; set; }
        public DbSet<Vendor> tbl_ictams_vendor { get; set; }
        public DbSet<DeviceType> tbl_ictams_devicetype { get; set; }
        public DbSet<SecondOwnerAlloc> tbl_ictams_ltnewalloc { get; set; }
        public DbSet<LaptopPeripheral> tbl_ictams_ltperipheral { get; set; }
        public DbSet<LaptopPeripheralAllocation> tbl_ictams_ltperipheralalloc { get; set; }
        public DbSet<SecondOwnerPeripAlloc> tbl_ictams_ltpsecowner { get; set; }
        public DbSet<ReturnType> tbl_ictams_returntype { get; set; }
        public DbSet<ReturnLTA> tbl_ictams_ltareturn { get; set; }
        public DbSet<ReturnPeripheral> tbl_ictams_ltpareturn { get; set; }
        public DbSet<LaptopBorrowed> tbl_ictams_ltborrowed { get; set; }
        public DbSet<BorrowedPeripherals> tbl_ictams_ltpborrowed { get; set; }
        public DbSet<MainBoard> tbl_ictams_mainboard { get; set; }
        public DbSet<InventoryDetails> tbl_ictams_laptopinvdetails { get; set; }
        public DbSet<DesktopInventory> tbl_ictams_desktopinv { get; set; }
        public DbSet<GraphicsCard> tbl_ictams_graphicscard { get; set; }
        public DbSet<DesktopInventoryDetail> tbl_ictams_desktopinvdetails { get; set; }
        public DbSet<DesktopAllocation> tbl_ictams_desktopalloc { get; set; }
        public DbSet<ReturnDTA> tbl_ictams_dtareturn { get; set; }
        public DbSet<DesktopNewAlloc> tbl_ictams_dtnewalloc { get; set; }
        public DbSet<DesktopBorrowed> tbl_ictams_dtborrowed { get; set; }
        public DbSet<MonitorBorrowed> tbl_ictams_monitorborrowed { get; set; }
        public DbSet<MonitorNewAlloc> tbl_ictams_monitornewalloc { get; set; }
        public DbSet<MonitorInventory> tbl_ictams_monitorinv { get; set; }
        public DbSet<MonitorDetail> tbl_ictams_monitordetails { get; set; }
        public DbSet<MonitorAllocation> tbl_ictams_monitoralloc { get; set; }
        public DbSet<ReturnMTA> tbl_ictams_monitorreturn { get; set; }
        public DbSet<UpsType> tbl_ictams_upstype { get; set; }
        public DbSet<UpsCondition> tbl_ictams_upscondition { get; set; }
        public DbSet<UpsBattStatus> tbl_ictams_upsbattstatus { get; set; }
        public DbSet<Ups> tbl_ictams_ups { get; set; }
        public DbSet<UpsPM> tbl_ictams_upspm { get; set; }
        public DbSet<UpsServe> tbl_ictams_upsserve { get; set; }
        public DbSet<UpsBatteryRep> tbl_ictams_upsbattrep { get; set; }
        public DbSet<ReturnLTA>? ReturnLTA { get; set; }
        public DbSet<LaptopBorrowed>? LaptopBorrowed { get; set; }
        public DbSet<DesktopInventory>? DesktopInventory { get; set; }
        public DbSet<DesktopAllocation>? DesktopAllocation { get; set; }
        public DbSet<DesktopNewAlloc>? DesktopNewAlloc { get; set; }
        public DbSet<DesktopBorrowed>? DesktopBorrowed { get; set; }
        public DbSet<Ups>? Ups { get; set; }
        public DbSet<UpsBattStatus>? UpsBattStatus { get; set; }
        public DbSet<UpsServe>? UpsServe { get; set; }
        public DbSet<UpsPM>? UpsPM { get; set; }
        public DbSet<UpsCondition>? UpsCondition { get; set; }
        public DbSet<UpsType>? UpsType { get; set; }
        public DbSet<UpsBatteryRep>? UpsBatteryRep { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasKey(u => u.UserCode);

            modelBuilder.Entity<User>()
                .Property(u => u.UserCode)
                .HasMaxLength(15);

            modelBuilder.Entity<User>()
                .Property(u => u.UserPassword)
                .HasMaxLength(15);

            modelBuilder.Entity<User>()
                .Property(u => u.UserADLogin)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.UserFullName)
                .HasMaxLength(150);

            modelBuilder.Entity<User>()
                .Property(u => u.UserStatus)
                .HasMaxLength(2);

            modelBuilder.Entity<User>()
                .Property(u => u.UserCreated)
                .HasMaxLength(15);

            modelBuilder.Entity<User>()
                .Property(u => u.UserUpdated)
                .HasMaxLength(15);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Status)
                .WithMany()
                .HasForeignKey(u => u.UserStatus)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithMany()
                .HasForeignKey(u => u.UserProfile)
                .OnDelete(DeleteBehavior.Restrict);
       
            modelBuilder.Entity<ProfileAccess>()
                  .HasKey(pa => new { pa.ProfileId, pa.ModuleId });

            modelBuilder.Entity<Profile>()
          .HasKey(p => p.ProfileId);

            modelBuilder.Entity<Profile>()
                .Property(p => p.ProfileStatus)
                .HasMaxLength(2);

            modelBuilder.Entity<Profile>()
                .Property(p => p.ProfileCreated)
                .HasMaxLength(15);

            modelBuilder.Entity<Profile>()
                .Property(p => p.ProfileUpdated)
                .HasMaxLength(15);

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.Status)
                .WithMany()
                .HasForeignKey(p => p.ProfileStatus)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.ProfileCreated)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.ProfileUpdated)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Module>()
               .HasOne(m => m.Status)
               .WithMany()
               .HasForeignKey(m => m.ModuleStatus);

            modelBuilder.Entity<Module>()
                .HasOne(m => m.CreatedBy)
                .WithMany()
                .HasForeignKey(m => m.ModuleCreated);

            modelBuilder.Entity<Module>()
                .HasOne(m => m.UpdatedBy)
                .WithMany()
                .HasForeignKey(m => m.ModuleUpdated);

            // user store
            modelBuilder.Entity<UserStore>()
            .HasKey(us => new { us.UserCode, us.StoreCode });

            modelBuilder.Entity<UserStore>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserCode);

            modelBuilder.Entity<UserStore>()
                .HasOne(us => us.Store)
                .WithMany()
                .HasForeignKey(us => us.StoreCode);

            //User Department 

            // user store
            modelBuilder.Entity<UserDepartment>()
            .HasKey(us => new { us.UserCode, us.DeptCode });

            modelBuilder.Entity<UserDepartment>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserCode);

            modelBuilder.Entity<UserDepartment>()
                .HasOne(us => us.Department)
                .WithMany()
                .HasForeignKey(us => us.DeptCode);

            modelBuilder.Entity<DesktopInventoryDetail>()
                .HasKey(c => new { c.desktopInvCode, c.unitTag });

            modelBuilder.Entity<DesktopBorrowed>()
                .HasOne(b => b.InventoryDetails)
                .WithMany() 
                .HasForeignKey(b => new { b.UnitID, b.UnitTag })
                .HasPrincipalKey(d => new { d.desktopInvCode, d.unitTag });

            modelBuilder.Entity<DesktopNewAlloc>()
                .HasOne(b => b.InventoryDetails)
                .WithMany()  
                .HasForeignKey(b => new { b.SecOwnerCode, b.UnitTag })
                .HasPrincipalKey(d => new { d.desktopInvCode, d.unitTag });

            modelBuilder.Entity<DesktopAllocation>()
                .HasOne(b => b.InventoryDetails)
                .WithMany()  
                .HasForeignKey(b => new { b.DesktopCode, b.UnitTag })
                .HasPrincipalKey(d => new { d.desktopInvCode, d.unitTag });



            modelBuilder.Entity<InventoryDetails>()
              .HasKey(c => new { c.laptoptinvCode, c.SerialCode});

            modelBuilder.Entity<LaptopAllocation>()
               .HasOne(b => b.LaptopInventoryDetails)
               .WithMany()
               .HasForeignKey(b => new { b.LaptopCode, b.SerialNumber})
               .HasPrincipalKey(d => new { d.laptoptinvCode, d.SerialCode});

            modelBuilder.Entity<SecondOwnerAlloc>()
              .HasOne(b => b.LaptopInventoryDetails)
              .WithMany()
              .HasForeignKey(b => new { b.SecOwnerCode, b.SerialNumber })
              .HasPrincipalKey(d => new { d.laptoptinvCode, d.SerialCode });

            modelBuilder.Entity<LaptopBorrowed>()
                .HasOne(b => b.LaptopInventoryDetails)
                .WithMany()
                .HasForeignKey(b => new { b.UnitID, b.SerialNumber })
                .HasPrincipalKey(d => new { d.laptoptinvCode, d.SerialCode });


            modelBuilder.Entity<MonitorDetail>()
            .HasKey(c => new { c.monitorCode, c.SerialNumber});

            modelBuilder.Entity<MonitorAllocation>()
              .HasOne(b => b.MonitorDetails)
              .WithMany()
              .HasForeignKey(b => new { b.monitorCode, b.SerialNumber })
              .HasPrincipalKey(d => new { d.monitorCode, d.SerialNumber});

            modelBuilder.Entity<MonitorBorrowed>()
            .HasOne(b => b.MonitorDetail)
            .WithMany()
            .HasForeignKey(b => new { b.UnitID, b.SerialNumber })
            .HasPrincipalKey(d => new { d.monitorCode, d.SerialNumber});

            modelBuilder.Entity<MonitorNewAlloc>()
             .HasOne(b => b.MonitorDetail)
             .WithMany()
             .HasForeignKey(b => new { b.SecOwnerCode, b.SerialNumber })
             .HasPrincipalKey(d => new { d.monitorCode, d.SerialNumber});
        }
    }
}


