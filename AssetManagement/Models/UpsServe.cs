using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_upsserve")]
    public class UpsServe
    {
        [Key]
        [Column("unit_no")]
        [DisplayName("UNIT NO")]
        public int UnitNo { get; set; }

        [Column("ups_store", TypeName = "VARCHAR(5)")]
        [DisplayName("UPS STORE")]
        public string UpsServeStore { get; set; }

        [ForeignKey("UpsServeStore")]
        [DisplayName("UPS STORE")]
        public Ups UpsStore { get; set; }

        [Column("ups_code", TypeName = "VARCHAR(10)")]
        [DisplayName("UPS CODE")]
        public string UpsServeCode { get; set; }

        [ForeignKey("UpsServeCode")]
        [DisplayName("UPS CODE")]
        public Ups UpsCode { get; set; }

        [Column("unit_serve", TypeName = "VARCHAR(50)")]
        [DisplayName("UNIT SERVE")]
        public string UnitServe { get; set; }

        [Column("unit_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string UnitCreatedBy { get; set; }

        [Column("unit_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime UnitCreatedAt { get; set; }

        [ForeignKey("UnitCreatedBy ")]
        [DisplayName("CREATED BY")]
        public User User { get; set; }
    }
}
