using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_level")]
    public class Level
    {
        [Key] 
        [Column("level_id")]
        [DisplayName("ID")]
        public int LevelId { get; set; }

        [Column("level_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(10)]
        public string LevelDescription { get; set; }

        [Column("level_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string LevelStatus { get; set; }

        [Column("level_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string LevelCreatedBy { get; set; }

        [Column("level_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime LevelCreatedDate { get; set; }

        [Column("level_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? LevelUpdatedBy { get; set; }

        [Column("level_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? LevelUpdatedDate { get; set; }
    }
}
