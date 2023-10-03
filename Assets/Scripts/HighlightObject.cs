using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Renderer))]
public class HighlightObject : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;
    private Color defaultEmissionColor;
    public Color highlightColor = Color.yellow;
    public float highlightEmissionStrength = 2.0f; // Default strength

    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");

    private void Start()
    {
        rend = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        // Get the default emission color
        rend.GetPropertyBlock(propertyBlock);
        defaultEmissionColor = propertyBlock.GetColor(EmissiveColor);
    }

    private void OnMouseEnter()
    {
        SetEmissionColor(highlightColor * highlightEmissionStrength);
    }

    private void OnMouseExit()
    {
        SetEmissionColor(defaultEmissionColor);
    }

    private void SetEmissionColor(Color color)
    {
        propertyBlock.SetColor(EmissiveColor, color);
        rend.SetPropertyBlock(propertyBlock);
    }
}
