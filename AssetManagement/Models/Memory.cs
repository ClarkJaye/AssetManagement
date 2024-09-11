using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_memory")]
    public class Memory
    {
        [Key]
        [Column("memory_id")]
        [DisplayName("ID")]
        public int MemoryId { get; set; }

        [Column("memory_description")]
        [DisplayName("DESCRIPTION")]
        [StringLength(20)]
        public string MemoryDescription { get; set; }


        [Column("memory_capacity")]
        [DisplayName("CAPACITY")]
        public int MemoryCapacity { get; set; }

        [Column("memory_status")]
        [DisplayName("STATUS")]
        [StringLength(2)]
        public string MemoryStatus { get; set; }

        [Column("memory_createdby")]
        [DisplayName("CREATED BY")]
        [StringLength(15)]
        public string MemoryCreatedBy { get; set; }

        [Column("memory_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime MemoryCreatedDate { get; set; }

        [Column("memory_updatedby")]
        [DisplayName("UPDATE BY")]
        [StringLength(15)]
        public string? MemoryUpdatedBy { get; set; }

        [Column("memory_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? MemoryUpdatedDate { get; set; }


        [ForeignKey("MemoryCapacity")]
        [DisplayName("CAPACITY")]
        public virtual Capacity? Capacity { get; set; }
    }
}
