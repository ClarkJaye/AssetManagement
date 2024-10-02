using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_upsbattrep")]
    public class UpsBatteryRep
    {
        [Key]
        [Column("battrep_no")]
        [DisplayName("BATTERY NO")]
        public int BatteryRepNo { get; set; }

        [Column("ups_store", TypeName = "VARCHAR(5)")]
        [DisplayName("UPS STORE")]
        public string UpsBattStore { get; set; }
        [ForeignKey("UpsBattStore")]
        [ValidateNever]
        public Ups UpsStore { get; set; }


        [Column("ups_code", TypeName = "VARCHAR(10)")]
        [DisplayName("UPS CODE")]
        public string UpsBattCode { get; set; }
        [ForeignKey("UpsBattCode")]
        [ValidateNever]
        public Ups UpsCode { get; set; }

        [Column("battrep_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string BatteryRepCreatedBy { get; set; }
        [ForeignKey("BatteryRepCreatedBy")]
        [ValidateNever]
        public User User { get; set; }

        [Column("battrep_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime BatteryRepCreatedAt { get; set; }

        [Column("battrep_dt")]
        [DisplayName("BATTERY DATE")]
        public DateTime BatteryRepDate { get; set; }

        [Column("battrep_remarks", TypeName = "VARCHAR(100)")]
        [DisplayName("REMARKS")]
        public string? BatteryRepRemarks { get; set; }
    }
}
