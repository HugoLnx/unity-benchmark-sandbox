using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MutableWaitWhile : CustomYieldInstruction
{
    public override bool keepWaiting => Predicate();
    public Func<bool> Predicate;

    public MutableWaitWhile(Func<bool> predicate = null)
    {
        Predicate = predicate ?? (() => false);
    }
}

public class MutableWaitUntil : CustomYieldInstruction
{
    public override bool keepWaiting => !Predicate();
    public Func<bool> Predicate;

    public MutableWaitUntil(Func<bool> predicate = null)
    {
        Predicate = predicate ?? (() => true);
    }
}

public interface ISimplePool<T>
{
    T Get();
    void Release(T resource);
}

public class SimpleExpandablePool<T> : ISimplePool<T>
{
    private Func<T> _factory;
    private Queue<T> _resources = new();
    private int _minSize;

    public SimpleExpandablePool(Func<T> factory, int minSize = 20)
    {
        _factory = factory;
        _minSize = minSize;

        for (int i = 0; i < _minSize; i++)
        {
            Grow();
        }
    }

    public T Get()
    {
        if (_resources.Count == 0)
        {
            Grow();
        }
        return _resources.Dequeue();
    }

    public void Release(T obj)
    {
        _resources.Enqueue(obj);
    }

    private void Grow()
    {
        _resources.Enqueue(_factory());
    }
}

public static class CoroutinesX
{
    private static readonly SimpleExpandablePool<WaitForSecondsRealtime> _waitSecondsRealtimePool = new(() => new(0f), minSize: 100);
    private static readonly Dictionary<int, WaitForSeconds> _waitForSecondsDictionary = new();
    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new();
    private const float _waitForSecondsMaxPrecision = 1e-3f;
    public static YieldInstruction WaitForSeconds(float seconds)
    {
        int id = Mathf.RoundToInt(seconds / _waitForSecondsMaxPrecision);
        if (!_waitForSecondsDictionary.TryGetValue(id, out var waitForSeconds))
        {
            waitForSeconds = new WaitForSeconds(id *_waitForSecondsMaxPrecision);
            _waitForSecondsDictionary[id] = waitForSeconds;
        }
        return waitForSeconds;
    }
    
    public static IEnumerator WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitRealtime = _waitSecondsRealtimePool.Get();
        waitRealtime.waitTime = seconds;
        yield return waitRealtime;
        _waitSecondsRealtimePool.Release(waitRealtime);
    }
}

public abstract class CoroutinesGarbageCreator : MonoBehaviour
{
    [SerializeField] private int _commandInx = -1;
    [SerializeField] private int _coroutinesAmount;
    [SerializeField] private int _coroutinesRealCount;
    private Stack<Coroutine> _coroutines = new();
    protected Func<YieldInstruction>[] _waitCommandsBuilders;

    private int NextCommandInx => (_commandInx >= 0 ? _commandInx : _coroutines.Count) % CoroutineWaitCommandBuilders.Length;
    private float ToTargetAmount => _coroutinesAmount - _coroutines.Count;
    protected Func<YieldInstruction>[] CoroutineWaitCommandBuilders => _waitCommandsBuilders ??= CreateWaitCommandsBuilders();
    protected abstract Func<YieldInstruction>[] CreateWaitCommandsBuilders();

    private static readonly YieldInstruction WaitFor1Sec = new WaitForSeconds(1f);
    private void OnEnable()
    {
        ResetCoroutines();
        StartCoroutine(CoroutinesMaintainerLoop());
    }

    [Button]
    private void ResetCoroutines()
    {
        StopAllCoroutines();
        _coroutines.Clear();
    }

    private IEnumerator CoroutinesMaintainerLoop()
    {
        while (true)
        {
            yield return WaitFor1Sec;

            while (ToTargetAmount > 0)
            {
                _coroutines.Push(StartCoroutine(NewCoroutineLoop()));
            }
            while (ToTargetAmount < 0)
            {
                StopCoroutine(_coroutines.Pop());
            }
            _coroutinesRealCount = _coroutines.Count;
        }
    }

    private IEnumerator NewCoroutineLoop()
    {
        Func<YieldInstruction> waitCommandBuilder = CoroutineWaitCommandBuilders[NextCommandInx];
        while (true)
        {
            yield return waitCommandBuilder();
        }
    }
}
