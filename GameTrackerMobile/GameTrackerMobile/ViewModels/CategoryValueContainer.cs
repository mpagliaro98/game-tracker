using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameTrackerMobile.ViewModels
{
    public class CategoryValueContainer : INotifyPropertyChanged
    {
        private string categoryName = "";
        private double categoryValue = 0;

        public string CategoryName
        {
            get => categoryName;
            set => SetProperty(ref categoryName, value);
        }

        public double CategoryValue
        {
            get => categoryValue;
            set => SetProperty(ref categoryValue, value);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
