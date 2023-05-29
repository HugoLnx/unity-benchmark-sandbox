using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    TMP_Text _text;
    private MovingAverage _unscaledDeltaTimeAvg;
    private WaitForSeconds _waitUpdateDelay = new(0.2f);

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _unscaledDeltaTimeAvg = new MovingAverage(60);
    }

    private void Start()
    {
        StartCoroutine(UpdateFps());
    }

    private void Update()
    {
        _unscaledDeltaTimeAvg.Push(Time.unscaledDeltaTime);
    }

    private IEnumerator UpdateFps()
    {
        while (true)
        {
            _text.text = $"FPS: {1f / _unscaledDeltaTimeAvg.Average:0.00}";
            yield return _waitUpdateDelay;
        }
    }
}
