using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_os")]
    public class OS
    {
        [Key] 
        [Column("os_id")]
        [DisplayName("ID")]
        public int OSId { get; set; }

        [Column("os_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(20)]
        public string OSDescription { get; set; }

        [Column("os_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string OSStatus { get; set; }

        [Column("os_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string OSCreatedBy { get; set; }

        [Column("os_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime OSCreatedDate { get; set; }

        [Column("os_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? OSUpdatedBy { get; set; }

        [Column("os_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? OSUpdatedDate { get; set; }
    }
}
