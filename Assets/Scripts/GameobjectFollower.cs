using UnityEngine;
using System.Collections.Generic;

public class GameobjectFollower : MonoBehaviour
{
    [HideInInspector] public bool isFollowing;
    [SerializeField] private List<Transform> followerObjects;
    [SerializeField] private List<Transform> followerTargets;

    void FixedUpdate()
    {
        if (followerObjects.Count != followerTargets.Count || !isFollowing)
            return; 

        for (int i = 0; i < followerTargets.Count;i++ ) 
        {
            followerObjects[i].transform.position = followerTargets[i].transform.position;
        }
    }
}
