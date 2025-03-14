﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_laptopalloc")]
    public class LaptopAllocation
    {
        [Key]
        [Column("alloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ID")]
        public string AllocId { get; set; }

        [Column("ltinv_code", TypeName = "VARCHAR(10)")]
        [DisplayName("LAPTOP CODE")]
        public string LaptopCode { get; set; }

        [Column("serial_number", TypeName = "VARCHAR(30)")]
        [DisplayName("SERIAL")]
        public string SerialNumber { get; set; }

        [ForeignKey("LaptopCode, SerialNumber")]
        [DisplayName("INVENTORY DETAILS")]
        public InventoryDetails LaptopInventoryDetails { get; set; }

        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [DisplayName("OWNER")]
        public string OwnerCode { get; set; }

        [Column("computer_name", TypeName = "VARCHAR(30)")]
        [DisplayName("COMPUTER NAME")]
        public string? ComputerName { get; set; }

        [Column("fixedasset_tag", TypeName = "VARCHAR(30)")]
        [DisplayName("ASSET TAG")]
        public string? FixedassetTag { get; set; }

        [Column("date_deployed")]
        [DisplayName("DATE DEPLOYED")]
        public DateTime DateDeployed { get; set; }

        [Column("alloc_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string AllocationStatus { get; set; }

        [Column("alloc_created", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string AllocCreated { get; set; }

        [Column("alloc_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("alloc_updated", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? AllocUpdated { get; set; }

        [Column("alloc_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }





        [ForeignKey("AllocationStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("OwnerCode")]
        [DisplayName("OWNER")]
        public Owner Owner { get; set; }

        [ForeignKey("AllocCreated")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [ForeignKey("AllocUpdated")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }
    }
}
