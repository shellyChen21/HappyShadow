using System.Collections.Generic;
using UnityEngine;

public class RealtimeCreateTriangle : MonoBehaviour
{
    [HideInInspector] public bool isCreateTriangle;
    [HideInInspector] public bool clearMesh;

    [Header("TriangleResource")]
    [SerializeField] private List<Transform> objectPos;
    [SerializeField] private Material meshMaterial;

    [Space]

    [Header("ShadowSetting")]
    [SerializeField] private Transform lightSource;
    [SerializeField] private LayerMask shadowReceiverLayer;
    [SerializeField] private float maxShadowDistance = 50f;
    [SerializeField] private float minShadowDistance = 0.1f;
    [SerializeField] private float zAxisOffset = 0f;

    [Space]

    [Header("RayCastReference")]
    [SerializeField] private float rayCastHeight = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLine;
    

    private Mesh createMesh;
    private bool hasCreateTriangle;
    private GameObject triangleGameObject;
    private PolygonCollider2D polygonCollider;
    private Vector3 refStart;
    private Vector3 refEnd;
    private Vector3 zRefPoint;
    private bool isHitWall;

    private void Start()
    {
        if (meshMaterial == null)
        {
            meshMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            meshMaterial.name = "TriangleTemporary";
        }

        lightSource = GameObject.Find("LightSourcePos").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (isCreateTriangle && objectPos.Count == 3)
        {
            CreateTriangle();
            isCreateTriangle = false;
            hasCreateTriangle = true;
        }

        if (hasCreateTriangle)
        {
            UpdateTriangle();
        }

        if (clearMesh)
        {
            ClearMesh();
            clearMesh = false;
        }

        //RayCast參考點計算
        refStart = new Vector3(0, rayCastHeight, 0);
        refEnd = new Vector3(0, rayCastHeight, 1);
        Vector3 rayCastRefDirection = (refEnd - refStart).normalized;

        if (Physics.Raycast(refStart, rayCastRefDirection, out RaycastHit hitWall, maxShadowDistance, shadowReceiverLayer))
        {
            isHitWall = true;
        }
        else
        {
            isHitWall = false;
        }
        if (showDebugLine && isHitWall)
        {
            Debug.DrawLine(refStart, hitWall.point, Color.white);
        }
        zRefPoint = new Vector3(0,0,hitWall.point.z);


    }

    private void CreateTriangle()
    {
        //新增Mesh所需Component們及2D Collider
        triangleGameObject = new GameObject("TriangleMesh");
        MeshFilter meshFilter = triangleGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = triangleGameObject.AddComponent<MeshRenderer>();
        createMesh = new Mesh { name = "Triangle" };
        meshFilter.mesh = createMesh;
        meshRenderer.material = meshMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        polygonCollider = triangleGameObject.AddComponent<PolygonCollider2D>();
    }

    private void UpdateTriangle()
    {
        List<Vector3> verticesPos = new List<Vector3>();
        Vector3 zMinus = new Vector3(0, 0, zAxisOffset);

        for (int i = 0; i < objectPos.Count; i++)
        {
            Vector3 worldVertex = objectPos[i].position;
            Vector3 direction = (worldVertex - lightSource.position).normalized;

            if (showDebugLine)
            {
                Debug.DrawLine(lightSource.position, worldVertex, Color.yellow); // 視覺化光源到Point_A_B_C的線
            }

            if (Physics.Raycast(worldVertex, direction, out RaycastHit hit, maxShadowDistance, shadowReceiverLayer))
            {
                float distance = Vector3.Distance(lightSource.position, hit.point);
                if (distance >= minShadowDistance)
                {
                    verticesPos.Add(hit.point - (zRefPoint - zMinus));
                    if (showDebugLine)
                    {
                        Debug.DrawLine(worldVertex, hit.point, Color.red); // 視覺化投影到牆上的線
                    }
                }
            }
            else
            {
                Vector3 projectedPoint = worldVertex + direction * maxShadowDistance;
                verticesPos.Add(projectedPoint - (zRefPoint - zMinus));
                if (showDebugLine)
                {
                    Debug.DrawLine(worldVertex, projectedPoint, Color.green); // 視覺化投影到最大距離的線
                }
            }
        }

        if (verticesPos.Count == 3)
        {
            createMesh.SetVertices(verticesPos);
            createMesh.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0);
            createMesh.RecalculateNormals();
            createMesh.RecalculateBounds();          

            Vector2[] colliderPoints = new Vector2[verticesPos.Count];
            for (int i = 0; i < verticesPos.Count; i++)
            {
                colliderPoints[i] = new Vector2(verticesPos[i].x, verticesPos[i].y);
            }
            polygonCollider.SetPath(0, colliderPoints);

            triangleGameObject.transform.position = new Vector3(0, 0, zRefPoint.z);
        }
    }

    private void ClearMesh()
    {
        if (createMesh != null)
        {
            createMesh.Clear();
        }

        if (triangleGameObject != null)
        {
            Destroy(triangleGameObject);
            triangleGameObject = null;
        }
        hasCreateTriangle = false;
    }
}