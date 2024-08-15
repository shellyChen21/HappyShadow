using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Shadow;
using Shape;

public class Shadow3DColliderSpawnerConvexHull : MonoBehaviour
{
    [Header("ShowDebugLine")] [SerializeField]
    private bool showDebugLine;

    [Space] [Header("ShadowSetting")] [SerializeField]
    private Vector3 lightSource;

    [SerializeField] private LayerMask shadowReceiverLayer;
    [SerializeField] private float maxShadowDistance = 50f;
    [SerializeField] private float minShadowDistance = 0.1f;
    [SerializeField] private float zAxisOffset = 0f;
    [SerializeField] private float meshColliderThickness = 0.1f;
    [SerializeField] private ShapeTag shapeTag;

    [Space] [Header("Material")] [SerializeField]
    private Material meshMaterial;

    [Header("RayCastReference")] [SerializeField]
    private float rayCastHeight = 1f;

    private GameObject shadowObject;
    private Mesh shadowMesh;
    private Vector3[] objectVertices;
    private Vector3[] originalVertices;
    private Vector3 refEnd;
    private Vector3 zRefPoint;
    private Vector3 meshRotation;
    private MeshCollider meshCollider;
    private bool isHitWall;

    private WallLightsManager wallLightsManager;

    private void OnEnable()
    {
        originalVertices = GetComponent<MeshFilter>().mesh.vertices.Clone() as Vector3[];
        objectVertices = originalVertices;

        wallLightsManager = FindObjectOfType<WallLightsManager>();

        shadowObject = new GameObject("ShadowObject");

        shadowObject.layer = LayerMask.NameToLayer("Shape");

        MeshFilter meshFilter = shadowObject.AddComponent<MeshFilter>();
        MeshRenderer shadowRenderer = shadowObject.AddComponent<MeshRenderer>();
        shadowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        shadowMesh = new Mesh { name = "ShadowMesh" };
        meshFilter.mesh = shadowMesh;

        meshCollider = shadowObject.AddComponent<MeshCollider>();

        if (meshMaterial)
        {
            shadowRenderer.material = meshMaterial;
        }
        else
        {
            shadowRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            meshMaterial.name = "MaterialTemporary";
        }

        var shadowView = shadowObject.AddComponent<ShadowView>();
        shadowView.SetShapeTag(shapeTag);
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
            UpdateMeshCollider();
        }
    }


    void UpdateRefRayCast()
    {
        refEnd = new Vector3(0, rayCastHeight, 0);
        Vector3 rayCastRefDirection = (refEnd - lightSource).normalized;

        if (Physics.Raycast(lightSource, rayCastRefDirection, out RaycastHit hitWall, maxShadowDistance,
                shadowReceiverLayer))
        {
            isHitWall = true;
            zRefPoint = hitWall.point;
        }
        else
        {
            shadowMesh.Clear();
            isHitWall = false;
        }

        if (showDebugLine && isHitWall)
        {
            Debug.DrawLine(lightSource, hitWall.point, Color.white);
        }

        shadowObject.transform.position = zRefPoint;
        shadowObject.transform.rotation = Quaternion.LookRotation(-hitWall.normal);

        Vector3 oppositeEulerAngles = -shadowObject.transform.rotation.eulerAngles;
        meshRotation = oppositeEulerAngles;
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
                    //projectedVertices.Add(hit.point - (zRefPoint - zMinus));
                    Vector3 zOffset = shadowObject.transform.forward * zAxisOffset;
                    projectedVertices.Add(hit.point - (zRefPoint + zOffset));

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
        
        if (hull.Count < 3)
        {
            shadowMesh.Clear();
            return;
        }

        Quaternion rotation = Quaternion.Euler(meshRotation);
        for (int i = 0; i < hull.Count; i++)
        {
            hull[i] = rotation * hull[i];
        }

        shadowMesh.Clear();
        shadowMesh.vertices = hull.ToArray();
        shadowMesh.triangles = TriangulateConvexHull(hull).ToArray();
        shadowMesh.RecalculateNormals();
        shadowMesh.RecalculateBounds();
    }

    void UpdateMeshCollider()
    {
        if (shadowMesh.vertices.Length < 3 )
            return;
        
        Mesh colliderMesh = GenerateColliderMeshWithThickness(shadowMesh, meshColliderThickness);
        

        meshCollider.sharedMesh = colliderMesh;
    }

    Mesh GenerateColliderMeshWithThickness(Mesh originalMesh, float thickness)
    {
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;

        int vertexCount = originalVertices.Length;
        Vector3[] vertices = new Vector3[vertexCount * 2];
        int[] triangles = new int[originalTriangles.Length * 2 + vertexCount * 6];

        Vector3 zOffset = new Vector3(0, 0, meshColliderThickness / 2);

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = originalVertices[i] + zOffset;
            vertices[i + vertexCount] = originalVertices[i] + Vector3.back * thickness + zOffset;
        }

        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            triangles[i] = originalTriangles[i];
            triangles[i + 1] = originalTriangles[i + 1];
            triangles[i + 2] = originalTriangles[i + 2];

            triangles[originalTriangles.Length + i] = originalTriangles[i + 2] + vertexCount;
            triangles[originalTriangles.Length + i + 1] = originalTriangles[i + 1] + vertexCount;
            triangles[originalTriangles.Length + i + 2] = originalTriangles[i] + vertexCount;
        }

        for (int i = 0; i < vertexCount; i++)
        {
            int nextIndex = (i + 1) % vertexCount;

            triangles[originalTriangles.Length * 2 + i * 6] = i;
            triangles[originalTriangles.Length * 2 + i * 6 + 1] = nextIndex;
            triangles[originalTriangles.Length * 2 + i * 6 + 2] = i + vertexCount;

            triangles[originalTriangles.Length * 2 + i * 6 + 3] = nextIndex;
            triangles[originalTriangles.Length * 2 + i * 6 + 4] = nextIndex + vertexCount;
            triangles[originalTriangles.Length * 2 + i * 6 + 5] = i + vertexCount;
        }

        Mesh colliderMesh = new Mesh { name = "ShadowMeshCollider" };
        colliderMesh.vertices = vertices;
        colliderMesh.triangles = triangles;
        colliderMesh.RecalculateNormals();
        colliderMesh.RecalculateBounds();

        return colliderMesh;
    }

    List<Vector3> ComputeConvexHull(List<Vector3> points)
    {
        if (points.Count <= 1)
            return points;

        points.Sort((a, b) =>
        {
            int xComparison = a.x.CompareTo(b.x);
            return xComparison != 0 ? xComparison : a.y.CompareTo(b.y);
        });

        List<Vector3> lowerHull = new List<Vector3>();
        foreach (var point in points)
        {
            while (lowerHull.Count >= 2 &&
                   Cross(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], point) <= 0)
            {
                lowerHull.RemoveAt(lowerHull.Count - 1);
            }

            lowerHull.Add(point);
        }

        List<Vector3> upperHull = new List<Vector3>();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            var point = points[i];
            while (upperHull.Count >= 2 &&
                   Cross(upperHull[upperHull.Count - 2], upperHull[upperHull.Count - 1], point) <= 0)
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