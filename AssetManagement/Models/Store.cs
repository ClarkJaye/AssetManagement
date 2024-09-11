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

    }
}
