using System;
using System.Collections.Generic;
using System.Text;

namespace GnssMeasurementSample.Library
{
    public interface IGnssCompare
    {
        void checkPermissions();
        void init();
    }
}
