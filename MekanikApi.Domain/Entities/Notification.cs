using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

        public DateTime TimeSent { get; set; }

        public Guid UserId { get; set; }
    }
}
