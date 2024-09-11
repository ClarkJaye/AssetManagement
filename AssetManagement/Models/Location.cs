using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace AssetManagement.Models
{
    [Table("tbl_ictams_location")]
    public class Location
    {
        [Key]
        [Column("location_code")]
        public int LocationCode { get; set; }

        [Column("location_description", TypeName = "VARCHAR(50)")]
        public string Description { get; set; }

        [Column("location_status", TypeName = "VARCHAR(2)")]
        public string LocationStatus { get; set; }

        [ForeignKey("LocationStatus")]
        public Status status { get; set; }
    }
}
