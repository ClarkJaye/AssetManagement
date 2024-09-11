using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_monitordetails")]
    public class MonitorDetail
    {
        [Column("monitor_code", TypeName = "VARCHAR(10)")]
        [DisplayName("MONITOR CODE")]
        public string monitorCode { get; set; }

        [Key]
        [Column("monitor_serial", TypeName = "VARCHAR(30)")]
        [DisplayName("SERIAL")]
        public string SerialNumber { get; set; }

        [Column("monitor_po", TypeName = "VARCHAR(15)")]
        [DisplayName("PO")]
        public string? PO { get; set; }

        [Column("monitor_price")]
        [DisplayName("PRICE")]
        public Decimal? Price { get; set; }

        [Column("monitor_vendor")]
        [DisplayName("VENDOR")]
        public int MonitorVendor { get; set; }

        [Column("monitor_datepurchased")]
        [DisplayName("DATE PURCHASED")]
        public DateTime? PurchaseDate { get; set; }

        [Column("monitor_deployed")]
        [DisplayName("DATE DEPLOYED")]
        public DateTime? DeployedDate { get; set; }

        [Column("monitor_status", TypeName = "VARCHAR(2)")]
        public string MonitorStatus { get; set; }

        [Column("monitor_createdby", TypeName = "VARCHAR(15)")]
        public string DetailCreated { get; set; }

        [Column("monitor_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("monitor_updatedby", TypeName = "VARCHAR(15)")]
        public string? DetailUpdated { get; set; }

        [Column("monitor_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }


        [ForeignKey("monitorCode")]
        [DisplayName("INVENTORY")]
        public MonitorInventory MonitorInventory { get; set; }

        [ForeignKey("DetailCreated")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [ForeignKey("DetailUpdated")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }

        [ForeignKey("MonitorVendor")]
        [DisplayName("VENDOR")]
        public Vendor Vendor { get; set; }

        [ForeignKey("MonitorStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

    }
}
