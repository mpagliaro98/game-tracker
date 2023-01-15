using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ILoadSaveLocation
    {
        // load file - take method as a parameter and return FileConnector
        //      method defines which FileConnector is returned - json returns JSONFileConnector, db returns DBFileConnector
        //          JSONFileConnector - could maintain JSON text from inside the file
        //              maintain text as a list of savable representations
        //              when you save a single object, only replace that one object in the list, then save the list back to the file
        //          DBFileConnector - could maintain a database connection
        // save function - takes file connector

        // id could be file name, aws bucket name, database table, etc.
        //void EstablishConnection(ILoadSaveMethod method, string id);
    }
}
