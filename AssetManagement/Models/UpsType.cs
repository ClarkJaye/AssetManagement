using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_upstype")]
    public class UpsType
    {
        [Key]
        [Column("type_id")]
        [DisplayName("TYPE ID")]
        public int type_id { get; set; }

        [Column("type_description", TypeName = "VARCHAR(15)")]
        [DisplayName("DESCRIPTION")]
        public string type_description { get; set; }

        [Column("type_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string type_status { get; set; }

        [Column("type_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string type_createdby { get; set; }

        [Column("type_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime type_createddt { get; set; }

        [Column("type_updatedby", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? type_updatedby { get; set; }

        [Column("type_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? type_updateddt { get; set; }



    }
}
