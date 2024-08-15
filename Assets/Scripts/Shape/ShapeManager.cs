using UnityEngine;

namespace Shape
{
    public class ShapeManager : MonoBehaviour
    {
        [SerializeField] private int allNeedSnapCount;

        private int currentSnapCount;

        public void OnFinishedSnap()
        {
            currentSnapCount++;
            
            if(currentSnapCount<allNeedSnapCount)
                return;
            
            Debug.Log("finished!");
        }
    }
}