using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{
	public class Module
	{
        [Key]
        [Column("module_id")]
        //[BindNever]
        public int ModuleId { get; set; }

		[StringLength(50)]
        [Column("module_title")]
        [DisplayName("MODULE TITLE")]

        public string ModuleTitle { get; set; }

        [Column("module_category")]
        [DisplayName("MODULE CATEGORY")]


        public int ModuleCategory { get; set; }

        [StringLength(2)]
        [Column("module_status")]
        [DisplayName("MODULE STATUS")]



        public string ModuleStatus { get; set; }

		[StringLength(15)]
        [Column("module_created")]
        
        public string ModuleCreated { get; set; }

        [Column("module_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime ModuleDtCreated { get; set; }

		[StringLength(15)]
        [Column("module_updated")]
        
        public string? ModuleUpdated { get; set; }
        [Column("module_dtupdated")]
        [DisplayName("UPDATED AT")]


        public DateTime? ModuleDtUpdated { get; set; }


        [ForeignKey("ModuleCategory")]
        [DisplayName("CATEGORY")]
        public Category Category { get; set; }


        [ForeignKey("ModuleStatus")]
        [DisplayName("STATUS")]
        public Status Status { get; set; }

		[ForeignKey("ModuleCreated")]
        [DisplayName("CREATED BY")]
        public User CreatedBy { get; set; }

        [ForeignKey("ModuleUpdated")]
        [DisplayName("UPDATED BY")]
        public User? UpdatedBy { get; set; }
	}
}