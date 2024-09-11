using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_owners")]
    public class Owner
    {
        [Key]
        [Required]
        [Column("owner_code", TypeName = "VARCHAR(15)")]
        [StringLength(15)]
        [DisplayName("OWNER CODE")]
        public string OwnerCode { get; set; }

        [Required]
        [Column("owner_fullname", TypeName = "VARCHAR(150)")]
        [DisplayName("FULL NAME")]
        public string OwnerFullName { get; set; }

        [Required]
        [Column("owner_dept")]
        [DisplayName("DEPARTMENT")]
        public int OwnerDept { get; set; }

        [Required]
        [Column("owner_location")]
        [DisplayName("LOCATION")]
        public int OwnerLocation { get; set; }


        [Column("owner_office", TypeName = "VARCHAR(5)")]
        [StringLength (5)]
        [DisplayName("OFFICE")]
        public string? OwnerOffice { get; set; }

        [Column("owner_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string OwnerStatus { get; set; }

        [Column("owner_created", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string OwnerCreated { get; set; }

        [Column("owner_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("owner_updated", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? Owner_updated { get; set; }

        [Column("owner_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }



        [ForeignKey("OwnerLocation")]
        public Location Location { get; set; }

        [ForeignKey("OwnerCreated")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [ForeignKey("Owner_updated")]
        [DisplayName("UPDATED BY")]
        public User Updatedby { get; set; }

        [ForeignKey("OwnerStatus")]
        [DisplayName("STATUS")]
        public virtual Status Status { get; set; }

        [ForeignKey("OwnerDept")]
        [DisplayName("DEPARTMENT")]
        public Department Department { get; set; }

        [ForeignKey("OwnerOffice")]
        [DisplayName("Office")]
        public Store Store { get; set; }

    }
}
