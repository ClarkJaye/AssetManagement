
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AssetManagement.Models
{
    [Table("tbl_ictams_ltareturn")]
    public class ReturnLTA
    {
        [Key]
        [Column("return_id")]
        [DisplayName("RETURN ID")]
        public string ReturnID { get; set; }

        [Column("alloc_id")]
        [DisplayName("ALLOCATION")]
        public string AllocID { get; set; }

        [Column("return_type")]
        [DisplayName("RETURN TYPE")]
        public int RTtype { get; set; }

        [Column("return_status")]
        [DisplayName("STATUS")]
        public string RTStatus { get; set; }

        [Column("return_createdby")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]
        public string CreatedBy { get; set; }


        [Column("return_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }


        [Column("return_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? RTUpdated { get; set; }

        [Column("return_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }






        [ForeignKey("AllocID")]
        [DisplayName("ALLOCATION")]
        public virtual LaptopAllocation LaptopAllocation { get; set; }

        [ForeignKey("RTtype")]
        [DisplayName("RETURN TYPE")]
        public virtual ReturnType ReturnType { get; set; }

        [ForeignKey("RTStatus")]
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
