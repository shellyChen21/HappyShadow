using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CollisionPlaySound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float delayTime;
    [SerializeField] private DecalProjector decal;

    private void Start()
    {
        if (!audioSource)
            return;

        audioSource.clip = audioClip;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (decal != null)
            Tween.Custom(0, 1, 2, onValueChange: newValue => decal.fadeFactor = newValue);
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(delayTime);
        audioSource.Play();
    }
}