using System.Threading.Tasks;
using Entities;

namespace IAR.DispatcherAPI.Interfaces
{
    public interface IDispatchConnectorService
    {
        void SendMessage(DispatchMessage message, Task whenPosted);

    }
}
