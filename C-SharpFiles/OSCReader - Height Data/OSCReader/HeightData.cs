using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pozyx_HeightData
{
    class HeightData
    {
        // Private Attributes
        //private const int _TagHeight = 570;         // mm (Tag mounted to 9V battery using Phone Charger Connection UWB chip up)
        private const int _TagHeight = 755;         // mm (Tag mounted to 9V battery using Phone Charger Connection UWB chip up)
        private int _errorTotal;
        private int _sampleTotal;

        // Methods
        public int GetError(int num)
        {
            int error = num - (_TagHeight);

            _sampleTotal += 1;
            _errorTotal += error;

            return error;
        }

        public int GetErrorAverage()
        {
            return _errorTotal / _sampleTotal;
        }

        public int GetActualHeight()
        {
            return _TagHeight;
        }
    }
}