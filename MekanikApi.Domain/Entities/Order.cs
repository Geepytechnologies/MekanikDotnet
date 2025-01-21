using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public Guid ChatConversationId { get; set; }

        // Navigation property to OrderItems
        public ICollection<OrderItem> OrderItems { get; set; } = [];

        public ICollection<Transaction> Transactions { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
