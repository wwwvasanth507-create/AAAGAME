using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Chapter2
{
    public class DistrictNode
    {
        public string DistrictId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public float Radius { get; set; } = 20f;
    }

    /// <summary>
    /// Layout builder for Elderwood Grove (second major settlement) districts, market,
    /// alchemy workshop, temple, and guard barracks.
    /// </summary>
    public class SecondSettlementContent
    {
        private readonly Dictionary<string, DistrictNode> _districts = new(StringComparer.OrdinalIgnoreCase);

        public void InitializeSecondSettlement()
        {
            RegisterDistrict(new DistrictNode
            {
                DistrictId = "dist_warden_hall",
                DisplayName = "Warden's Great Hall",
                Position = new Vector3(200, 0, 300),
                Radius = 25f
            });

            RegisterDistrict(new DistrictNode
            {
                DistrictId = "dist_sylvan_market",
                DisplayName = "Sylvan Marketplace",
                Position = new Vector3(220, 0, 280),
                Radius = 20f
            });

            RegisterDistrict(new DistrictNode
            {
                DistrictId = "dist_ironwood_forge",
                DisplayName = "Ironwood Forge",
                Position = new Vector3(180, 0, 320),
                Radius = 15f
            });

            RegisterDistrict(new DistrictNode
            {
                DistrictId = "dist_golden_leaf_inn",
                DisplayName = "The Golden Leaf Inn",
                Position = new Vector3(240, 0, 320),
                Radius = 18f
            });

            RegisterDistrict(new DistrictNode
            {
                DistrictId = "dist_alchemy_workshop",
                DisplayName = "Corin's Alchemy Workshop",
                Position = new Vector3(170, 0, 280),
                Radius = 15f
            });

            RegisterDistrict(new DistrictNode
            {
                DistrictId = "dist_sun_temple",
                DisplayName = "Temple of the Sun",
                Position = new Vector3(200, 5, 350),
                Radius = 22f
            });
        }

        public void RegisterDistrict(DistrictNode dist)
        {
            if (dist != null && !string.IsNullOrEmpty(dist.DistrictId))
            {
                _districts[dist.DistrictId] = dist;
            }
        }

        public DistrictNode? GetDistrict(string districtId)
        {
            return _districts.TryGetValue(districtId, out var d) ? d : null;
        }

        public IReadOnlyCollection<DistrictNode> AllDistricts => _districts.Values;
    }
}
