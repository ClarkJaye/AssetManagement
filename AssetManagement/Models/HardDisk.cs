using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_hardisk")]
    public class HardDisk
    {

        [Key] 
        [Column("hd_id")]
        [DisplayName("ID")]
        public int HDId { get; set; }

        [Column("hd_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(20)]
        public string HDDescription { get; set; }

        [Column("hd_capacity")]
        [DisplayName("CAPACITY")]
        public int HDCapacity { get; set; }

        [Column("hd_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string HDStatus { get; set; }

        [Column("hd_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string HDCreatedBy { get; set; }

        [Column("hd_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime HDCreatedDate { get; set; }

        [Column("hd_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? HDUpdatedBy { get; set; }

        [Column("hd_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? HDUpdatedDate { get; set; }

        [ForeignKey("HDCapacity")]
        [DisplayName("CAPACITY")]
        public virtual Capacity? Capacity { get; set; }
    }
}
