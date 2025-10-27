using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    public Transform[] GetPoints()
    {
        int count = transform.childCount;
        Transform[] points = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            points[i] = transform.GetChild(i);
        }
        return points;
    }

    void OnDrawGizmos()
    {
        // Draw gizmos for clarity
        var pts = GetPoints();
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pts.Length - 1; i++)
        {
            Gizmos.DrawSphere(pts[i].position, 0.2f);
            Gizmos.DrawLine(pts[i].position, pts[i + 1].position);
        }
    }
}
