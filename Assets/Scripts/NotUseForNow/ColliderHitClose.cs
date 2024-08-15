using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHitClose : MonoBehaviour
{
    [SerializeField] private GameObject closeObject;
    private void OnTriggerEnter2D(Collider2D other)
    {
        closeObject.SetActive(false);
    }
}
