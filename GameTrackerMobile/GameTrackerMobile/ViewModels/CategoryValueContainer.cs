using System;
using System.Collections.Generic;
using System.Text;

namespace GameTrackerMobile.ViewModels
{
    public class CategoryValueContainer
    {
        private string categoryName = "";
        private double categoryValue = 0;

        public string CategoryName
        {
            get => categoryName;
            set => categoryName = value;
        }

        public double CategoryValue
        {
            get => categoryValue;
            set => categoryValue = value;
        }
    }
}
