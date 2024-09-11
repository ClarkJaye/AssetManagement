
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace AssetManagement.Models
{
    [Table("tbl_ictams_desktopinvdetails")]
    public class DesktopInventoryDetail
    {
        [Column("dtinv_code", TypeName = "VARCHAR(10)")]
        [DisplayName("DESKTOP CODE")]
        public string desktopInvCode { get; set; }

        [ForeignKey("desktopInvCode")]
        [DisplayName("INVENTORY")]
        public DesktopInventory DesktopInventory { get; set; }

        [Key]
        [Column("unit_tag", TypeName = "VARCHAR(15)")]
        [DisplayName("UNIT TAG")]
        public string unitTag { get; set; }

        [Column("po_number", TypeName = "VARCHAR(15)")]
        [DisplayName("PO")]
        public string? PO { get; set; }

        [Column("unit_price")]
        [DisplayName("UNIT PRICE")]
        public Decimal? Price { get; set; }

        [Column("dtinv_vendor")]
        [DisplayName("VENDOR")]
        public int DTDVendor { get; set; }

        [ForeignKey("DTDVendor")]
        [DisplayName("VENDOR")]
        public Vendor Vendor { get; set; }

        [Column("date_purchased")]
        public DateTime? PurchaseDate { get; set; }

        [Column("date_deployed")]
        public DateTime? DeployedDate { get; set; }

        [Column("status", TypeName = "VARCHAR(2)")]
        public string DTStatus { get; set; }

        [ForeignKey("DTStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [Column("details_created", TypeName = "VARCHAR(15)")]
        public string Created { get; set; }

        [ForeignKey("Created")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [Column("details_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("details_updated", TypeName = "VARCHAR(15)")]
        public string? Updated { get; set; }

        [ForeignKey("Updated")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }

        [Column("details_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? UpdatedDate { get; set; }
    }
}
