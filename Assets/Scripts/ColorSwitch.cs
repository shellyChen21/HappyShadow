using PrimeTween;
using UnityEngine;

public class ColorSwitch : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private Color newColor;

    private MeshRenderer meshRenderer;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private Sequence sequence;
    private Color originalColor;

    // private void OnEnable()
    // {
    //     meshRenderer = GetComponent<MeshRenderer>();
    //
    //     var material = meshRenderer.material;
    //     originalColor = material.GetColor(BaseColor);
    //
    //     sequence = Sequence.Create(-1)
    //         .ChainDelay(time)
    //         .ChainCallback(() =>
    //         {
    //             material.SetColor(BaseColor, newColor);
    //         })
    //         .ChainDelay(time)
    //         .ChainCallback(() =>
    //         {
    //             material.SetColor(BaseColor, originalColor);
    //         });
    // }

    public void StartChangeColor()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        var material = meshRenderer.material;
        originalColor = material.GetColor(BaseColor);

        sequence = Sequence.Create(-1)
            .ChainDelay(time)
            .ChainCallback(() =>
            {
                material.SetColor(BaseColor, newColor);
            })
            .ChainDelay(time)
            .ChainCallback(() =>
            {
                material.SetColor(BaseColor, originalColor);
            });
    }

    public void StopChangeColor()
    {
        sequence.Stop();
    }
}