using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_vendor")]
    public class Vendor
    {
        [Key]
        [Column("vendor_id")]
        [DisplayName("ID")]
        public int VendorID { get; set; }

        [Column("vendor_name", TypeName = "VARCHAR(100)")]
        [DisplayName("NAME")]
        public string VendorName { get; set; }

        [Column("vendor_address", TypeName = "VARCHAR(250)")]
        [DisplayName("ADDRESS")]
        public string VendorAddress { get; set; }

        [Column("vendor_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string VendorStatus { get; set; }

        [Column("vendor_created", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string VCreatedby { get; set; }

        [Column("vendor_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("vendor_updated", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? VUpdateby { get; set; }

        [Column("vendor_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }


        [ForeignKey("VendorStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("VCreatedby")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [ForeignKey("VUpdateby")]
        [DisplayName("UPDATED BY")]
        public User? Updatedby { get; set; }
    }
}
