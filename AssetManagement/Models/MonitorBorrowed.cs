using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_monitorborrowed")]
    public class MonitorBorrowed
    {
        [Key]
        [Column("mborrowed_id")]
        [DisplayName("BORROWED ID")]
        public string BorrowedID { get; set; }

        [Column("mborrowed_unitid")]
        [DisplayName("INVENTORY")]
        public string UnitID { get; set; }

        [Column("mborrowed_serial")]
        [DisplayName("SERIAL NUMBER")]
        public string SerialNumber { get; set; }

        [ForeignKey("UnitID, SerialNumber")]
        [DisplayName("SERIAL NUMBER")]
        public MonitorDetail MonitorDetail { get; set; }

        [Column("mborrower_code")]
        [DisplayName("OWNER")]
        public string OwnerID { get; set; }

        [Column("mborrowed_status")]
        [DisplayName("STATUS")]
        public string StatusID { get; set; }

        [Column("mborrowed_date")]
        [DisplayName("DATE BORROWED")]
        public DateTime DateBorrow { get; set; }

        [Column("mborrowed_remarks")]
        [DisplayName("REMARKS")]
        public string? Remark { get; set; }

        [Column("mborrowed_createdby")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]
        public string CreatedBy { get; set; }

        [Column("mborrowed_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("mborrowed_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? UpdatedBy { get; set; }

        [Column("mborrowed_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }

        [ForeignKey("OwnerID")]
        [DisplayName("OWNER")]
        public virtual Owner Owner { get; set; }

        [ForeignKey("StatusID")]
        [DisplayName("STATUS")]
        public virtual Status Status { get; set; }

        [ForeignKey("CreatedBy")]
        [DisplayName("CREATED BY")]
        public virtual User UserCreated { get; set; }

        [ForeignKey("UpdatedBy")]
        [DisplayName("UPDATED BY")]
        public virtual User UserUpdated { get; set; }
    }
}
