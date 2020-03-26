/*
   This class utilises a queue of data values (position or orientation) based on a given sample space (user-defined or default)
   to calculate a moving average in real-time for data smoothing.

   Written by Paul Hammond, 16/11/2018
*/

using System;
using System.Collections;

class MovingAverage
{
    // Private Attributes
    private const int DEFAULT_N = 10;
    private int _Sample_N;
    private int _CurrentTotal;
    private Queue _SampleSpace;
    private int _SmoothedAverage;

    // Custom Constructors
    public MovingAverage() : this(DEFAULT_N)
    {
    }

    public MovingAverage(int n)
    {
        _Sample_N = n;
        _SampleSpace = new Queue();
        _CurrentTotal = 0;
    }

    /// <summary>
    /// Takes in a position data value (x, y, or z), adds to queue, calculates a smoothing average across
    /// a designated sample space and returns the smoothed value
    /// </summary>
    /// <param name="dataValue"></param>
    /// <param name="value"></param>
    public int SmoothPositionData(int dataValue)
    {
        int entryNumber;    // Number entering queue
        int exitNumber;     // Number exiting queue

        entryNumber = dataValue;

        // If queue is not full, add number to current total
        if (_SampleSpace.Count < _Sample_N)
        {
            _SampleSpace.Enqueue(entryNumber);
            _CurrentTotal += entryNumber;
        }
        // If queue is full, calculate current average of data values in queue, dequeue FIFO number, enqueue next number and 
        // adjust current total
        else if (_SampleSpace.Count == _Sample_N)
        {
            _SmoothedAverage = _CurrentTotal / _Sample_N;
            exitNumber = Convert.ToInt32(_SampleSpace.Dequeue());
            _CurrentTotal -= exitNumber;
            _SampleSpace.Enqueue(entryNumber);
            _CurrentTotal += entryNumber;
        }

        return _SmoothedAverage;
    }

    /// <summary>
    /// Takes in a orientation data value (pitch, roll, or yaw), adds to queue, calculates a smoothing average across
    /// a designated sample space and returns the smoothed value
    /// </summary>
    /// <param name="dataValue"></param>
    /// <param name="value"></param>
    public int SmoothOrientationData(float dataValue)
    {
        int entryNumber;    // Number entering queue
        int exitNumber;     // Number exiting queue

        entryNumber = Convert.ToInt32(dataValue);

        // If queue is not full, add number to current total
        if (_SampleSpace.Count < _Sample_N)
        {
            _SampleSpace.Enqueue(entryNumber);
            _CurrentTotal += entryNumber;
        }
        // If queue is full, calculate current average of data values in queue, dequeue FIFO number, enqueue next number and 
        // adjust current total
        else if (_SampleSpace.Count == _Sample_N)
        {
            _SmoothedAverage = _CurrentTotal / _Sample_N;
            exitNumber = Convert.ToInt32(_SampleSpace.Dequeue());
            _CurrentTotal -= exitNumber;
            _SampleSpace.Enqueue(entryNumber);
            _CurrentTotal += entryNumber;
        }

        return _SmoothedAverage;
    }
}