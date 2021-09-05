using MyApiTemplate.Service.Contract;
using System;

namespace MyApiTemplate.Service.Implementation
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}