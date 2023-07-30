using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public interface IToastService
    {
        Task ShowToastAsync(string message);
    }
}
