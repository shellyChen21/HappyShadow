using System;
using PrimeTween;
using UnityEngine;

public class TextureSwitch : MonoBehaviour
{
    [SerializeField] private TextureGroup[] textureGroups;
    [SerializeField] private float time;

    private MeshRenderer meshRenderer;
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    private static readonly int MetallicMap = Shader.PropertyToID("_MetallicGlossMap");
    private static readonly int NormalMap = Shader.PropertyToID("_BumpMap");

    private int currentTextureIndex;
    private Material material;
    private Sequence sequence;

    // private void OnEnable()
    // {
    //     meshRenderer = GetComponent<MeshRenderer>();
    //     material = meshRenderer.material;
    //
    //     sequence = Sequence.Create(-1)
    //         .ChainCallback(SwitchTexture)
    //         .ChainDelay(time);
    // }

    private void SwitchTexture()
    {
        if (textureGroups[currentTextureIndex].baseMap != null)
            material.SetTexture(BaseMap, textureGroups[currentTextureIndex].baseMap);

        if (textureGroups[currentTextureIndex].metallicMap != null)
            material.SetTexture(MetallicMap, textureGroups[currentTextureIndex].metallicMap);

        if (textureGroups[currentTextureIndex].normalMap != null)
            material.SetTexture(NormalMap, textureGroups[currentTextureIndex].normalMap);

        currentTextureIndex++;

        if (currentTextureIndex >= textureGroups.Length)
            currentTextureIndex = 0;
    }

    public void StartChangeTexture()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

        sequence = Sequence.Create(-1)
            .ChainCallback(SwitchTexture)
            .ChainDelay(time);
    }

    public void StopChangeTexture()
    {
        sequence.Stop();
    }


    [Serializable]
    public class TextureGroup
    {
        public Texture baseMap;
        public Texture metallicMap;
        public Texture normalMap;
    }
}