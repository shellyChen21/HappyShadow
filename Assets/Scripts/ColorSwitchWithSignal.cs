using UnityEngine;

public class ColorSwitchWithSignal : MonoBehaviour
{
    [SerializeField] private Color blue;
    [SerializeField] private Color red;
    
    private MeshRenderer meshRenderer;
    private static readonly int BaseColor = Shader.PropertyToID("_PillColor");
    
    public void ChangeColorToBlue()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        var material = meshRenderer.material;
        material.SetColor(BaseColor, blue);
    }
    
    public void ChangeColorToRed()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        var material = meshRenderer.material;
        material.SetColor(BaseColor, red);
    }
}