using UnityEngine;

public class CircleGizmo : MonoBehaviour
{
    public int resolution = 10;

    private void OnDrawGizmosSelected()
    {
        float step = 2f / resolution;
        for (int i = 0; i <= resolution; i++)
        {
            ShowPoint(i * step - 1f, -1f);
            ShowPoint(i * step - 1f, 1f);
        }
        for (int i = 0; i <= resolution; i++)
        {
            ShowPoint(-1f, i * step - 1f);
            ShowPoint(1f, i * step - 1f);
        }
    }

    private void ShowPoint(float x, float y)
    {
        Vector2 square = new Vector2(x, y);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(square, 0.025f);

        Vector2 circle = square.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(square.normalized * 0.8f, 0.025f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector2.zero, square);

    }
}