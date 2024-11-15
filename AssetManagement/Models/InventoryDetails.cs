using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace AssetManagement.Models
{
	[Table("tbl_ictams_laptopinvdetails")]
	public class InventoryDetails
	{
		[Column("ltinv_code", TypeName = "VARCHAR(10)")]
		[DisplayName("LAPTOP CODE")]
		public string laptoptinvCode { get; set; }

		[ForeignKey("laptoptinvCode")]
		[DisplayName("INVENTORY")]
		public LaptopInventory LaptopInventory { get; set; }

		[Column("serial_number", TypeName = "VARCHAR(30)")]
		[DisplayName("SERIAL")]
		public string SerialCode { get; set; }

        [Column("computer_name", TypeName = "VARCHAR(50)")]
        [DisplayName("COMPUTER NAME")]
        public string? ComputerName { get; set; }

        [Column("po_number", TypeName = "VARCHAR(15)")]
		[DisplayName("PO")]
		public string? PO { get; set; }

		[Column("unit_price")]
		[DisplayName("UNIT PRICE")]
		public Decimal?  Price { get; set; }

		[Column("ltinv_vendor")]
		[DisplayName("VENDOR")]
		public int LTDVendor { get; set; }

        [ForeignKey("LTDVendor")]
		[DisplayName("VENDOR")]
		public Vendor Vendor { get; set; }

		[Column("date_purchased")]
		public DateTime? PurchaseDate { get; set; }

		[Column("date_deployed")]
		public DateTime? DeployedDate { get; set; }

		[Column("status", TypeName = "VARCHAR(2)")]
		public string LTStatus { get; set; }

		[ForeignKey("LTStatus")]
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

        [NotMapped]
        public Owner Owner { get; set; }

    }
}
