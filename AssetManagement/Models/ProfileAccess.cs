using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{ 
public class ProfileAccess
{
		[Key, Column("profile_id",Order = 0)]
        public int ProfileId { get; set; }

		[Key, Column("module_id",Order = 1)]

        public int ModuleId { get; set; }

		[StringLength(1)]
        [ Column("open_access")]
        [DisplayName("OPEN ACCES")]
        public string OpenAccess { get; set; }

		[StringLength(15)]
        [Column("user_created")]

        public string UserCreated { get; set; }

        [Column("user_dtcreated")]
       
        public DateTime DateCreated { get; set; }

		[StringLength(15)]
        [Column("user_updated")]
        [DisplayName("UPDATEDBY")]

        public string? UserUpdated { get; set; }
        [Column("user_dtupdated")]
       

        public DateTime? DateUpdated { get; set; }

        [ForeignKey("ProfileId")]
        [DisplayName("PROFILE")]


        public Profile Profile { get; set; }

		[ForeignKey("UserCreated")]
		public User CreatedBy { get; set; }

		[ForeignKey("UserUpdated")]
		public User UpdatedBy { get; set; }

		[ForeignKey("ModuleId")]
        [DisplayName("MODULE")]
        public Module Module { get; set; }

	} }
