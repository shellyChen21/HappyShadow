using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTrackHead : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float headBodyDistance = -0.45f;
    private Vector3 originPos;

    private void Start()
    {
        Vector3 yDis = new Vector3(0, headBodyDistance, 0);
        originPos = (Vector3.zero + yDis) - head.position ;
    }
    void FixedUpdate()
    {
        transform.position = originPos + head.position;

    }
}
