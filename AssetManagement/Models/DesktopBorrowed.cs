    using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_dtborrowed")]
    public class DesktopBorrowed
    {
        [Key]
        [Column("dtborrowed_id")]
        [DisplayName("BORROWED ID")]
        public string BorrowedID { get; set; }

        [Column("dtborrowed_unitid")]
        [DisplayName("UNIT ID")]
        public string UnitID { get; set; }

        [Column("dtborrowed_unittag")]
        [DisplayName("UNIT TAG")]
        public string UnitTag { get; set; }

        [ForeignKey("UnitTag")]
        [DisplayName("UNIT TAG")]
        public DesktopInventoryDetail InventoryDetails { get; set; }

        [Column("dtborrower_code")]
        [DisplayName("OWNER")]
        public string OwnerID { get; set; }

        [Column("dtborrowed_status")]
        [DisplayName("STATUS")]
        public string StatusID { get; set; }

        [Column("dtborrowed_date")]
        [DisplayName("DATE BORROWED")]
        public DateTime DateBorrow { get; set; }

        [Column("dtborrowed_remarks")]
        [DisplayName("REMARKS")]
        public string? Remark { get; set; }

        [Column("dtborrowed_createdby")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]
        public string CreatedBy { get; set; }


        [Column("dtborrowed_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }


        [Column("dtborrowed_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? RTUpdated { get; set; }

        [Column("dtborrowed_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }



        [ForeignKey("UnitID")]
        [DisplayName("INVENTORY")]
        public DesktopInventory DesktopInventory { get; set; }

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
