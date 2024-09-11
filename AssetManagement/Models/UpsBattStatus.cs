using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_upsbattstatus")]
    public class UpsBattStatus
    {
        [Key]
        [Column("status_id")]
        [DisplayName("STATUS ID")]
        public int status_id { get; set; }

        [Column("status_description", TypeName = "VARCHAR(15)")]
        [DisplayName("STATUS DESCRIPTION")]
        public string status_description { get; set; }

        [Column("status_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string status_status { get; set; }

        [Column("status_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string status_createdby { get; set; }

        [Column("status_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime status_createddt { get; set; }

        [Column("status_updatedby", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? status_updatedby { get; set; }

        [Column("status_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? status_updateddt { get; set; }

    }
}
