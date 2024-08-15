using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Shadow2DColliderSpawnerConvexHull : MonoBehaviour
{
    [Header("ShowDebugLine")]
    [SerializeField] private bool showDebugLine;

    [Space]

    [Header("ShadowSetting")]
    [SerializeField] private Vector3 lightSource;
    [SerializeField] private LayerMask shadowReceiverLayer;
    [SerializeField] private float maxShadowDistance = 50f;
    [SerializeField] private float minShadowDistance = 0.1f;
    [SerializeField] private float zAxisOffset = 0f;

    [Space]

    [Header("Material")]
    [SerializeField] private Material meshMaterial;

    [Header("RayCastReference")]
    [SerializeField] private float rayCastHeight = 1f;

    private GameObject shadowObject;
    private Mesh shadowMesh;
    private Vector3[] objectVertices;
    //private Vector3 refStart;
    private Vector3 refEnd;
    private Vector3 zRefPoint;
    private bool isHitWall;
    private PolygonCollider2D polygonCollider;

    private WallLightsManager wallLightsManager;
    
    private void OnEnable()
    {
        objectVertices = GetComponent<MeshFilter>().mesh.vertices;

        wallLightsManager = GameObject.FindObjectOfType<WallLightsManager>();
        
        shadowObject = new GameObject("ShadowObject");

        MeshFilter meshFilter = shadowObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = shadowObject.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        shadowMesh = new Mesh { name = "ShadowMesh" };
        meshFilter.mesh = shadowMesh;

        polygonCollider = shadowObject.AddComponent<PolygonCollider2D>();



        if (meshMaterial)
        {
            meshRenderer.material = meshMaterial;
        }
        else 
        {
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            meshMaterial.name = "MaterialTemporary";
        }
       
    }

    private void FixedUpdate()
    {
        if (this.isActiveAndEnabled)
        {
            UpdateRefRayCast();

            if (wallLightsManager != null)
            {
                lightSource = wallLightsManager.NowLightPos;
        }
        }

        if (isHitWall)
        {
            UpdateShadowMesh();
            UpdatePolygonCollider();
        }
        
    }

    void UpdateRefRayCast() 
    {

        refEnd = new Vector3(0, rayCastHeight, 0);
        Vector3 rayCastRefDirection = (refEnd - lightSource).normalized;

        if (Physics.Raycast(lightSource, rayCastRefDirection, out RaycastHit hitWall, maxShadowDistance, shadowReceiverLayer))
        {
            isHitWall = true;
        }
        else
        {
            isHitWall = false;
        }
        if (showDebugLine && isHitWall)
        {
            Debug.DrawLine(lightSource, hitWall.point, Color.white);
        }
        zRefPoint = new Vector3(0, 0, hitWall.point.z);

        shadowObject.transform.position = zRefPoint;
    }

    void UpdateShadowMesh()
    {
        List<Vector3> projectedVertices = new List<Vector3>();
        Vector3 zMinus = new Vector3(0, 0, zAxisOffset);

        for (int i = 0; i < objectVertices.Length; i++)
        {
            Vector3 worldVertex = transform.TransformPoint(objectVertices[i]);
            Vector3 direction = (worldVertex - lightSource).normalized;

            if (showDebugLine)
            {
                Debug.DrawLine(lightSource, worldVertex, Color.yellow);
            }

            if (Physics.Raycast(worldVertex, direction, out RaycastHit hit, maxShadowDistance, shadowReceiverLayer))
            {
                float distance = Vector3.Distance(lightSource, hit.point);

                if (distance >= minShadowDistance)
                {
                    projectedVertices.Add(hit.point - (zRefPoint - zMinus));

                    if (showDebugLine)
                    {
                        Debug.DrawLine(worldVertex, hit.point, Color.red);
                    }
                }
            }
            else
            {
                Vector3 projectedPoint = worldVertex + direction * maxShadowDistance;

                projectedVertices.Add(projectedPoint - (zRefPoint - zMinus));

                if (showDebugLine)
                {
                    Debug.DrawLine(worldVertex, projectedPoint, Color.green);
                }
            }
        }

        List<Vector3> hull = ComputeConvexHull(projectedVertices);

        shadowMesh.Clear();
        shadowMesh.vertices = hull.ToArray();
        shadowMesh.triangles = TriangulateConvexHull(hull).ToArray();
        shadowMesh.RecalculateNormals();
        shadowMesh.RecalculateBounds();

    }

    void UpdatePolygonCollider()
    {
        List<Vector2> colliderPath = new List<Vector2>();

        for (int i = 0; i < shadowMesh.vertexCount; i++)
        {
            Vector3 vertex = shadowMesh.vertices[i];
            colliderPath.Add(new Vector2(vertex.x, vertex.y));
        }

        polygonCollider.SetPath(0, colliderPath.ToArray());
    }

    List<Vector3> ComputeConvexHull(List<Vector3> points)
    {
        if (points.Count <= 1)
            return points;

        points.Sort((a, b) => {
            int xComparison = a.x.CompareTo(b.x);
            return xComparison != 0 ? xComparison : a.y.CompareTo(b.y);
        });

        List<Vector3> lowerHull = new List<Vector3>();
        foreach (var point in points)
        {
            while (lowerHull.Count >= 2 && Cross(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], point) <= 0)
            {
                lowerHull.RemoveAt(lowerHull.Count - 1);
            }
            lowerHull.Add(point);
        }

        List<Vector3> upperHull = new List<Vector3>();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            var point = points[i];
            while (upperHull.Count >= 2 && Cross(upperHull[upperHull.Count - 2], upperHull[upperHull.Count - 1], point) <= 0)
            {
                upperHull.RemoveAt(upperHull.Count - 1);
            }
            upperHull.Add(point);
        }

        upperHull.RemoveAt(upperHull.Count - 1);
        lowerHull.RemoveAt(lowerHull.Count - 1);

        lowerHull.AddRange(upperHull);
        return lowerHull;
    }

    float Cross(Vector3 o, Vector3 a, Vector3 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }

    List<int> TriangulateConvexHull(List<Vector3> hull)
    {
        List<int> triangles = new List<int>();
        for (int i = 1; i < hull.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        return triangles;
    }

    private void OnDisable()
    {
        Destroy(shadowObject);
      
        Debug.Log("clear");
    }
}
