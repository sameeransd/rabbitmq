using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDomain
{
    [Serializable]
    public class Payment
    {
        public Payment()
        {
        }

        public Payment(int id, decimal amount, string paymentType)
        {
            Id = id;
            Amount = amount;
            PaymentType = paymentType;
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
    }
}
 