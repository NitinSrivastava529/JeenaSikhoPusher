using System.Collections.Generic;

namespace JeenaSikhoPusher
{
    class CancelReportModel
    {
        public string order_id { get; set; }
        public string externalVisitNo { get; set; }
        public string cancelRemark { get; set; }        
        public List<TestItem> testBookingItems { get; set; }
    }
    public class TestItem
    {
        public string itemId { get; set; }
    }
}
