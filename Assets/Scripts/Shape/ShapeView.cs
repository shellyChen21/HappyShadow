using System.Collections.Generic;
using UnityEngine;

namespace Shape
{
    public class ShapeView : MonoBehaviour
    {
        [SerializeField] private ShapeTag tag;
        [SerializeField] private MeshCollider collider;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float radius;
        [SerializeField] private float matchRange = .03f;
        [SerializeField] private Color newColor;

        private List<Vector3> shapeVertex = new();

        private bool isSnap;
        private MeshRenderer meshRenderer;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            Snap();
            SetVertex();
        }

        private void Snap()
        {
            if (isSnap)
                return;

            var hitColliders = new Collider[8];

            var overlapCount = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, layerMask);

            if (overlapCount == 0)
                return;

            IInteractable interactObj = null;

            foreach (var col in hitColliders)
            {
                if (col == null)
                    continue;

                var interactShapeTag = col.GetComponent<IInteractable>().GetShapeTag();

                if (interactShapeTag != tag)
                    continue;

                var targetVertex = col.GetComponent<IInteractable>().GetShadowVertex();

                var isAllInside = IsInside(targetVertex);

                if (isAllInside)
                {
                    interactObj = col.GetComponent<IInteractable>();
                    interactObj.DoSnap();
                    ChangeColor();
                    isSnap = true;
                }

                break;
            }
        }

        private void ChangeColor()
        {
            collider.enabled = true;
            var material = meshRenderer.material;
            material.SetColor("_BaseColor", newColor);
        }

        private bool IsInside(List<Vector3> targetVertex)
        {
            if (targetVertex == null)
                return false;

            var allVerticesInRange = true;

            foreach (var vertex in targetVertex)
            {
                var closestVertex = FindClosestVertex(vertex, shapeVertex);

                var distance = Vector3.Distance(vertex, closestVertex);

                if (distance < matchRange)
                    continue;

                allVerticesInRange = false;
                break;
            }

            return allVerticesInRange;
        }

        Vector3 FindClosestVertex(Vector3 vertex, List<Vector3> vertices)
        {
            var closestVertex = vertices[0];
            var minDistance = Vector3.Distance(vertex, closestVertex);

            foreach (var v in vertices)
            {
                var distance = Vector3.Distance(vertex, v);


                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestVertex = v;
                }
            }

            return closestVertex;
        }

        private void SetVertex()
        {
            var meshFilter = GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                return;
            }

            var vertices = meshFilter.sharedMesh.vertices;

            if (vertices.Length == 0)
                return;

            shapeVertex = new List<Vector3>();

            if (tag != ShapeTag.Circle)
            {
                shapeVertex = VertexUtility.GetAllVertex(transform, vertices);

                return;
            }

            shapeVertex = VertexUtility.GetOtherVertex(transform, vertices);
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        
            if (shapeVertex.Count == 0)
            {
                return;
            }
        
            for (var i = 0; i < shapeVertex.Count; i++)
            {
                Gizmos.DrawSphere(shapeVertex[i], 0.02f);
                UnityEditor.Handles.Label(shapeVertex[i], i.ToString());
            }
        }
    }
}