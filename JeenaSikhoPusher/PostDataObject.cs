using System.Collections.Generic;

namespace JeenaSikhoPusher
{
    public class PostDataObject
    {
        public string uhid { get; set; }
        public string contactNumber { get; set; }
        public string reportUrl { get; set; }
        public string clientId { get; set; }
        public string externalVisitNo { get; set; }
        public string remark { get; set; }
        public List<TestBookingItem> testBookingItems { get; set; }
        public Payment payment { get; set; }
    }
    public class TestBookingItem
    {
        public int itemId { get; set; }
        public int rate { get; set; }
        public int discount { get; set; }
        public int net_rate { get; set; }
    }

    public class Payment
    {
        public List<string> payment_mode { get; set; }
        public List<string> payment_method { get; set; }
        public List<int> amount { get; set; }
    }
}
