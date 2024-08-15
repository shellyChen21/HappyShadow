using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shadow2DColliderSpawner : MonoBehaviour
{
    [Header("ShowDebugLine")]
    [SerializeField] private bool showDebugLine;

    [Space]

    [Header("ShadowSetting")]
    [SerializeField] private Transform lightSource;
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

    private Vector3 refStart;
    private Vector3 refEnd;
    private Vector3 zRefPoint;
    private bool isHitWall;
    private PolygonCollider2D polygonCollider;

    private void Start()
    {
        objectVertices = GetComponent<MeshFilter>().mesh.vertices;

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
        UpdateShadowMesh();
        UpdatePolygonCollider();

        //以下計算Ref RayCast

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
            Vector3 direction = (worldVertex - lightSource.position).normalized;

            if (showDebugLine)
            {
                Debug.DrawLine(lightSource.position, worldVertex, Color.yellow);
            }

            if (Physics.Raycast(worldVertex, direction, out RaycastHit hit, maxShadowDistance, shadowReceiverLayer))
            {
                float distance = Vector3.Distance(lightSource.position, hit.point);

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

        List<Vector3> hull = ComputeConcaveHull(projectedVertices);

        shadowMesh.Clear();
        shadowMesh.vertices = hull.ToArray();
        shadowMesh.triangles = Triangulate(hull).ToArray();
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

    List<Vector3> ComputeConcaveHull(List<Vector3> points)
    {
        // Compute the centroid of the points
        Vector3 centroid = Vector3.zero;
        foreach (var point in points)
        {
            centroid += point;
        }
        centroid /= points.Count;

        // Sort points based on their angle from the centroid
        points = points.OrderBy(p => Mathf.Atan2(p.y - centroid.y, p.x - centroid.x)).ToList();

        return points;
    }

    private List<int> Triangulate(List<Vector3> vertices)
    {
        List<int> indices = new List<int>();

        if (vertices.Count < 3)
            return indices;

        int[] V = new int[vertices.Count];
        if (Area(vertices) > 0)
        {
            for (int v = 0; v < vertices.Count; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < vertices.Count; v++)
                V[v] = (vertices.Count - 1) - v;
        }

        int nv = vertices.Count;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices;

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(vertices, u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices;
    }

    private float Area(List<Vector3> vertices)
    {
        int n = vertices.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector3 pval = vertices[p];
            Vector3 qval = vertices[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(List<Vector3> vertices, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = vertices[V[u]];
        Vector3 B = vertices[V[v]];
        Vector3 C = vertices[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = vertices[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }

    private void OnDisable()
    {
        shadowMesh.Clear();
        Debug.Log("clear");
    }
}