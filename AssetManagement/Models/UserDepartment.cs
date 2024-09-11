using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_userdept")]
    public class UserDepartment
    {
        [Key]
        [Column("user_code", TypeName = "VARCHAR(15)")]
        [MaxLength(15)]
        public string UserCode { get; set; }
        [Key]
        [Column("dept_code")]
        public int DeptCode { get; set; }

        [ForeignKey("UserCode")]
        [DisplayName("USER")]
        public User User { get; set; }

        [ForeignKey("DeptCode")]
        [DisplayName("DEPARTMENT")]
        public Department Department { get; set; }
    }
}
