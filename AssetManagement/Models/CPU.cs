using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_cpu")]
    public class CPU
    {
        [Key]
        [Column("cpu_id")]
        [DisplayName("ID")]
        public int CPUId { get; set; }

        [Column("cpu_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(20)]
        public string CPUDescription { get; set; }

        [Column("cpu_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string CPUStatus { get; set; }

        [Column("cpu_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string CPUCreatedBy { get; set; }

        [Column("cpu_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime CPUCreatedDate { get; set; }

        [Column("cpu_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? CPUUpdatedBy { get; set; }

        [Column("cpu_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? CPUUpdatedDate { get; set; }
    }
}
