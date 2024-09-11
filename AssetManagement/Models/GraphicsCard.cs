
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{
    [Table("tbl_ictams_graphicscard")]
    public class GraphicsCard
    {
        [Key]
        [Column("graphics_id")]
        [DisplayName("ID")]
        public int GraphicsID { get; set; }

        [Column("graphics_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(50)]
        public string GraphicsDescription { get; set; }

        [Column("graphics_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string GraphicsStatus { get; set; }

        [Column("graphics_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string GraphicsCreatedBy { get; set; }

        [Column("graphics_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime GraphicsCreatedDate { get; set; }

        [Column("graphics_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? GraphicsUpdatedBy { get; set; }

        [Column("graphics_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? GraphicsUpdatedDate { get; set; }
    }
}
