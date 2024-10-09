using NuGet.ContentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{
    public class LaptopPeripheralAllocation
    {
        [Key]
        [Column("peripalloc_id", TypeName = "VARCHAR(15)")]
       
        public string PeripheralAllocationId { get; set; }

        [Column("peripheral_code", TypeName = "VARCHAR(10)")]
        [DisplayName("PERIPHERAL CODE")]

        public string PeripheralCode { get; set; }

        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [DisplayName("OWNER CODE")]

        public string OwnerCode { get; set; }

        [Column("serial_number", TypeName = "VARCHAR(15)")]
        [DisplayName("SERIAL NO")]
        public string SerialNumber { get; set; }

        [Column("fixedasset_tag", TypeName = "VARCHAR(30)")]
        [DisplayName("ASSET TAG")]
        public string FixedAssetTag { get; set; }

        [Column("date_purchased")]
        [DisplayName("DATE PURCHASED")]
        public DateTime? DatePurchased { get; set; }

        [Column("age_years" , TypeName = "DECIMAL(10, 2)")]
        [DisplayName("AGE YEARS")]
        public Decimal AgeYears { get; set; }

        [Column("po_number", TypeName = "VARCHAR(15)")]
        [DisplayName("PO NUMBER")]
        public string? PONumber { get; set; }

        [Column("peripalloc_status", TypeName = "CHAR(2)")]
        
        public string PeripheralAllocStatus { get; set; }

        [Column("peripalloc_created", TypeName = "VARCHAR(15)")]
   
        public string PeripheralAllocCreatedBy { get; set; }

        [Column("peripalloc_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime PeripheralAllocCreatedAt { get; set; }

        [Column("peripalloc_updated", TypeName = "VARCHAR(15)")]
      
        public string? PeripheralAllocUpdatedBy { get; set; }

        [Column("peripalloc_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? PeripheralAllocUpdateAt    { get; set; }

        [Column("unit_price")]
        [DisplayName("UNIT PRICE")]
        public Decimal? UnitPrice { get; set; }


        [ForeignKey("PeripheralCode")]
        [DisplayName("LAPTOP PERIPHERAL")]
        public LaptopPeripheral LaptopPeripheral { get; set; }

        [ForeignKey("OwnerCode")]
        [DisplayName("OWNER")]
        public Owner Owner { get; set; }

        [ForeignKey("PeripheralAllocStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("PeripheralAllocCreatedBy")]
        [DisplayName("CREATED BY")]
        public User CreatedBy { get; set; }

        [ForeignKey("PeripheralAllocUpdatedBy")]
        [DisplayName("UPDATED BY")]
        public User UpdatedBy { get; set; }



    }
}
