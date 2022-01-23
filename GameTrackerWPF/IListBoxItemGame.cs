using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameTracker.Model;

namespace GameTrackerWPF
{
    public interface IListBoxItemGame
    {
        RatableGame Game { get; }
    }
}
