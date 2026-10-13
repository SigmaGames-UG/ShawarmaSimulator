using System.Collections.Generic;
using UnityEngine;

public class AimGlow : MonoBehaviour
{
    [Header("Outline")]
    [SerializeField] private Material outlineMaterial;

    [Header("Soft emission glow (optional)")]
    [SerializeField] private bool useEmission = true;
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float glowIntensity = 0.25f;

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private MaterialPropertyBlock propertyBlock;
    private bool glowing;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        var list = new List<Renderer>();
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            if (r is MeshRenderer || r is SkinnedMeshRenderer)
                list.Add(r);
        }
        renderers = list.ToArray();

        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].sharedMaterials;

        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetGlow(bool enabled)
    {
        if (glowing == enabled) return;
        glowing = enabled;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer rend = renderers[i];

            if (enabled)
            {
                // Add the outline material as an extra material
                if (outlineMaterial != null)
                {
                    var original = originalMaterials[i];
                    var withOutline = new Material[original.Length + 1];
                    original.CopyTo(withOutline, 0);
                    withOutline[original.Length] = outlineMaterial;
                    rend.sharedMaterials = withOutline;
                }

                if (useEmission)
                {
                    rend.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetColor(EmissionColor, glowColor * glowIntensity);
                    rend.SetPropertyBlock(propertyBlock);
                }
            }
            else
            {
                rend.sharedMaterials = originalMaterials[i];
                rend.SetPropertyBlock(null);
            }
        }
    }
}