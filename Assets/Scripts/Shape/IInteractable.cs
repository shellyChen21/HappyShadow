using System.Collections.Generic;
using UnityEngine;

namespace Shape
{
    public interface IInteractable
    {
        void DoSnap();
        ShapeTag GetShapeTag();
        List<Vector3> GetShadowVertex();
    }
}