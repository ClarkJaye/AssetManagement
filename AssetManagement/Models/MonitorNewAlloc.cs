using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_monitornewalloc")]
    public class MonitorNewAlloc
    {
        [Key]
        [Column("newalloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ID")]
        public string SecAllocId { get; set; }

        [Column("alloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ALLOCATED ID")]
        public string AllocId { get; set; }

        [Column("monitor_code", TypeName = "VARCHAR(10)")]
        [DisplayName("MONITOR CODE")]
        public string SecMonitorCode { get; set; }

        [Column("monitor_serial", TypeName = "VARCHAR(30)")]
        [DisplayName("SERIAL NUMBER")]
        public string SerialNumber { get; set; }

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

        [ForeignKey("SerialNumber")]
        //[DisplayName("UNIT TAG")]
        public MonitorDetail MonitorDetail { get; set; }

        [ForeignKey("AllocId")]
        //[DisplayName("ID")]
        public MonitorAllocation MonitorAllocation { get; set; }

        [ForeignKey("SecAllocationStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("SecMonitorCode")]
        [DisplayName("MONITOR CODE")]
        public MonitorInventory MonitorInventory { get; set; }

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
