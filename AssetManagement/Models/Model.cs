using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_model")]
    public class Model
    {
        [Key] 
        [Column("model_id")]
        [DisplayName("ID")]
        public int ModelId { get; set; }

        [Column("model_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(20)]
        public string ModelDescription { get; set; }

        [Column("model_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string ModelStatus { get; set; }

        [Column("model_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string ModelCreatedBy { get; set; }

        [Column("model_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime ModelCreatedDate { get; set; }

        [Column("model_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? ModelUpdatedBy { get; set; }

        [Column("model_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? ModelUpdatedDate { get; set; }

    }
}
