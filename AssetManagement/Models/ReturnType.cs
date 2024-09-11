

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_returntype")]
    public class ReturnType
    {
        [Key]
        [Column("rettype_id")]
        [DisplayName("RETURN ID")]
        public int TypeID { get; set; }

        [Column("rettype_description")]
        [StringLength(20)]
        [DisplayName("DESCRIPTION")]
        public string Description { get; set; }

        [Column("rettype_status")]
        [StringLength(2)]
        [DisplayName("STATUS")]
        public string Return_Status { get; set; }

        [Column("rettype_inventory")]
        [StringLength(1)]
        [DisplayName("Y/N")]
        public string Return_Inv { get; set; }

        [Required]
        [Column("rettype_createdby")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]
        public string CreatedBy { get; set; }


        [Column("rettype_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }


        [Column("rettype_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? RTUpdated { get; set; }

        [Column("rettype_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }




        [ForeignKey("Return_Status")]
        [DisplayName("STATUS")]
        public virtual Status Status { get; set; }

        [ForeignKey("CreatedBy")]
        [DisplayName("CREATED BY")]
        public virtual User UserCreated { get; set; }

        [ForeignKey("RTUpdated")]
        [DisplayName("UPDATED BY")]
        public virtual User UserUpdated { get; set; }


    }
}
