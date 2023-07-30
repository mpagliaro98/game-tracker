using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Services
{
    public class AlertServiceMAUI : IAlertService
    {
        public Task DisplayAlertAsync(string title, string message, string confirm = "OK")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, confirm);
        }

        public Task<bool> DisplayConfirmationAsync(string title, string message, string yesOption = "Yes", string noOption = "No")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, yesOption, noOption);
        }
    }
}
