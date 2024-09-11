using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_upscondition")]
    public class UpsCondition
    {
        [Key]
        [Column("condition_id")]
        [DisplayName("CONDITION ID")]
        public int condition_id { get; set; }

        [Column("condition_description", TypeName = "VARCHAR(15)")]
        [DisplayName("DESCRIPTION")]
        public string condition_description { get; set; }

        [Column("condition_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string condition_status { get; set;}

        [Column("condition_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string condition_createdby { get; set; }

        [Column("condition_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime condition_createddt { get; set; }

        [Column("condition_updatedby", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? condition_updatedby { get; set; }

        [Column("condition_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? condition_updateddt { get; set; }

    }
}
