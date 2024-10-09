using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class Store
    {
        [Key]
        [Column("store_code")]
        [DisplayName("STORE CODE")]
        public string Store_code { get; set; }
        [StringLength(100)]
        [Column("store_name")]
        [DisplayName("STORE NAME")]
        public string StoreName { get; set; }


        [StringLength(2)]
        [Column("store_status")]
        public string StoreStatus { get; set; }
        [ForeignKey("StoreStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }


        [Column("created_by")]
        [DisplayName("CREATED BY")]
        public string SCreatedby { get; set; }
        [ForeignKey("SCreatedby ")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }

        [Column("Store_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime DateCreated { get; set; }

        [Column("updated_by")]
        [DisplayName("UPDATED BY")]
        public string? SUpdateby { get; set; }
        [ForeignKey("SUpdateby")]
        [DisplayName("UPDATED BY")]
        public User? Updatedby { get; set; }

        [Column("Store_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }

    }
}
