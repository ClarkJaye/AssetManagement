
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_desktopalloc")]
    public class DesktopAllocation
    {
        [Key]
        [Column("alloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ID")]
        public string AllocId { get; set; }


        [Column("dtinv_code", TypeName = "VARCHAR(10)")]
        [DisplayName("DESKTOP CODE")]
        public string DesktopCode { get; set; }

        [Column("unit_tag", TypeName = "VARCHAR(30)")]
        [DisplayName("UNIT TAG")]
        public string UnitTag { get; set; }

        [ForeignKey("DesktopCode, UnitTag")]
        [DisplayName("INVENTORY DETAILS")]
        public DesktopInventoryDetail InventoryDetails { get; set; }

        [Column("computer_name", TypeName = "VARCHAR(30)")]
        [DisplayName("COMPUTER NAME")]
        public string? ComputerName { get; set; }


        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [DisplayName("OWNER")]
        public string OwnerCode { get; set; }

        [Column("fixedasset_tag", TypeName = "VARCHAR(30)")]
        [DisplayName("ASSET TAG")]
        public string? FixedassetTag { get; set; }

        [Column("date_deployed")]
        [DisplayName("DATE  DEPLOYED")]
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
        [DisplayName("CREATEDBY")]
        public User Createdby { get; set; }

        [ForeignKey("AllocUpdated")]
        [DisplayName("UPDATEDBY")]
        public User Updatedby { get; set; }
    }
}
