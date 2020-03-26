/*
   This class utilises a queue of data values (position or orientation) based on a given sample space (user-defined or default)
   to calculate a moving average in real-time for data smoothing.

   Written by Paul Hammond, 16/11/2018
*/

using System;
using System.Collections;
using UnityEngine;

class DataSmooth
{
    // Private Attributes
    private const int DEFAULT_N = 1;    // 1 = No Smoothing
    private int _Sample_N;
    private Queue _SampleSpace;
    // Position Related:
    private int _SmoothedAverage;
    private int _CurrentTotal;
    // Orientation Related:
    private float _SmoothedOrientationAverage;
    private float _CurrentTotalOrientation;

    // Custom Constructors
    public DataSmooth() : this(DEFAULT_N)
    {
    }

    public DataSmooth(int n)
    {
        // Protect against invalid inputs
        if (n <= 0)
        {
            _Sample_N = DEFAULT_N;
            Debug.Log(string.Format("You entered an invalid SMOOTHING value: must be 1 or greater\n\nUsing default value of {0}", DEFAULT_N));
        }
        else
        {
            _Sample_N = n;
        }

        // Initialise
        _SampleSpace = new Queue();
        _CurrentTotal = 0;
        _CurrentTotalOrientation = 0;

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
    /// Takes in a orientation data value (x, y, z, w), adds to queue, calculates a smoothing average across
    /// a designated sample space and returns the smoothed value
    /// </summary>
    /// <param name="dataValue"></param>
    /// <param name="value"></param>
    public float SmoothOrientationData(float dataValue)
    {
        float entryNumber;    // Number entering queue
        float exitNumber;     // Number exiting queue

        entryNumber = dataValue;

        // If queue is not full, add number to current total
        if (_SampleSpace.Count < _Sample_N)
        {
            _SampleSpace.Enqueue(entryNumber);
            _CurrentTotalOrientation += entryNumber;
        }
        // If queue is full, calculate current average of data values in queue, dequeue FIFO number, enqueue next number and 
        // re-adjust current total
        else if (_SampleSpace.Count == _Sample_N)
        {
            _SmoothedOrientationAverage = _CurrentTotalOrientation / _Sample_N;
            exitNumber = (float)Convert.ToDouble(_SampleSpace.Dequeue());
            _CurrentTotalOrientation -= exitNumber;
            _SampleSpace.Enqueue(entryNumber);
            _CurrentTotalOrientation += entryNumber;
        }

        return _SmoothedOrientationAverage;
    }
}