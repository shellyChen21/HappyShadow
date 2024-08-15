using UnityEngine;

public class Shadow3DColliderSpawner : MonoBehaviour
{
    [SerializeField] private bool showDebugLine;
    [SerializeField] private Transform lightSource;
    [SerializeField] private LayerMask shadowReceiverLayer;
    private MeshCollider shadowCollider;
    private Mesh shadowMesh;
    private Vector3[] objectVertices;
    [SerializeField] private float maxShadowDistance = 50f; // 設置最大陰影距離
    [SerializeField] private float minShadowDistance = 0.1f; // 設置最小陰影距離

    void Awake()
    {
        shadowCollider = gameObject.AddComponent<MeshCollider>();
        shadowMesh = new Mesh { name = "ShadowCollider" };
        shadowCollider.sharedMesh = shadowMesh;
        objectVertices = GetComponent<MeshFilter>().mesh.vertices;
    }

    void FixedUpdate()
    {
        UpdateShadowMesh();
    }

    void UpdateShadowMesh()
    {
        Vector3[] projectedVertices = new Vector3[objectVertices.Length];
        for (int i = 0; i < objectVertices.Length; i++)
        {
            Vector3 worldVertex = transform.TransformPoint(objectVertices[i]);
            Vector3 direction = (worldVertex - lightSource.position).normalized;
            if (showDebugLine)
            {
                Debug.DrawLine(lightSource.position, worldVertex, Color.yellow); // 視覺化光源到頂點的線
            }
            

            if (Physics.Raycast(worldVertex, direction, out RaycastHit hit, maxShadowDistance, shadowReceiverLayer))
            {
                float distance = Vector3.Distance(lightSource.position, hit.point);
                if (distance >= minShadowDistance)
                {
                    projectedVertices[i] = transform.InverseTransformPoint(hit.point);

                    if (showDebugLine)
                    {
                        Debug.DrawLine(worldVertex, hit.point, Color.red); // 視覺化投影到牆上的線
                    }
                    
                }
            }
            else
            {
                Vector3 projectedPoint = worldVertex + direction * maxShadowDistance;
                projectedVertices[i] = transform.InverseTransformPoint(projectedPoint);

                if (showDebugLine)
                {
                    Debug.DrawLine(worldVertex, projectedPoint, Color.green); // 視覺化投影到最大距離的線
                }
                
            }
        }
        shadowMesh.vertices = projectedVertices;
        shadowMesh.triangles = GetComponent<MeshFilter>().mesh.triangles;  // 假設拓撲結構相同
        shadowMesh.RecalculateNormals();
        shadowMesh.RecalculateBounds();
        shadowCollider.sharedMesh = shadowMesh;
    }
}