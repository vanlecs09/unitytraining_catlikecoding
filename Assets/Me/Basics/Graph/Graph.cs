using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform _pointPrefab;
    [SerializeField, Range(10, 100)]
    int _resolution = 10;
    Transform[] _points;
    [SerializeField]
    FunctionLibrary.FunctionName function;
    void Awake()
    {
        Vector3 position = Vector3.zero;
        float step = _resolution / 2;
        var scale = Vector3.one / _resolution;
        _points = new Transform[_resolution * _resolution];
        for (int i = 0; i < _resolution; i++)
        {
            for (int j = 0; j < _resolution; j++)
            {
                Transform point = Instantiate(_pointPrefab);
                position.x = (i) / step - 1f;
                position.y = position.x * position.x * position.x;
                position.z = (j) / step - 1f;
                point.localPosition = position;
                point.localScale = scale;
                point.SetParent(transform);
                _points[i*_resolution + j] = point;
            }
        }
    }

    private void Update()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = _points[i];
            Vector3 position = point.localPosition;
            // position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time));
            position.y = f(position.x, position.z, Time.time);
            point.localPosition = position;
        }
    }
}
