using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_desktopinv")]
    public class DesktopInventory
    {
        [Key]
        [Column("dtinv_code", TypeName = "VARCHAR(10)")]
        [DisplayName("CODE")]
        public string desktopInvCode { get; set; }

        [Column("dtinv_description", TypeName = "VARCHAR(150)")]
        [DisplayName("DESCRIPTION")]
        public string Description { get; set; }

        [Column("dtinv_level")]
        [DisplayName("LEVEL")]
        public int DTLevel { get; set; }

        [ForeignKey("DTLevel")]
        [DisplayName("LEVEL")]
        public Level Level { get; set; }

        [Column("dtinv_brand")]
        [DisplayName("BRAND")]
        public int DTBrand { get; set; }


        [ForeignKey("DTBrand")]
        [DisplayName("BRAND")]
        public Brand Brand { get; set; }

        [Column("dtinv_model")]
        [DisplayName("MODEL")]
        public int DTModel { get; set; }

        [ForeignKey("DTModel")]
        [DisplayName("MODEL")]
        public Model Model { get; set; }

        [Column("dtinv_mobo")]
        [DisplayName("MAIN BOARD")]
        public int DTMBoard { get; set; }

        [ForeignKey("DTMBoard")]
        [DisplayName("MAIN BOARD")]
        public MainBoard MainBoard { get; set; }

        [Column("dtinv_cpu")]
        [DisplayName("CPU")]
        public int DTcpu { get; set; }

        [ForeignKey("DTcpu")]
        [DisplayName("CPU")]
        public CPU CPU { get; set; }

        [Column("dtinv_graphics")]
        [DisplayName("GRAPHICS")]
        public int DTgraphics { get; set; }

        [ForeignKey("DTgraphics")]
        [DisplayName("GRAPHICS CARD")]
        public GraphicsCard GraphicsCard { get; set; }

        [Column("dtinv_harddisk")]
        [DisplayName("HARD DISK")]
        public int DTHardisk { get; set; }

        [ForeignKey("DTHardisk")]
        [DisplayName("HARD DISK")]
        public HardDisk HardDisk { get; set; }

        [Column("dtinv_memory")]
        [DisplayName("MEMORY")]
        public int DTMemory { get; set; }

        [ForeignKey("DTMemory")]
        [DisplayName("MEMORY")]
        public Memory Memory { get; set; }

        [Column("dtinv_os")]
        [DisplayName("OS")]
        public int DTOS { get; set; }

        [ForeignKey("DTOS")]
        [DisplayName("OS")]
        public OS OS { get; set; }

        [Column("dtinv_qty")]
        [DisplayName("QTY")]
        public int Quantity { get; set; }

        [Column("dtinv_alloc")]
        [DisplayName("ALLOCATED NO")]
        public int? AllocatedNo { get; set; }

        [Column("dtinv_status", TypeName = "VARCHAR(2)")]
        public string DTStatus { get; set; }

        [ForeignKey("DTStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [Column("dtinv_created", TypeName = "VARCHAR(15)")]
        public string DTCreated { get; set; }

        [ForeignKey("DTCreated")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [Column("dtinv_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("dtinv_updated", TypeName = "VARCHAR(15)")]
        public string? DTUpdated { get; set; }

        [ForeignKey("DTUpdated")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }

        [Column("dtinv_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }



    }
}
