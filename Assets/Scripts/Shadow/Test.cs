using System.Collections.Generic;
using UnityEngine;

namespace Shadow
{
    public class Test : MonoBehaviour
    {
        public Transform source;
        public GameObject cube;
        public float normalScale;

        [SerializeField] private Material newMaterial;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private Vector3 refEnd;
        private GameObject shadowObject;
        private Vector3 zRefPoint;

        private void Start()
        {
            CreateNewMesh();
        }

        private void CreateNewMesh()
        {
            shadowObject = new GameObject("ShadowObject", typeof(MeshFilter), typeof(MeshRenderer));
            meshFilter = shadowObject.GetComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
            shadowObject.GetComponent<MeshRenderer>().material = newMaterial;
        }

        private void FixedUpdate()
        {
            Vector3[] vertices;
            Vector3[] normals;
        
            UpdateRefRayCast();
            ProjectVertex(source, cube, out vertices, out normals);
            UpdateMesh(vertices);
        }

        void UpdateRefRayCast()
        {
            refEnd = new Vector3(0, 1.5f, 0);
            Vector3 rayCastRefDirection = (refEnd - source.position).normalized;

            Vector3 directionToCube = (cube.transform.position - source.position).normalized;

            if (Physics.Raycast(source.position, directionToCube, out RaycastHit hitCube, 50))
            {
                // 更新 shadowObject 的位置和旋转
                zRefPoint = hitCube.point;
            }

            Debug.DrawLine(source.position, hitCube.point, Color.blue);

            shadowObject.transform.position = hitCube.point;
            shadowObject.transform.rotation = Quaternion.LookRotation(-hitCube.normal);
        }


        private void ProjectVertex(Transform source, GameObject obj, out Vector3[] vertices, out Vector3[] normals)
        {
            Vector3[] originalVertices = obj.GetComponent<MeshFilter>().mesh.vertices;
            List<Vector3> projectedVerticesList = new List<Vector3>();
            normals = new Vector3[originalVertices.Length];

            Vector3 sourcePos = source.position;

            for (var i = 0; i < originalVertices.Length; i++)
            {
                Ray direction = new Ray(sourcePos, obj.transform.TransformPoint(originalVertices[i]) - sourcePos);
                RaycastHit hit;

                if (Physics.Raycast(direction, out hit))
                {
                    if (hit.collider.gameObject != obj)
                    {
                        projectedVerticesList.Add(hit.point);
                        normals[i] = hit.normal;

                        // DEBUG
                        Debug.DrawLine(source.position, hit.point, Color.red);
                    }
                }
            }

            vertices = projectedVerticesList.ToArray();
        }


        private void UpdateMesh(Vector3[] vertices)
        {
            Mesh mesh = meshFilter.mesh;

            mesh.Clear();
            mesh.vertices = vertices;

            // Optionally, generate triangles and UVs if needed
            int[] triangles = new int[(vertices.Length - 2) * 3];
            for (int i = 0; i < vertices.Length - 2; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}