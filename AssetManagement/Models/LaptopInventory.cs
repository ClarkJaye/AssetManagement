using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_laptopinv")]
    public class LaptopInventory
    {
        [Key]
        [Column("ltinv_code", TypeName = "VARCHAR(10)")]
        [DisplayName("CODE")]
        public string laptoptinvCode { get; set; }

        [Column("ltinv_description", TypeName = "VARCHAR(150)")]
        [DisplayName("DESCRIPTION")]
        public string Description { get; set; }

        [Column("ltinv_level")]
        [DisplayName("LEVEL")]
        public int LTLevel { get; set; } 

        [Column("ltinv_brand")]
        [DisplayName("BRAND")]
        public int LTBrand { get; set; }

        [Column("ltinv_model")]
        [DisplayName("MODEL")]
        public int LTModel { get; set; }

        [Column("ltinv_cpu")]
        [DisplayName("CPU")]
        public int LTcpu { get; set; }

        [Column("ltinv_harddisk")]
        [DisplayName("HARD DISK")]
        public int LTHardisk { get; set; }

        [Column("ltinv_memory")]
        [DisplayName("MEMORY")]
        public int LTMemory { get; set; }

        [Column("ltinv_os")]
        [DisplayName("OS")]
        public int LTOS { get; set; }

        [Column("ltinv_qty")]
        [DisplayName("QTY")]
        public int Quantity { get; set; }

        [Column("ltinv_alloc")]
        [DisplayName("ALLOCATED NO")]
        public int AllocatedNo { get; set; }

        [Column("ltinv_status", TypeName = "VARCHAR(2)")]
        public string? LTStatus { get; set; }

        [Column("ltinv_created", TypeName = "VARCHAR(15)")]
        public string LTCreated { get; set; }

        [Column("ltinv_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("ltinv_updated", TypeName = "VARCHAR(15)")]
        public string? LTUpdated { get; set; }

        [Column("ltinv_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }



        [ForeignKey("LTLevel")]
        [DisplayName("LEVEL")]
        public Level Level { get; set; }

        [ForeignKey("LTBrand")]
        [DisplayName("BRAND")]
        public Brand Brand { get; set; }

        [ForeignKey("LTModel")]
        [DisplayName("MODEL")]
        public Model Model { get; set; }

        [ForeignKey("LTcpu")]
        [DisplayName("CPU")]
        public CPU CPU  { get; set; }

        [ForeignKey("LTHardisk")]
        [DisplayName("HARD DISK")]
        public HardDisk HardDisk { get; set; }

        [ForeignKey("LTMemory")]
        [DisplayName("MEMORY")]
        public Memory Memory { get; set; }

        [ForeignKey("LTOS")]
        [DisplayName("OS")]
        public OS OS { get; set; }

        [ForeignKey("LTStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("LTCreated")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [ForeignKey("LTUpdated")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }



    }
}
