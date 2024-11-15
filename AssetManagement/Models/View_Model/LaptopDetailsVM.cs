namespace AssetManagement.Models.View_Model
{
    public class LaptopDetailsVM
    {
        public string? LaptopCode{ get; set; }
        public string? SerialNo{ get; set; }
        public string? PoNo{ get; set; }
        public string? Price{ get; set; }
        public string? Vendor{ get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? DelpoyedDate { get; set; }
        public string? Owner { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
