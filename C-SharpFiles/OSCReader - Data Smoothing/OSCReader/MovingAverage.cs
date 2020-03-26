using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OSCReader
{
    class MovingAverage
    {
        // Private Attributes
        private const int DEFAULT_N = 60;
        private int _Sample_N;
        private int _CurrentTotal;
        private bool _NewLine = false;
        private Queue _SampleSpace;

        // Custom Constructors
        public MovingAverage() : this(DEFAULT_N, false)
        {
        }

        public MovingAverage(int n) : this (n, false)
        {
        }

        public MovingAverage(int n, bool newLine)
        {
            _Sample_N = n;
            _NewLine = newLine;
            _SampleSpace = new Queue();
            _CurrentTotal = 0;
        }

        /// <summary>
        /// Takes in a position data value (x, y, or z) and creates a smoothing average across a designated sample space
        /// </summary>
        /// <param name="dataValue"></param>
        /// <param name="value"></param>
        public void SmoothPositionData(string dataValue, Values value)
        {
            int entryNumber;    // Number entering queue
            int exitNumber;     // Number exiting queue
            int average;

            entryNumber = int.Parse(dataValue);

            if (_SampleSpace.Count < _Sample_N)
            {
                _SampleSpace.Enqueue(entryNumber);
                _CurrentTotal += entryNumber;
            }
            else if (_SampleSpace.Count == _Sample_N)
            {
                average = _CurrentTotal / _Sample_N;
                Console.Write("Current {2} value: {1}\tAverage is: {0}\t\t", average, entryNumber, value.ToString());
                if (_NewLine)
                {
                    Console.WriteLine();
                }
                exitNumber = Convert.ToInt32(_SampleSpace.Dequeue());
                _CurrentTotal -= exitNumber;
                _SampleSpace.Enqueue(entryNumber);
                _CurrentTotal += entryNumber;
            }
        }

        /// <summary>
        /// Takes in a orientation data value (pitch, roll, or yaw) and creates a smoothing average across a designated sample space
        /// </summary>
        /// <param name="dataValue"></param>
        /// <param name="value"></param>
        public void SmoothOrientationData(string dataValue, Values value)
        {
            int entryNumber;    // Number entering queue
            int exitNumber;     // Number exiting queue
            int average;    
            const char F_CHAR = 'f';

            entryNumber = Convert.ToInt32(float.Parse((dataValue.TrimEnd(F_CHAR))));

            if (_SampleSpace.Count < _Sample_N)
            {
                _SampleSpace.Enqueue(entryNumber);
                _CurrentTotal += entryNumber;
            }
            else if (_SampleSpace.Count == _Sample_N)
            {
                average = _CurrentTotal / _Sample_N;
                Console.Write("Current {2} value: {1}\tAverage is: {0}\t\t", average, entryNumber, value.ToString());
                if (_NewLine)
                {
                    Console.WriteLine();
                }
                exitNumber = Convert.ToInt32(_SampleSpace.Dequeue());
                _CurrentTotal -= exitNumber;
                _SampleSpace.Enqueue(entryNumber);
                _CurrentTotal += entryNumber;
            }
        }
    }
}


