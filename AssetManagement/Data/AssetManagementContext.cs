
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Models;

namespace AssetManagement.Data
{
    public class AssetManagementContext : DbContext  
    {
        public AssetManagementContext (DbContextOptions<AssetManagementContext> options)
            : base(options)
        {
        }

        public DbSet<AssetManagement.Models.User> tbl_ictams_users { get; set; } = default!;
        public DbSet<AssetManagement.Models.Status> tbl_ictams_status { get; set; }
        public DbSet<AssetManagement.Models.ProfileAccess> tbl_ictams_profileaccess { get; set; }
        public DbSet<AssetManagement.Models.Profile> tbl_ictams_profiles { get; set; }
        public DbSet<AssetManagement.Models.Module> tbl_ictams_modules { get; set; }
        public DbSet<AssetManagement.Models.Parameter> tbl_ictams_parameters { get; set; }
        public DbSet<AssetManagement.Models.Category> tbl_ictams_category { get; set; }
        public DbSet<AssetManagement.Models.Store> tbl_ictams_stores { get; set; }
        public DbSet<AssetManagement.Models.UserStore> tbl_user_stores { get; set; }
        public DbSet<AssetManagement.Models.UserDepartment> tbl_ictams_userdept { get; set; }
        public DbSet<AssetManagement.Models.Department> tbl_ictams_department { get; set; }

        public DbSet<AssetManagement.Models.Owner> tbl_ictams_owners { get; set; }
        public DbSet<AssetManagement.Models.LaptopInventory> tbl_ictams_laptopinv { get; set; }
        public DbSet<AssetManagement.Models.LaptopAllocation> tbl_ictams_laptopalloc { get; set; }
        public DbSet<AssetManagement.Models.Location> tbl_ictams_location { get; set; }
        public DbSet<AssetManagement.Models.Brand> tbl_ictams_brand { get; set; }
        public DbSet<AssetManagement.Models.Capacity> tbl_ictams_capacity { get; set; }
        public DbSet<AssetManagement.Models.CPU> tbl_ictams_cpu { get; set; }
        public DbSet<AssetManagement.Models.HardDisk> tbl_ictams_hardisk { get; set; }
        public DbSet<AssetManagement.Models.Level> tbl_ictams_level { get; set; }
        public DbSet<AssetManagement.Models.Memory> tbl_ictams_memory { get; set; }
        public DbSet<AssetManagement.Models.Model> tbl_ictams_model { get; set; }
        public DbSet<AssetManagement.Models.OS> tbl_ictams_os { get; set; }
        public DbSet<AssetManagement.Models.Vendor> tbl_ictams_vendor { get; set; }
        public DbSet<AssetManagement.Models.DeviceType> tbl_ictams_devicetype { get; set; }

        public DbSet<AssetManagement.Models.SecondOwnerAlloc> tbl_ictams_ltnewalloc { get; set; }
        public DbSet<AssetManagement.Models.LaptopPeripheral> tbl_ictams_ltperipheral { get; set; }
        public DbSet<AssetManagement.Models.LaptopPeripheralAllocation> tbl_ictams_ltperipheralalloc { get; set; }
        public DbSet<AssetManagement.Models.SecondOwnerPeripAlloc> tbl_ictams_ltpsecowner { get; set; }
        public DbSet<AssetManagement.Models.ReturnType> tbl_ictams_returntype { get; set; }
        public DbSet<AssetManagement.Models.ReturnLTA> tbl_ictams_ltareturn { get; set; }
        public DbSet<AssetManagement.Models.ReturnPeripheral> tbl_ictams_ltpareturn { get; set; }
        public DbSet<AssetManagement.Models.LaptopBorrowed> tbl_ictams_ltborrowed { get; set; }
        public DbSet<AssetManagement.Models.BorrowedPeripherals> tbl_ictams_ltpborrowed { get; set; }
        public DbSet<AssetManagement.Models.MainBoard> tbl_ictams_mainboard { get; set; }
        public DbSet<AssetManagement.Models.InventoryDetails> tbl_ictams_laptopinvdetails { get; set; }
        public DbSet<AssetManagement.Models.DesktopInventory> tbl_ictams_desktopinv { get; set; }
        public DbSet<AssetManagement.Models.GraphicsCard> tbl_ictams_graphicscard { get; set; }
        public DbSet<AssetManagement.Models.DesktopInventoryDetail> tbl_ictams_desktopinvdetails { get; set; }
        public DbSet<AssetManagement.Models.DesktopAllocation> tbl_ictams_desktopalloc { get; set; }
        public DbSet<AssetManagement.Models.ReturnDTA> tbl_ictams_dtareturn { get; set; }
        public DbSet<AssetManagement.Models.DesktopNewAlloc> tbl_ictams_dtnewalloc { get; set; }
        public DbSet<AssetManagement.Models.DesktopBorrowed> tbl_ictams_dtborrowed { get; set; }
        public DbSet<AssetManagement.Models.MonitorBorrowed> tbl_ictams_monitorborrowed { get; set; }
        public DbSet<AssetManagement.Models.MonitorNewAlloc> tbl_ictams_monitornewalloc { get; set; }
        public DbSet<AssetManagement.Models.MonitorInventory> tbl_ictams_monitorinv { get; set; }
        public DbSet<AssetManagement.Models.MonitorDetail> tbl_ictams_monitordetails { get; set; }
        public DbSet<AssetManagement.Models.MonitorAllocation> tbl_ictams_monitoralloc { get; set; }
        public DbSet<AssetManagement.Models.ReturnMTA> tbl_ictams_monitorreturn { get; set; }
        public DbSet<AssetManagement.Models.UpsType> tbl_ictams_upstype { get; set; }
        public DbSet<AssetManagement.Models.UpsCondition> tbl_ictams_upscondition { get; set; }
        public DbSet<AssetManagement.Models.UpsBattStatus> tbl_ictams_upsbattstatus { get; set; }
        public DbSet<AssetManagement.Models.Ups> tbl_ictams_ups { get; set; }
        public DbSet<AssetManagement.Models.UpsPM> tbl_ictams_upspm { get; set; }
		public DbSet<AssetManagement.Models.UpsServe> tbl_ictams_upsserve { get; set; }
        public DbSet<AssetManagement.Models.UpsBatteryRep> tbl_ictams_upsbattrep { get; set; }
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

        }
        public DbSet<AssetManagement.Models.ReturnLTA>? ReturnLTA { get; set; }
        public DbSet<AssetManagement.Models.LaptopBorrowed>? LaptopBorrowed { get; set; }
        public DbSet<AssetManagement.Models.DesktopInventory>? DesktopInventory { get; set; }
        public DbSet<AssetManagement.Models.DesktopAllocation>? DesktopAllocation { get; set; }
        public DbSet<AssetManagement.Models.DesktopNewAlloc>? DesktopNewAlloc { get; set; }
        public DbSet<AssetManagement.Models.DesktopBorrowed>? DesktopBorrowed { get; set; }
        public DbSet<AssetManagement.Models.Ups>? Ups { get; set; }
        public DbSet<AssetManagement.Models.UpsBattStatus>? UpsBattStatus { get; set; }
        public DbSet<AssetManagement.Models.UpsServe>? UpsServe { get; set; }
        public DbSet<AssetManagement.Models.UpsPM>? UpsPM { get; set; }
        public DbSet<AssetManagement.Models.UpsCondition>? UpsCondition { get; set; }
        public DbSet<AssetManagement.Models.UpsType>? UpsType { get; set; }
        public DbSet<AssetManagement.Models.UpsBatteryRep>? UpsBatteryRep { get; set; }

    }
    
}
