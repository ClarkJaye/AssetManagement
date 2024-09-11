
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_mainboard")]
    public class MainBoard
    {
        [Key]
        [Column("mobo_id")]
        [DisplayName("ID")]
        public int BoardID { get; set; }

        [Column("mobo_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(50)]
        public string BoardDescription { get; set; }

        [Column("mobo_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string BoardStatus { get; set; }


        [Column("mobo_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string BoardCreatedBy { get; set; }

        [Column("mobo_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime BoardCreatedDate { get; set; }

        [Column("mobo_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? BoardUpdatedBy { get; set; }

        [Column("mobo_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? BoardUpdatedDate { get; set; }

    }
}
