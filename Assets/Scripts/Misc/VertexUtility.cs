using System.Collections.Generic;
using UnityEngine;

public static class VertexUtility
{
    public static List<Vector3> GetAllVertex(this Transform transform, Vector3[] vertices)
    {
        if (vertices.Length == 0)
            return null;

        var shapeVertex = new List<Vector3>();

        foreach (var vertex in vertices)
        {
            Vector3 worldPos = transform.TransformPoint(vertex);

            shapeVertex.Add(worldPos);
        }

        return shapeVertex;
    }

    public static List<Vector3> GetOtherVertex(this Transform transform, Vector3[] vertices)
    {
        if (vertices.Length == 0)
            return null;

        var shapeVertex = new List<Vector3>();

        var targetTopVertex = transform.TransformPoint(vertices[0]);
        var targetBottomVertex = transform.TransformPoint(vertices[0]);
        var targetLeftVertex = transform.TransformPoint(vertices[0]);
        var targetRightVertex = transform.TransformPoint(vertices[0]);

        // 迴圈遍歷每個頂點
        foreach (var vertex in vertices)
        {
            var worldPos = transform.TransformPoint(vertex);

            // 更新最上（Y最大）、最下（Y最小）、最左（X最小）、最右（X最大）頂點
            if (worldPos.y > targetTopVertex.y) targetTopVertex = worldPos;
            if (worldPos.y < targetBottomVertex.y) targetBottomVertex = worldPos;
            if (worldPos.x < targetLeftVertex.x) targetLeftVertex = worldPos;
            if (worldPos.x > targetRightVertex.x) targetRightVertex = worldPos;
        }

        shapeVertex.Add(targetTopVertex);
        shapeVertex.Add(targetBottomVertex);
        shapeVertex.Add(targetLeftVertex);
        shapeVertex.Add(targetRightVertex);

        return shapeVertex;
    }
}