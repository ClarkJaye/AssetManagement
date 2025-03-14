﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AssetManagement.Models
{
    [Table("tbl_ictams_upspm")]
    public class UpsPM
    {
        [Key]
        [Column("pm_no")]
        [DisplayName("PM NO")]
        public int PMNO { get; set; }

        [Column("ups_store")]
        [DisplayName("UPS STORE")]
        public string UpsPMStore { get; set; }
        [ForeignKey("UpsPMStore")]
        [DisplayName("UPS STORE")]
        [ValidateNever]
        public Ups UpsStore { get; set; }

        [Column("ups_code")]
        [DisplayName("UPS CODE")]
        public string UpsPMCode { get; set; }
        [ForeignKey("UpsPMCode")]
        [DisplayName("UPS CODE")]
        [ValidateNever]
        public Ups UpsCode { get; set; }

        [Column("pm_createdby")]
        [DisplayName("CREATED BY")]
        public string PMCreatedBy { get; set; }
        [ForeignKey("PMCreatedBy")]
        [DisplayName("CREATED BY")]
        [ValidateNever]
        public User User { get; set; }

        [Column("pm_createddt")]
        [DisplayName("CREATED AT")]
        public DateTime PMCreatedAt { get; set; }

        [Column("pm_date")]
        [DisplayName("PM DATE")]
        public DateTime PMDate { get; set; }

        [Column("pm_remarks", TypeName = "VARCHAR(100)")]
        [DisplayName("REMARKS")]
        public string? UpsPMRemarks { get; set; }

    }
}
