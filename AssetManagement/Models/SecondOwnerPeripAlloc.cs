using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace AssetManagement.Models
{
    public class SecondOwnerPeripAlloc
    {
        [Key]
        [Column("secalloc_id", TypeName = "VARCHAR(16)")]
        [DisplayName("ID")]
        public string SecAllocId { get; set; }

        [Column("alloc_id", TypeName = "VARCHAR(15)")]
        [DisplayName("ALLOCATED ID")]
        public string AllocId { get; set; }

        [Column("peripheral_code", TypeName = "VARCHAR(10)")]
        [DisplayName("PERIPHERAL CODE")]
        public string SecPeripheralCode { get; set; }

        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [DisplayName("OWNER")]
        public string SecOwnerCode { get; set; }

        [Column("date_purchased")]
        [DisplayName("DATE PURCHASE")]
        public DateTime? DatePurchased { get; set; }

        [Column("age_years")]
        [DisplayName("AGE YEARS")]
        public Decimal? AgeYears { get; set; }

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
        [DisplayName("UPDATED BY")]
        public string? AllocUpdated { get; set; }

        [Column("alloc_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }




        [ForeignKey("AllocId")]
        [DisplayName("PERIPHERAL ALLOCATION")]
        public LaptopPeripheralAllocation LaptopPeripAlloc { get; set; }

        [ForeignKey("SecPeripheralCode")]
        [DisplayName("PERIPHERAL CODE")]
        public LaptopPeripheral LaptopPeripheral { get; set; }

        [ForeignKey("SecOwnerCode")]
        [DisplayName("OWNER")]
        public Owner Owner { get; set; }

        [ForeignKey("SecAllocationStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("AllocCreated")]
        [DisplayName("CREATED BY")]
        public User CreatedBy { get; set; }

        [ForeignKey("AllocUpdated")]
        [DisplayName("UPDATED BY")]
        public User UpdatedBy { get; set; }

    }
}
