using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDeformer : MonoBehaviour
{
    [SerializeField] private bool isHandCenter;

    [Space][Header("ShapeObjects")]
    //[SerializeField] private float scaleSmoothSpeed = 3;
    [SerializeField] private Transform sourceObject;

    [Space]
    [Header("TargetTransform")]
    [SerializeField] private Transform headTraget;
    [SerializeField] private Transform bodyTarget;
    [SerializeField] private Transform rightController;
    [SerializeField] private Transform leftController;

    [Space] [Header("ValueSetting")]   
    [SerializeField] private float xScaler = 1f;
    [SerializeField] private float yScaler = 1f;
    [SerializeField] private float rotationLerpSpeed = 5.0f;
    [SerializeField] private float objectOffsetZ;

    void FixedUpdate()
    {
        var handsCenter = (rightController.position + leftController.position) / 2;
        Vector3 offsetZ = new Vector3(0, 0, objectOffsetZ);
        var headAndCenterDis = Vector3.Distance(headTraget.position, handsCenter);

        float scaleY;

        if (handsCenter.y > headTraget.position.y)
        {
            scaleY = headAndCenterDis * -1 * yScaler;
        }
        else
        {
            scaleY = headAndCenterDis * yScaler;
        }
        
        
        if (isHandCenter)
        {
            sourceObject.position = handsCenter + offsetZ;
        }
        else
        {
            sourceObject.position = bodyTarget.position + offsetZ;
        }

        float dis = Vector3.Distance(rightController.position, leftController.position);
        float multiScale = dis * xScaler;

        //float scaleDelta = Time.deltaTime * scaleSmoothSpeed;
        sourceObject.localScale = new Vector3(multiScale, scaleY, 1);
        //sourceObject.localScale = Vector3.MoveTowards(sourceObject.localScale , resultScale, scaleDelta);

        Vector3 direction = rightController.position - leftController.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, direction) * bodyTarget.rotation;

            sourceObject.rotation =
                Quaternion.Slerp(sourceObject.rotation, targetRotation, Time.fixedDeltaTime * rotationLerpSpeed);
        }
    }

    public void SetSourceObject(Transform sourceTarget) => sourceObject = sourceTarget;
}