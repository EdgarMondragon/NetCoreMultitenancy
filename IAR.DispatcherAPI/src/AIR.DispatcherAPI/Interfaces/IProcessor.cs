using System.Threading.Tasks;
using Entities;

namespace IAR.DispatcherAPI.Interfaces
{
    public interface IProcessor
    {
        Task ProcessMessage(DispatchMessage dispatchMessage);
    }
}
