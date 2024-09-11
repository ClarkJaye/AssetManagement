using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
	public class Status
	{
		[Key]
		[Required]
		[StringLength(2)]
		[Column("status_code")]

		public string status_code { get; set; }

		[Required]
		[StringLength(20)]
        [DisplayName("STATUS")]
        [Column("status_name")]
        public string status_name { get; set; }
	}
}
