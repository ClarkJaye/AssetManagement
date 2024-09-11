using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class Category
    {
        [Key]
        [Required]
        [Column("category_id")]

        public int category_id { get; set; }

        [Required]
        [StringLength(20)]
        [Column("category_name")]
        public string category_name { get; set; }

        [Required]
        [StringLength(2)]
        [Column("category_status")]
        public string category_status { get; set; }
    }
}
