using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
   
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }

        public Guid OrderId { get; set; } 
        public Order? Order { get; set; }
    }
}
