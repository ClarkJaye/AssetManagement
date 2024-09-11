using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    [Table("tbl_user_stores")]
    public class UserStore
    {
        [Key]
        [Column("user_code", TypeName = "VARCHAR(15)")]

        [MaxLength(15)]
        public string UserCode { get; set; }
        [Key]
        [Column("store_code", TypeName = "VARCHAR(5)")]
        [MaxLength(5)]
        public string StoreCode { get; set; }

        [ForeignKey("UserCode")]
        [DisplayName("USER")]

        public User User { get; set; }

        [ForeignKey("StoreCode")]
        [DisplayName("STORE")]
        public Store Store { get; set; }

    }
}
