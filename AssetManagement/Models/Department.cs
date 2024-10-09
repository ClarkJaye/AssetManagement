using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    public class Department
    {
        [Key]
        [Column("dept_code")]
        [DisplayName("DEPARTMENT CODE")]
        public int Dept_code { get; set; }

        [StringLength(50)]
        [DisplayName("DEPARTMENT NAME")]
        [Column("dept_name")]
        public string Dept_name { get; set; }

        [StringLength(2)]
        [DisplayName("STATUS")]
        [Column("dept_status")]

        public string Dept_status { get; set; }
        [ForeignKey("Dept_status")]
        public Status Status { get; set; }

        [Column("Department_created")]
        [DisplayName("CREATED BY")]
        public string DCreatedby { get; set; }
        [ForeignKey("DCreatedby")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [Column("Department_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("Department_updated")]
        [DisplayName("UPDATED BY")]
        public string? DUpdateby { get; set; }
        [ForeignKey("DUpdateby ")]
        [DisplayName("UPDATED BY")]
        public User? Updatedby { get; set; }

        [Column("Department_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }

    }
}
