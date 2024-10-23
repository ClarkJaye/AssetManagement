

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AssetManagement.Models
{
    [Table("tbl_ictams_ltborrowed")]
    public class LaptopBorrowed
    {
        [Key]
        [Column("ltborrowed_id")]
        [DisplayName("BORROWED ID")]
        public string BorrowedID { get; set; }

        [Column("ltborrowed_unitid")]
        [DisplayName("INVENTORY")]
        public string UnitID { get; set; }

        [Column("serial_number")]
        [DisplayName("SERIAL NUMBER")]
        public string SerialNumber { get; set; }

        [ForeignKey("UnitID, SerialNumber")]
        [DisplayName("SERIAL NUMBER")]
        public InventoryDetails LaptopInventoryDetails { get; set; }

        [Column("computer_name", TypeName = "VARCHAR(50)")]
        [DisplayName("COMPUTER NAME")]
        public string? ComputerName { get; set; }

        [Column("ltborrower_code")]
        [DisplayName("BORROWER NAME")]
        public string OwnerID { get; set; }

        [Column("dept_code")]
        [DisplayName("BORROWER DEPARTMENT")]
        public int Deptment_Code{ get; set; }
        [ForeignKey("Deptment_Code")]
        public virtual Department Department { get; set; }

        [Column("ltborrowed_status")]
        [DisplayName("STATUS")]
        public string StatusID { get; set; }

        [Column("ltborrowed_date")]
        [DisplayName("DATE Of BORROWED")]
        public DateTime DateBorrow { get; set; }

        [Column("return_date")]
        [DisplayName("RETURN DATE")]
        public DateTime? Return_date { get; set; }

        [Column("expected_return_date")]
        [DisplayName("EXPECTED RETURN DATE")]
        public DateTime Expected_return { get; set; }

        [Column("ltborrowed_remarks")]
        [DisplayName("REMARKS")]
        public string? Remark { get; set; }

        [Column("ltborrowed_createdby")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]
        public string CreatedBy { get; set; }


        [Column("ltborrowed_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }


        [Column("ltborrowed_updatedby")]
        [DisplayName("UPDATED BY")]
        public string? RTUpdated { get; set; }

        [Column("ltborrowed_updateddt")]
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

        [ForeignKey("RTUpdated")]
        [DisplayName("UPDATED BY")]
        public virtual User UserUpdated { get; set; }
    }
}
