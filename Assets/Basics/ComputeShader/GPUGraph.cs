using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;
    [SerializeField, Range(10, maxResolution)]
    int _resolution = 10;
    [SerializeField]
    ThreeDimensionFunctionLibrary.FunctionName function;
    public enum TransitionMode { Cycle, Random }
    [SerializeField]
    TransitionMode transitionMode;
    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;
    float _duration;

    bool transitioning;
    ComputeBuffer positionsBuffer;
    ThreeDimensionFunctionLibrary.FunctionName transitionFunction;
    [SerializeField]
    ComputeShader computeShader;
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;
    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
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
        UpdateFunctionOnGPU();
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / _resolution;
        computeShader.SetInt(resolutionId, _resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);

        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, _duration / transitionDuration)
            );
        }

        var kernelIndex =
            (int)function + (int)(transitioning ? transitionFunction : function) * 5;
        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(_resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / _resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, _resolution * _resolution);
    }


    void OnDrawGizmos()
    {
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / _resolution));
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * (2f + 2f / _resolution));
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            ThreeDimensionFunctionLibrary.GetNextFunctionName(function) :
            ThreeDimensionFunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}
