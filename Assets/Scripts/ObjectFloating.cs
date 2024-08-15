using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloating : MonoBehaviour
{
    [SerializeField] private float amplitude = 1.0f;
    [SerializeField] private float speed = 1.0f;
    private Vector3 oiriginPos;
    void Start()
    {
        oiriginPos = transform.position;
    }

    void Update()
    {
        float newY = oiriginPos.y + Mathf.Sin(Time.time * speed) * amplitude;

        transform.position = new Vector3(oiriginPos.x, newY, oiriginPos.z);
    }
}
