using System;

namespace QuartzTest.Jobs
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}