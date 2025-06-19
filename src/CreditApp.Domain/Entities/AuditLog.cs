using System;
using CreditApp.Domain.Abstractions;

namespace CreditApp.Domain.Entities
{
    public class AuditLog : Entity
    {
        public string EntityName { get; private set; }
        public Guid EntityId { get; private set; }
        public string Action { get; private set; }
        public string Details { get; private set; }
        public string PerformedBy { get; private set; }
        public DateTime Timestamp { get; private set; }

        private AuditLog()
        {
            EntityName = string.Empty;
            Action = string.Empty;
            Details = string.Empty;
            PerformedBy = string.Empty;
        }

        public AuditLog(string entityName, Guid entityId, string action, string details, string performedBy)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Entity name cannot be empty", nameof(entityName));

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty", nameof(action));

            if (string.IsNullOrWhiteSpace(details))
                throw new ArgumentException("Details cannot be empty", nameof(details));

            if (string.IsNullOrWhiteSpace(performedBy))
                throw new ArgumentException("Performed by cannot be empty", nameof(performedBy));

            EntityName = entityName;
            EntityId = entityId;
            Action = action;
            Details = details;
            PerformedBy = performedBy;
            Timestamp = DateTime.UtcNow;
        }
    }
} 