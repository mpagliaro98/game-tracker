﻿using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface IModuleStatus
    {
        StatusExtensionModule StatusExtension { get; }
    }
}
