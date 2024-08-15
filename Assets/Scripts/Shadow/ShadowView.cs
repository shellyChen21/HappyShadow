using System.Collections.Generic;
using PrimeTween;
using Shape;
using UnityEngine;

namespace Shadow
{
    public class ShadowView : MonoBehaviour, IInteractable
    {
        private ShapeTag shapeTag;
        private MeshCollider meshCollider;

        private MeshFilter meshFilter;
        private List<Vector3> shadowVertex = new();
        
        private MeshRenderer meshRenderer;
        private static readonly int AlphaColor = Shader.PropertyToID("_Alpha");

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetShapeTag(ShapeTag newTag) => shapeTag = newTag;


        public void DoSnap()
        {
            meshCollider = GetComponent<MeshCollider>();

            meshCollider.enabled = false;

            gameObject.SetActive(false);
        }

        public ShapeTag GetShapeTag()
        {
            return shapeTag;
        }

        public List<Vector3> GetShadowVertex()
        {
            return shadowVertex;
        }


        private void Update()
        {
            GetVertex();
        }

        private void GetVertex()
        {
            if (meshFilter == null)
                return;

            // 取得Mesh的所有頂點
            var vertices = meshFilter.sharedMesh.vertices;

            if (shapeTag != ShapeTag.Circle)
            {
                shadowVertex = VertexUtility.GetAllVertex(transform, vertices);
                return;
            }

            shadowVertex = VertexUtility.GetOtherVertex(transform, vertices);
        }
        
        public void ShowUp()
        {
            var material = meshRenderer.material;
            Tween.MaterialProperty(material, AlphaColor, 1, .5f);
        }

        public void Hide()
        {
            var material = meshRenderer.material;
            Tween.MaterialProperty(material, AlphaColor, 0, .5f);
        }

        // private void OnDrawGizmos()
        // {
        //     if (shadowVertex.Count == 0)
        //     {
        //         return;
        //     }
        //
        //     for (var i = 0; i < shadowVertex.Count; i++)
        //     {
        //         // 在頂點位置繪製一個小球來表示頂點
        //         Gizmos.DrawSphere(shadowVertex[i], 0.02f);
        //         UnityEditor.Handles.Label(shadowVertex[i], i.ToString());
        //     }
        // }
    }
}