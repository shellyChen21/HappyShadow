using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class CollisionAndCreateTriangle : MonoBehaviour
{
    [SerializeField] private List<Vector3> collisionPoints;
    [SerializeField] private List<MeshRenderer> allRenderer;
    [SerializeField] private Material meshMaterial;

    private void Start()
    {
        if (meshMaterial == null)
        {
            meshMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            meshMaterial.name = "Triangle_Temporary";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Sphere"))
        {
            Vector3 collisionPosition = other.transform.position;
            MeshRenderer currentRenderer = other.GetComponent<MeshRenderer>();
                         

            if (!collisionPoints.Contains(collisionPosition))
            {
                collisionPoints.Add(collisionPosition);
                allRenderer.Add(currentRenderer);
                currentRenderer.material.SetColor("_BaseColor", Color.red);
                currentRenderer.material.SetColor("_EmissionColor", Color.red);
            }
            else
            {
                collisionPoints.Remove(collisionPosition);
                allRenderer.Remove(currentRenderer);
                currentRenderer.material.SetColor("_BaseColor", Color.white);
                currentRenderer.material.SetColor("_EmissionColor", Color.white);
            }


            if (collisionPoints.Count == 3)
            {
                for (int i=0 ; i < allRenderer.Count; i++)
                {
                    allRenderer[i].material.SetColor("_BaseColor", Color.white);
                    allRenderer[i].material.SetColor("_EmissionColor", Color.white);
                }

                CreateTriangle();              
                collisionPoints.Clear();
                allRenderer.Clear();
            }
        }
    }

    private void CreateTriangle()
    {
        GameObject newGameObject = new GameObject("TriangleMesh");

        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();

        Mesh createMesh = new Mesh();

        meshFilter.mesh = createMesh;

        createMesh.name = "Triangle";

        createMesh.SetVertices(collisionPoints);

        var indices = new[] { 0, 1, 2 };

        createMesh.SetIndices(indices, MeshTopology.Triangles, 0);

        createMesh.RecalculateNormals();

        meshRenderer.material = meshMaterial;

    }
}
