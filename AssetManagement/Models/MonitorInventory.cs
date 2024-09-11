using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace AssetManagement.Models
{
    [Table("tbl_ictams_monitorinv")]
    public class MonitorInventory
    {
        [Key]
        [Column("monitor_code", TypeName = "VARCHAR(10)")]
        [DisplayName("CODE")]
        public string monitorCode { get; set; }

        [Column("monitor_description", TypeName = "VARCHAR(50)")]
        [DisplayName("DESCRIPTION")]
        public string Description { get; set; }

        [Column("monitor_model")]
        [DisplayName("MODEL")]
        public int MonitorModel { get; set; }

        [Column("monitor_quantity")]
        [DisplayName("QUANTITY")]
        public int Quantity { get; set; }

        [Column("monitor_qtyalloc")]
        [DisplayName("ALLOCATED NO")]
        public int AllocatedNo { get; set; }

        [Column("monitor_status")]
        [DisplayName("QUANTITY")]
        public string MonitorStatus { get; set; }

        [Column("monitor_createdby")]
        [DisplayName("CREATED BY")]
        public string MonitorCreatedBy { get; set; }

        [Column("monitor_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("monitor_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? MonitorUpdatedBy { get; set; }

        [Column("monitor_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }
            
        [ForeignKey("MonitorModel")]
        [DisplayName("Model")]
        public Model Model { get; set; }

        [ForeignKey("MonitorStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("MonitorCreatedBy")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [ForeignKey("MonitorUpdatedBy")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }


    }
}
