using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{
    public class Parameter
    {
        [Key]
        [Column("parm_code")]
        public string parm_code { get; set; }

        [Column("parm_value")]
        public int parm_value { get; set; }

        [Column("parm_string")]
        public string parm_string { get; set; }
    }
}
