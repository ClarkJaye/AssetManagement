using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace AssetManagement.Models
{
    [Table("tbl_ictams_monitoralloc")]
    public class MonitorAllocation
    {
        [Key]
        [Column("alloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ID")]
        public string AllocId { get; set; }

        [Column("monitor_code", TypeName = "VARCHAR(10)")]
        [DisplayName("MONITOR CODE")]
        public string monitorCode { get; set; }

        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [DisplayName("OWNER")]
        public string OwnerCode { get; set; }

        [Column("monitor_serial", TypeName = "VARCHAR(30)")]
        [DisplayName("SERIAL")]
        public string SerialNumber { get; set; }

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



        [ForeignKey("monitorCode")]
        [DisplayName("MONITOR CODE")]
        public MonitorInventory MonitorInventory { get; set; }

        [ForeignKey("SerialNumber")]
        [DisplayName("SERIAL")]
        public MonitorDetail MonitorDetails { get; set; }

        [ForeignKey("OwnerCode")]
        [DisplayName("OWNER")]
        public Owner Owner { get; set; }

        [ForeignKey("AllocationStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("AllocCreated")]
        [DisplayName("CREATEDBY")]
        public User Createdby { get; set; }

        [ForeignKey("AllocUpdated")]
        [DisplayName("UPDATEDBY")]
        public User Updatedby { get; set; }
    }
}
