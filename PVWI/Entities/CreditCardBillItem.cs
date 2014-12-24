using System;

namespace PVWI.Entities
{
    public class CreditCardBillItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public double ItemValue { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}