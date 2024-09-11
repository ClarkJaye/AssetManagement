using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_brand")]
    public class Brand
    {
        [Key]
        [Column("brand_id")]
        [DisplayName("ID")]
        public int BrandId { get; set; }

        [Column("brand_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(15)]
        public string BrandDescription { get; set; }

        [Column("brand_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string BrandStatus { get; set; }

        [Column("brand_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string BrandCreatedBy { get; set; }

        [Column("brand_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime BrandCreatedDate { get; set; }

        [Column("brand_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? BrandUpdatedBy { get; set; }

        [Column("brand_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? BrandUpdatedDate { get; set; }


    }
}
