using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class Capacity
    {
        [Key, Column("capacity_id")]
        [DisplayName("ID")]
        public int CapacityId { get; set; }

        [Column("capacity_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(6)]
        public string CapacityDescription { get; set; }

        [Column("capacity_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string? CapacityStatus { get; set; }

        [Column("capacity_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string? CapacityCreatedBy { get; set; }

        [Column("capacity_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime? CapacityCreatedDate { get; set; }

        [Column("capacity_updatedby")]
        [DisplayName("UPDATED BY")]
        [StringLength(15)]
        public string? CapacityUpdatedBy { get; set; }

        [Column("capacity_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? CapacityUpdatedDate { get; set; }

    }
}
