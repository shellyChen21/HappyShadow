#if UNITY_EDITOR
using UnityEngine;

public class GizmoViewer : MonoBehaviour
{
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private Color gizmosColor = Color.white;
    [SerializeField] private float xLength = 0.15f;
    [SerializeField] private float zLength = 0.15f;

    Vector3 x1;
    Vector3 z1;

    void OnDrawGizmos()
    {
        x1 = new Vector3(xLength, 0 ,0);
        z1 = new Vector3(0, 0, zLength);

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position - x1, transform.position + x1);
        Gizmos.DrawLine(transform.position - z1, transform.position + z1);

    }
}
#endif
