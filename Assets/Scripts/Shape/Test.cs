using UnityEngine;

namespace Shape
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private ShapeTag shapeTag;
        
        private Vector3 highestPoint;
        private Vector3 lowestPoint;
        private Vector3 leftmostPoint;
        private Vector3 rightmostPoint;

        private void OnDrawGizmos()
        {
            if (shapeTag != ShapeTag.Circle)
            {
                T();
                return;
            }
            
            var _mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            if (_mesh == null)
            {
                return;
            }

            var vertices = _mesh.vertices;

            if (vertices.Length == 0)
                return;

            

            // 初始化最高點、最低點、最左、最右點
            highestPoint = transform.TransformPoint(vertices[0]);
            lowestPoint = highestPoint;
            leftmostPoint = highestPoint;
            rightmostPoint = highestPoint;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(vertices[i]);

                // 檢查是否為最高點
                if (worldPos.y > highestPoint.y)
                {
                    highestPoint = worldPos;
                }

                // 檢查是否為最低點
                if (worldPos.y < lowestPoint.y)
                {
                    lowestPoint = worldPos;
                }

                // 檢查是否為最左邊的點
                if (worldPos.x < leftmostPoint.x)
                {
                    leftmostPoint = worldPos;
                }

                // 檢查是否為最右邊的點
                if (worldPos.x > rightmostPoint.x)
                {
                    rightmostPoint = worldPos;
                }
            }

            // 設定不同的顏色來標示這些點
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(highestPoint, 0.05f); // 綠色表示最高點

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(lowestPoint, 0.05f); // 藍色表示最低點

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(leftmostPoint, 0.05f); // 黃色表示最左邊

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rightmostPoint, 0.05f); // 紅色表示最右邊
        }

        private void T()
        {
            var _mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            if (_mesh == null)
            {
                return;
            }

            var vertices = _mesh.vertices;

            Gizmos.color = Color.red;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(vertices[i]);
                
                // 在頂點位置繪製一個小球來表示頂點
                Gizmos.DrawSphere(worldPos, 0.02f);
                UnityEditor.Handles.Label(worldPos, i.ToString());
            }
        }
    }
}