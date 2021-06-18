using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderer : MonoBehaviour
{
    public Vector2[] coordinates;
    public int xSize, ySize;
    private Vector3[] vertices;
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    public Color color;

    // LineRenderer
    void Start()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Line";
        xSize = ySize = coordinates.Length / 2 - 1;
        vertices = new Vector3[coordinates.Length];
        for(int i = 0; i < coordinates.Length; i++) 
        {
            vertices[i] = new Vector3(coordinates[i].x, coordinates[i].y);
        }
        mesh.vertices = vertices;
        int[] triangles = new int[(xSize) * (ySize) * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
                colors[i] = color;
        }


        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
