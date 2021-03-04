namespace AirandWebAPI.Core.Domain
{
    public class Merchant : BaseEntity
    {
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerEmail { get; set; }
        public string ProductCategory { get; set; }
        public string DeliveryFrequency { get; set; }
        public string AvgMonthlyDelivery { get; set; }
        public string Status { get; set; }
    }
}
