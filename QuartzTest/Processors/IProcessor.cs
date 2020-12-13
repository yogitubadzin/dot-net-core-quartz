using System.Threading.Tasks;

namespace QuartzTest.Processors
{
    public interface IProcessor
    {
        Task Run();
    }
}