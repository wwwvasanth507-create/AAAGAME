using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Graphics
{
    public enum ShaderCategory
    {
        Character,
        Terrain,
        Vegetation,
        Water,
        UI,
        Sky,
        Transparent,
        Emission,
        Dissolve,
        Highlight
    }

    /// <summary>
    /// Central manager for material and shader parameter adjustments, seasonal variants,
    /// dissolve effects, and interactive material highlights.
    /// </summary>
    public class ShaderManager
    {
        private readonly Dictionary<string, ShaderMaterial> _materialCache = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterMaterial(string materialId, ShaderMaterial material)
        {
            if (material != null)
            {
                _materialCache[materialId] = material;
            }
        }

        public void SetMaterialDissolve(string materialId, float dissolveAmount)
        {
            if (_materialCache.TryGetValue(materialId, out var mat))
            {
                mat.SetShaderParameter("dissolve_amount", Math.Clamp(dissolveAmount, 0.0f, 1.0f));
            }
        }

        public void SetMaterialHighlight(string materialId, Color highlightColor, float intensity)
        {
            if (_materialCache.TryGetValue(materialId, out var mat))
            {
                mat.SetShaderParameter("highlight_color", highlightColor);
                mat.SetShaderParameter("highlight_intensity", Math.Max(0.0f, intensity));
            }
        }

        public void SetSeasonalVariant(string materialId, string season)
        {
            if (_materialCache.TryGetValue(materialId, out var mat))
            {
                mat.SetShaderParameter("seasonal_tint", GetSeasonTint(season));
            }
        }

        private Color GetSeasonTint(string season)
        {
            return season.ToLowerInvariant() switch
            {
                "winter" => new Color(0.8f, 0.9f, 1.0f),
                "autumn" => new Color(1.0f, 0.6f, 0.3f),
                "spring" => new Color(0.9f, 1.0f, 0.8f),
                _ => Colors.White
            };
        }

        public void ClearCache()
        {
            _materialCache.Clear();
        }
    }
}
