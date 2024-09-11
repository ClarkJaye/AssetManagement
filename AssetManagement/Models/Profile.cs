using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{
	public class Profile
	{
        [Column("profile_id")]
        //[BindNever]
        public int ProfileId { get; set; } 

		[Required]
		[StringLength(20)]
        [DisplayName("PROFILE")]
        [Column("profile_name")]

        public string ProfileName { get; set; }

		[Required]
		[StringLength(100)]
        [Column("profile_description")]
        [DisplayName("DESCRIPTION")]
        public string ProfileDescription { get; set; }

		
        [Column("profile_status")]
        [DisplayName("STATUS")]
        public string ProfileStatus { get; set; }

	
        [Column("profile_created")]
        [DisplayName("CREATED BY")]

        public string ProfileCreated { get; set; }

		[Required]
        [Column("profile_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime ProfileDtCreated { get; set; }

		[StringLength(15)]
        [ Column("profile_updated")]
        [DisplayName("UPDATED BY")]

        public string? ProfileUpdated { get; set; }

        [Column("profile_dtupdated")]
        [DisplayName("UPDATED AT")]

        public DateTime? ProfileDtUpdated { get; set; }
        






        [ForeignKey("ProfileStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

        [ForeignKey("ProfileCreated")]
        [DisplayName("CREATED BY")]
        public User CreatedBy { get; set; }

		[ForeignKey("ProfileUpdated")]
        [DisplayName("UPDATED BY")]
        public User? UpdatedBy { get; set; }
	}
}