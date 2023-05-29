using System;
using System.Linq;
using UnityEngine;

public class MovingAverage
{
    private readonly float[] _values;
    private int _index = 0;
    private int _maxIndex = -1;
    private float? _average;
    private float? _maxValue;
    private float? _minValue;

    public float Average => _average ??= RecalculateAverage();
    public float MaxValue => _maxValue ??= RecalculateMaxValue();
    public float MinValue => _minValue ??= RecalculateMinValue();
    public bool IsFull => _maxIndex == _values.Length - 1;

    public MovingAverage(int size)
    {
        _values = new float[size];
        _average = 0;
        _minValue = 0;
        _maxValue = 0;
    }

    public void Push(float value)
    {
        float oldValue = _values[_index];
        _values[_index] = value;

        if (_minValue.HasValue && value < _minValue.Value) _minValue = value;
        if (_maxValue.HasValue && value < _maxValue.Value) _maxValue = value;

        if (oldValue == _minValue) _minValue = null;
        if (oldValue == _maxValue) _maxValue = null;
        if (IsFull)
        {
            if (_average.HasValue)
            {
                _average -= oldValue / _values.Length;
                _average += value / _values.Length;
            }
            else
            {
                RecalculateFullLoopValues();
            }
        }
        else
        {
            _average = null;
        }
        if (_index > _maxIndex) _maxIndex = _index;
        _index = (_index + 1) % _values.Length;

        // TODO: This is a hack to make sure that the values are correct
        // at the end of each push loop.
        // I need to to understand why it doesn't work properly without doing that.
        if (_index == 0) RecalculateFullLoopValues();
    }

    private float RecalculateAverage()
    {
        RecalculateFullLoopValues();
        return _average.Value;
    }

    private float RecalculateMaxValue()
    {
        RecalculateFullLoopValues();
        return _maxValue.Value;
    }

    private float RecalculateMinValue()
    {
        RecalculateFullLoopValues();
        return _minValue.Value;
    }

    private void RecalculateFullLoopValues()
    {
        int count = _maxIndex + 1;
        if (count == 0)
        {
            _average = 0;
            _minValue = 0;
            _maxValue = 0;
            return;
        }
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        float sum = 0;
        for (int i = 0; i < count; i++)
        {
            float value = _values[i];
            sum += value;
            if (value < minValue)
            {
                minValue = value;
            }
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        _minValue = minValue;
        _maxValue = maxValue;
        _average = sum / count;
    }
}
