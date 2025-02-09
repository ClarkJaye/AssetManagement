﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace AssetManagement.Models
{
    [Table("tbl_ictams_dtnewalloc")]
    public class DesktopNewAlloc
    {
        [Key]
        [Column("newalloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ID")]
        public string SecAllocId { get; set; }

        [Column("alloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ALLOCATED ID")]
        public string AllocId { get; set; }

        [Column("dtinv_code", TypeName = "VARCHAR(10)")]
        [DisplayName("DESKTOP CODE")]
        public string SecDesktopCode { get; set; }

        [Column("unit_tag", TypeName = "VARCHAR(15)")]
        [DisplayName("UNIT TAG")]
        public string UnitTag { get; set; }


        [ForeignKey("SecDesktopCode, UnitTag")]
        [DisplayName("INVENTORY DETAILS")]
        public DesktopInventoryDetail InventoryDetails { get; set; }


        [ForeignKey("AllocId")]
        public DesktopAllocation DesktopAllocation { get; set; }


        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [DisplayName("OWNER")]
        public string SecOwnerCode { get; set; }

        [Column("alloc_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string SecAllocationStatus { get; set; }

        [Column("alloc_created", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string AllocCreated { get; set; }

        [Column("alloc_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("alloc_updated", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED AT")]
        public string? AllocUpdated { get; set; }

        [Column("alloc_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }

        [ForeignKey("SecAllocationStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }



        [ForeignKey("SecOwnerCode")]
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
