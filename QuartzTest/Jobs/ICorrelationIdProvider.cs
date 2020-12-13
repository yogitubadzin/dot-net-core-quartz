using System;

namespace QuartzTest.Jobs
{
    public interface ICorrelationIdGenerator
    {
        Guid Generate();
    }
}