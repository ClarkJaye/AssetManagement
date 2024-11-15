using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_laptoptype")]
    public class LaptopType
    {
        [Key]
        [Column("lttype_Id", TypeName = "INT")]
        [DisplayName("CODE")]
        public int? LaptopTypeID { get; set; }

        [Column("lttype_description", TypeName = "VARCHAR(150)")]
        [DisplayName("DESCRIPTION")]
        public string Description { get; set; }


        [Column("lttype_qty")]
        [DisplayName("QTY")]
        public int Quantity { get; set; } = 0;

        [Column("lttype_alloc")]
        [DisplayName("ALLOCATED NO")]
        public int AllocatedNo { get; set; } = 0;


        [Column("lttype_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime? DateCreated { get; set; }

        [Column("lttype_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }

        [Column("lttype_status", TypeName = "VARCHAR(2)")]
        public string? LTStatus { get; set; }
        [ForeignKey("LTStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [Column("lttype_created", TypeName = "VARCHAR(15)")]
        public string LTCreated { get; set; }
        [ForeignKey("LTCreated")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [Column("lttype_updated", TypeName = "VARCHAR(15)")]
        public string? LTUpdated { get; set; }
        [ForeignKey("LTUpdated")]
        [DisplayName("UPDATED BY")]
        public User? Updatedby { get; set; }



    }
}
