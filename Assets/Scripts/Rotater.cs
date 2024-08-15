using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1f;
    void Update()
    {
        // transform.rotation = new Quaternion(0,rotateSpeed * Time.deltaTime,0,0);

        transform.eulerAngles += new Vector3(0,0,rotateSpeed * Time.deltaTime);
    }
}
