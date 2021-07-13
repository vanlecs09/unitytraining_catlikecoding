using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDimensionGraph : MonoBehaviour
{
    [SerializeField]
    Transform _pointPrefab;
    [SerializeField, Range(10, 200)]
    int _resolution = 10;
    Transform[] _points;
    [SerializeField]
    ThreeDimensionFunctionLibrary.FunctionName function;
    public enum TransitionMode { Cycle, Random }
    [SerializeField]
    TransitionMode transitionMode;
    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;
    float _duration;

    bool transitioning;

    ThreeDimensionFunctionLibrary.FunctionName transitionFunction;
    void Awake()
    {
        float step = 2f / _resolution;
        var scale = Vector3.one * step;
        _points = new Transform[_resolution * _resolution];
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = Instantiate(_pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            _points[i] = point;
        }
    }

    private void Update()
    {
        _duration += Time.deltaTime;
        if (transitioning)
        {
            if (_duration >= transitionDuration)
            {
                _duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (_duration >= functionDuration)
        {
            _duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    void UpdateFunction()
    {
        ThreeDimensionFunctionLibrary.Function f = ThreeDimensionFunctionLibrary.GetFunction(function);
        float step = 2f / _resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == _resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            _points[i].localPosition = f(u, v, Time.time);
        }
    }

    void UpdateFunctionTransition()
    {
        ThreeDimensionFunctionLibrary.Function
            from = ThreeDimensionFunctionLibrary.GetFunction(transitionFunction),
            to = ThreeDimensionFunctionLibrary.GetFunction(function);
        float progress = _duration / transitionDuration;
        float time = Time.time;
        float step = 2f / _resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == _resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            _points[i].localPosition = ThreeDimensionFunctionLibrary.Morph(
                u, v, time, from, to, progress
            );
        }
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            ThreeDimensionFunctionLibrary.GetNextFunctionName(function) :
            ThreeDimensionFunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}
