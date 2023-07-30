using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;

namespace GameTrackerMAUI.Services
{
    public class ToastServiceToolkit : IToastService
    {
        public async Task ShowToastAsync(string message)
        {
            var toast = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long);
            await toast.Show();
        }
    }
}
