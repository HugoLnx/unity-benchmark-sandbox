using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BenchmarkReport
{
    public class BenchmarkRunner
    {
        private object _target;
        private int _iterations;
        private MovingAverage _totalMovingAverage;
        private MovingAverage _minBaseAverage;
        private MovingAverage _gcMovingAverage;
        private int _movingAverageSize;
        private BenchmarkCall[] _calls = new BenchmarkCall[0];
        private BenchmarkCall[] _references = new BenchmarkCall[0];
        private Measurer _callMeasurer;
        private Measurer _runMeasurer;
        private Measurer _prepareMeasurer;
        private Measurer _minTimeMeasurer;
        private float _y;
        private float _x;
        private float _z;
        private readonly Action<int> _prepareForCall;
        private readonly Action _noneCall;
        private static readonly object[] NoArgs = new object[0];

        public BenchmarkRunner(int iterations, int movingAverageSize, Action<int> prepareForCall = null)
        {
            _iterations = iterations;
            _totalMovingAverage = new MovingAverage(movingAverageSize);
            _minBaseAverage = new MovingAverage(movingAverageSize);
            _gcMovingAverage = new MovingAverage(movingAverageSize);
            _movingAverageSize = movingAverageSize;
            _prepareForCall = prepareForCall;
            _noneCall = () => {};
            _prepareMeasurer ??= new Measurer();
            _callMeasurer ??= new Measurer();
            _minTimeMeasurer = new Measurer();
        }

        public BenchmarkRunner AddCall(string name, Action action, bool isReference = false, int? iterations = null, float? timeMultiplier=1f)
        {
            var call = new BenchmarkCall(
                name: name,
                action: action,
                movingAverage: new MovingAverage(_movingAverageSize),
                referenceId: isReference ? _references.Length + 1 : 0,
                iterations: iterations,
                timeMultiplier: timeMultiplier
            );
            _calls = AppendIn(_calls, call);
            if (isReference)
            {
                _references = AppendIn(_references, call);
            }
            return this;
        }

        private T[] AppendIn<T>(T[] items, T item)
        {
            if (items == null)
            {
                return new[] { item };
            }
            else
            {
                return items.Append(item).ToArray();
            }
        }

        public void Run()
        {
            _callMeasurer.Restart();
            GC.Collect();
            _gcMovingAverage.Push(_callMeasurer.UpToNow);

            float totalRunTime = 0;
            foreach (BenchmarkCall call in _calls)
            {
                int iterations = call.Iterations ?? _iterations;
                float callTime = ExecuteCall(call.Action, iterations) * call.TimeMultiplier;
                call.MovingAverage.Push(callTime);
                totalRunTime += callTime;
            }
            _totalMovingAverage.Push(totalRunTime);
        }

        private float ExecuteCall(Action action, int iterations)
        {
            float callTotalTime = 0f;
            for (int i = 0; i < iterations; i++)
            {
                float callTime = 0f;
                _prepareForCall?.Invoke(i);

                //_minTimeMeasurer.Restart();
                //ExecuteBaseMinOperation();
                //minTime = _minTimeMeasurer.UpToNow;
                //minTime = MeasureBaseMinTime();
                MeasureAndRecordMinTime();

                _callMeasurer.Restart();
                action();
                callTime += _callMeasurer.UpToNow;

                _callMeasurer.Restart();
                _noneCall();
                callTime -= _callMeasurer.UpToNow;
                callTotalTime += callTime; // callTime < minTime ? minTime : callTime;
            }
            return callTotalTime;
        }

        private void MeasureAndRecordMinTime()
        {
            _minBaseAverage.Push(MeasureBaseMinTime());
        }

        private float MeasureBaseMinTime()
        {
            _minTimeMeasurer.Restart();
            return _minTimeMeasurer.UpToNow;
        }

        private void ExecuteBaseMinOperation()
        {
            _y = 7f;
            _z = (_x + _y) * (_z - _x) * _y;
            _z = 11f;
            _x = 17f;
            //_minBaseAverage.Push(_minTimeMeasurer.UpToNow);
        }

        public float FPSPercent(float refFPS = 60f)
        {
            return _totalMovingAverage.Average / (1000f / refFPS);
        }

        public BenchmarkCallEntry ReportGC()
        {
            return new BenchmarkCallEntry
            {
                Title = "[GC Time]",
                Percentage = _gcMovingAverage.Average / (_totalMovingAverage.Average + _gcMovingAverage.Average),
                ReferenceComparisons = new float[0],
                Avg = _gcMovingAverage.Average,
                Max = _gcMovingAverage.MaxValue,
                Min = _gcMovingAverage.MinValue
            };
        }

        public BenchmarkTotalEntry ReportTotal()
        {
            return new BenchmarkTotalEntry
            {
                Avg = _totalMovingAverage.Average,
                Max = _totalMovingAverage.MaxValue,
                Min = _totalMovingAverage.MinValue,
                Percent80FPS = FPSPercent(80f),
                Percent60FPS = FPSPercent(60f)
            };

        }

        public IEnumerable<BenchmarkCallEntry> ReportCalls()
        {
            int inx = 0;
            BenchmarkCallEntry[] entries = new BenchmarkCallEntry[_calls.Length];
            foreach (BenchmarkCall call in _calls)
            {
                string title = call.Name;
                if (call.IsReference)
                {
                    title += $" [Ref#{call.ReferenceId}]";
                }
                entries[inx++] = new BenchmarkCallEntry
                {
                    Title = title,
                    Percentage = call.MovingAverage.Average / _totalMovingAverage.Average,
                    ReferenceId = call.ReferenceId,
                    ReferenceComparisons = ReportReferenceComparisons(call.MovingAverage.Average),
                    Avg = call.MovingAverage.Average,
                    Max = call.MovingAverage.MaxValue,
                    Min = call.MovingAverage.MinValue
                };
            }
            return entries.OrderByDescending(e => e.Avg);
        }

        private float[] ReportReferenceComparisons(float avgValue)
        {
            avgValue = Mathf.Max(_minBaseAverage.Average, avgValue);
            float[] comparisons = new float[_references.Length];
            for (int i = 0; i < _references.Length; i++)
            {
                float refAvg = Mathf.Max(_minBaseAverage.Average, _references[i].MovingAverage.Average);
                comparisons[i] = avgValue/ refAvg;
            }
            return comparisons;
        }
    }
}
