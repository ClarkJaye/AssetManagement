using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_ups")]
    public class Ups
    {
        [Key]
        [Column("ups_code", TypeName = "VARCHAR(10)")]
        [DisplayName("UPS CODE")]
        public string ups_code { get; set; }

        [Column("ups_store", TypeName = "VARCHAR(5)")]
        [DisplayName("UPS STORE")]
        public string ups_store { get; set; }

        [Column("ups_brand")]
        [DisplayName("UPS BRAND")]
        public int ups_brand { get; set; }

        [Column("ups_model")]
        [DisplayName("UPS MODEL")]
        public int ups_model { get; set; }

        [Column("ups_type")]
        [DisplayName("UPS TYPE")]
        public int ups_type { get; set; }

        [Column("ups_serialno", TypeName = "VARCHAR(30)")]
        [DisplayName("SERIAL NUMBER")]
        public string ups_serial { get; set; }

        [Column("ups_warranty", TypeName = "VARCHAR(1)")]
        [DisplayName("WARRANTY")]
        public string ups_warranty { get; set; }

        [Column("ups_validity")]
        [DisplayName("VALIDITY")]
        public DateTime ups_validity { get; set; }

        [Column("ups_iovoltage", TypeName = "VARCHAR(20)")]
        [DisplayName("IO VOLTAGE")]
        public string ups_iovoltage { get; set; }

        [Column("ups_dcvoltage", TypeName = "VARCHAR(20)")]
        [DisplayName("DC VOLTAGE")]
        public string ups_dcvoltage { get; set; }

        [Column("ups_idealload", TypeName = "VARCHAR(6)")]
        [DisplayName("IDEAL LOAD")]
        public string ups_idealload { get; set; }

        [Column("ups_currentload", TypeName = "VARCHAR(6)")]
        [DisplayName("CURRENT LOAD")]
        public string ups_currentload { get; set; }

        [Column("ups_vendor")]
        [DisplayName("VENDOR")]
        public int ups_vendor { get; set; }

        [Column("ups_dtinstalled")]
        [DisplayName("DATE INSTALLED")]
        public DateTime ups_dtinstalled { get; set; }

        [Column("ups_condition")]
        [DisplayName("UPS CONDITION")]
        public int ups_condition { get; set; }

        [Column("ups_serviceyrs", TypeName = "VARCHAR(30)")]
        [DisplayName("SERVICE YEARS")]
        public string ups_serviceyrs { get; set; }

        [Column("ups_lastpmdt")]
        [DisplayName("LASTPMDATE")]
        public DateTime? ups_lastpmdt { get; set; }

        [Column("ups_battstatus")]
        [DisplayName("BATTERY STATUS")]
        public int ups_battstatus { get; set; }

        [Column("ups_battrepcount")]
        [DisplayName("BATTERY COUNT")]

        public int ups_battrepcount { get; set; }

        [Column("ups_unitserve")]
        [DisplayName("UNIT SERVE")]
        public int ups_unitserve { get; set; }

        [Column("ups_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string ups_status { get; set; }

        [Column("ups_remarks", TypeName = "VARCHAR(100)")]
        [DisplayName("REMARKS")]
        public string? ups_remarks { get; set; }

        [Column("ups_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string ups_createdby { get; set; }

        [Column("ups_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime ups_createddt { get; set; }

        [Column("ups_updatedby", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? ups_updatedby { get; set; }

        [Column("ups_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? ups_updateddt { get; set; }

        [ForeignKey("ups_brand")]
        [DisplayName("UPS BRAND")]
        public Brand Brand { get; set; }

        [ForeignKey("ups_model")]
        [DisplayName("UPS MODEL")]
        public Model Model { get; set; }

        [ForeignKey("ups_type")]
        [DisplayName("UPS TYPE")]
        public UpsType UpsType { get; set; }

        [ForeignKey("ups_vendor")]
        [DisplayName("VENDOR")]
        public Vendor Vendor { get; set; }

        [ForeignKey("ups_condition")]
        [DisplayName("UPS CONDITION")]
        public UpsCondition UpsCondition { get; set; }

        [ForeignKey("ups_battstatus")]
        [DisplayName("BATTERY STATUS")]
        public UpsBattStatus UpsBattStatus { get; set; }

        [ForeignKey("ups_status")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("ups_createdby")]
        [DisplayName("CREATED BY")]
        public User Createdby{ get; set; }

        [ForeignKey("ups_updatedby")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }

        [ForeignKey("ups_store")]
        [DisplayName("UPS STORE")]
        public Store Store { get; set; }

    }
}
