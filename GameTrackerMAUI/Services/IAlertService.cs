using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public interface IAlertService
    {
        Task DisplayAlertAsync(string title, string message, string confirm = "OK");
        Task<bool> DisplayConfirmationAsync(string title, string message, string yesOption = "Yes", string noOption = "No");
        Task<string> DisplayInputAsync(string title, string message, string initialValue = "");
    }
}
