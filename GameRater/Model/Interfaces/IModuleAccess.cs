using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    interface IModuleAccess
    {
        RatingModule GetParentModule();
        void SetParentModule(RatingModule parentModule);
    }
}
