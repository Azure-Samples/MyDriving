using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Interfaces
{
    public interface IHubIOT
    {
        void Initialize(string connectionStr);
        Task SendEvents(IEnumerable<String> blobs);

        Task SendEvent(string blob);
    }
}
