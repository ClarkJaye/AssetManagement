using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class LaptopPeripheral
    {
        [Key]
        [Column("peripheral_code",TypeName = "VARCHAR(10)")]
        [DisplayName("PERIPHERAL CODE")]

        public string PeripheralCode { get; set; }

        [Column("peripheral_description", TypeName = "VARCHAR(150)")]
        [DisplayName("DESCRIPTION")]
        public string PeripheralDescription { get; set; }

        [Column("peripheral_brand")]
        [DisplayName("PERIPHERAL BRAND")]

        public int PeripheralBrand { get; set; }

        [Column("peripheral_device")]
        [DisplayName("DEVICE")]
        public int PeripheralDevice { get; set; }

        [Column("peripheral_vendor")]
        [DisplayName("VENDOR")]
        public int PeripheralVendor { get; set; }

        [Column("peripheral_qty")]
        [DisplayName("QUANTITY")]
        public int PeripheralQty { get; set; }

        [Column("peripheral_alloc")]
        [DisplayName("ALLOCATED NO")]
        public int PeripheralAllocation { get; set; }

        [Column("peripheral_status", TypeName = "CHAR(2)")]
        public string PeripheralStatus { get; set; }

        [Column("peripheral_created", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string PeripheralCreatedBy { get; set; }

        [Column("peripheral_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime PeripheralCreatedAt { get; set; }

        [Column("peripheral_updated", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? PeripheralUpdatedBy { get; set; }

        [Column("peripheral_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? PeripheralUpdatedAt { get; set; }



        [ForeignKey("PeripheralBrand")]
        [DisplayName("BRAND")]
        public Brand Brand { get; set; }

        [ForeignKey("PeripheralDevice")]
        [DisplayName("DEVICE TYPE")]
        public DeviceType DeviceType { get; set; }

        [ForeignKey("PeripheralVendor")]
        [DisplayName("VENDOR")]
        public Vendor Vendor { get; set; }

        [ForeignKey("PeripheralStatus")]
        
        public Status Status { get; set; }

        [ForeignKey("PeripheralCreatedBy")]
        [DisplayName("CREATED BY")]
        public User CreatedBy { get; set; }

        [ForeignKey("PeripheralUpdatedBy")]
        [DisplayName("UPDATED BY")]
        public User UpdatedBy { get; set; }

    }
}
