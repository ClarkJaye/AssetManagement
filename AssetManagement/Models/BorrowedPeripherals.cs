
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_ltpborrowed")]
    public class BorrowedPeripherals
    {
        [Key]
        [Column("ltpborrowed_id")]
        [DisplayName("BORROWED ID")]
        public string BorrowedID { get; set; }

        [Column("ltpborrowed_unitid")]
        [DisplayName("PERIPHERAL")]
        public string UnitID { get; set; }

        [Column("ltpborrower_code")]
        [DisplayName("OWNER")]
        public string OwnerID { get; set; }

        [Column("ltpborrowed_status")]
        [DisplayName("STATUS")]
        public string StatusID { get; set; }

        [Column("ltpborrowed_date")]
        [DisplayName("DATE BORROWED")]
        public DateTime DateBorrow { get; set; }

        [Column("ltpborrowed_remarks")]
        [DisplayName("REMARKS")]
        public string? Remark { get; set; }

        [Column("ltpborrowed_createdby")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]
        public string CreatedBy { get; set; }


        [Column("ltpborrowed_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }


        [Column("ltpborrowed_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? RTUpdated { get; set; }

        [Column("ltpborrowed_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }





        [ForeignKey("UnitID")]
        [DisplayName("INVENTORY")]
        public virtual LaptopPeripheral LaptopPeripheral { get; set; }

        [ForeignKey("OwnerID")]
        [DisplayName("OWNER")]
        public virtual Owner Owner { get; set; }

        [ForeignKey("StatusID")]
        [DisplayName("STATUS")]
        public virtual Status Status { get; set; }

        [ForeignKey("CreatedBy")]
        [DisplayName("CREATED BY")]
        public virtual User UserCreated { get; set; }

        [ForeignKey("RTUpdated")]
        [DisplayName("UPDATED BY")]
        public virtual User UserUpdated { get; set; }
    }
}
