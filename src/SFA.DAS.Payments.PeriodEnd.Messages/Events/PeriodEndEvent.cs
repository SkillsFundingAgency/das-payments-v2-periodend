using System;
using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PeriodEnd.Messages.Events
{
    public abstract class PeriodEndEvent : IPeriodEndEvent, IMonitoredMessage
    {
        protected PeriodEndEvent()
        {
            EventId = Guid.NewGuid();
            EventTime = DateTimeOffset.UtcNow;
        }

        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public Guid EventId { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
    }
}