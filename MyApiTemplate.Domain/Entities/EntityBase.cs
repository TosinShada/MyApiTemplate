using System;

namespace MyApiTemplate.Domain.Entities
{
    public class EntityBase
    {
        public long RowCreatedById { get; set; }
        public long RowModifiedById { get; set; }
        public DateTime RowCreatedDateTimeUtc { get; set; }
        public DateTime RowModifiedDateTimeUtc { get; set; }
    }
}
