using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangentSpaceVisualizer : MonoBehaviour
{
    public float offset = 0.01f;
    public float scale = 0.1f;


    private void OnDrawGizmos()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh)
            {
                ShowTangentSpace(mesh);
            }
        }
    }

    private void ShowTangentSpace(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        for (int i = 0; i < vertices.Length; i++)
        {
            ShowTangentSpace(
                transform.TransformPoint(vertices[i]),
                transform.TransformDirection(normals[i]),
                transform.TransformDirection(tangents[i]),
                // transform.TransformDirection(Vector3.Cross(normals[i], tangents[i]))
                tangents[i].w
            );
        }
        // throw new NotImplementedException();
    }

    void ShowTangentSpace(Vector3 vertex, Vector3 normal, Vector3 tangent, float binormalSign)
    {
        vertex += normal * offset;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(vertex, vertex + normal * scale);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(vertex, vertex + tangent * scale);
        Vector3 binormal = Vector3.Cross(normal, tangent) * binormalSign;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(vertex, vertex + binormal * scale);
    }
}
