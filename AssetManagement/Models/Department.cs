using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    public class Department
    {
        [Key]
        [Column("dept_code")]
        public int Dept_code { get; set; }

        [StringLength(50)]
        [Column("dept_name")]
      

        public string Dept_name { get; set; }

        [StringLength(2)]
        [Column("dept_status")]

        public string Dept_status { get; set; }

        [ForeignKey("Dept_status")]
        public Status Status { get; set; }
    }
}
