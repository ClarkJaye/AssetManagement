using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssetManagement.Models
{
    public class DeviceType
    {
        [Key]
        [Column("devtype_id")]
        [DisplayName("ID")]
        public int DevtypeID { get; set; }



        [Column("devtype_description", TypeName = "VARCHAR(20)")]
        [DisplayName("DESCRIPTION")]
        public string? DevtypeDescription { get; set; }



        [Column("devtype_status", TypeName = "VARCHAR(2)")]
        [DisplayName("STATUS")]
        public string? DevtypeStatus { get; set; }


        [Column("devtype_createdby", TypeName = "VARCHAR(15)")]
        [DisplayName("CREATED BY")]
        public string? DevtypeCreatedby { get; set; }



        [Column("devtype_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime? DateCreated { get; set; }



        [Column("devtype_updatedby", TypeName = "VARCHAR(15)")]
        [DisplayName("UPDATED BY")]
        public string? DevtypeUpdateby { get; set; }



        [Column("devtype_updateddt")]
        [DisplayName("UPDATED AT")]
        public DateTime? DateUpdated { get; set; }


        [ForeignKey("DevtypeStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }



        [ForeignKey("DevtypeCreatedby")]
        [DisplayName("CREATED BY")]
        public User Createdby { get; set; }



        [ForeignKey("DevtypeUpdateby")]
        [DisplayName("UPDTED BY")]
        public User Updatedby { get; set; }
    }
}
