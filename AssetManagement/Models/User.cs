
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AssetManagement.Models
{
    public class User
    {
        [Key]
        [Column("user_code")]
        [StringLength(15)]
        [DisplayName("USER CODE")]
        public string UserCode { get; set; }

        [Required]
        [Column("user_pass")]
        [StringLength(200)]
        [DisplayName("PASSWORD")]
        public string UserPassword { get; set; }

        [Required]
        [Column("user_adlogin")]
        [StringLength(50)]
        [DisplayName("AD LOGIN")]
        public string UserADLogin { get; set; }

        [Required]
        [Column("user_fullname")]
        [StringLength(150)]
        [DisplayName("FULL NAME")]
        public string UserFullName { get; set; }

        [Required]
        [Column("user_profile")]
        [DisplayName("PROFILE")]
        public int? UserProfile { get; set; }

        [Required]
        [Column("user_status")]
        [StringLength(2)]
        public string UserStatus { get; set; }

        [Required]
        [Column("user_created")]
        [StringLength(15)]
        [DisplayName("CREATED BY")]

        public string UserCreated { get; set; }

     
        [Column("user_dtcreated")]
        [DisplayName("CREATED AT")]
        public DateTime UserDateCreated { get; set; }

        [Column("user_updated")]
        [StringLength(15)]
        [DisplayName("UPDATED BY")]
        public string? UserUpdated { get; set; }

        [Column("user_dtupdated")]
        [DisplayName("UPDATED AT")]
        public DateTime? UserDateUpdated { get; set; }

        [ForeignKey("UserStatus")]
        [DisplayName("STATUS")]
        public virtual Status Status { get; set; }

        [ForeignKey("UserProfile")]
        [DisplayName("PROFILE")]
        public virtual Profile Profile { get; set; }
    }

}