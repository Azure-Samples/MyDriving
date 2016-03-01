using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Interfaces
{
    public interface IHubIOT
    {
        Task Initialize(string connectionStr);

        Task<bool> SendEvent(string blob);
    }
}
