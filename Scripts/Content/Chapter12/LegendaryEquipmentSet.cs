using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter12
{
    public class LegendaryPieceDefinition
    {
        public string ItemId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Slot { get; set; } = "Weapon"; // Weapon, Armor, Head
        public int RequiredLevel { get; set; } = 45;
        public int AttackBonus { get; set; } = 0;
        public int DefenseBonus { get; set; } = 0;
        public string SpecialTrait { get; set; } = "";
        public bool IsAcquired { get; set; } = false;
    }

    /// <summary>
    /// Content definition for The Solwarden Sun-King Legendary Set (Tier 5 Legendary Equipment).
    /// Manages acquisition, stat bonuses, unique active traits, and set bonus activation.
    /// </summary>
    public class LegendaryEquipmentSet
    {
        private readonly Dictionary<string, LegendaryPieceDefinition> _pieces = new(StringComparer.OrdinalIgnoreCase);

        public string SetId { get; } = "set_solwarden_legendary";
        public string DisplayName { get; } = "Solwarden Sun-King Regalia";
        public string FullSetBonusTrait { get; } = "Sun-King Solar Core: +25% Holy Damage, Immune to Starlight Crystal Hazards, 15% Chance to Emit Solar Nova on Hit";

        public LegendaryEquipmentSet()
        {
            InitializeSetPieces();
        }

        public void InitializeSetPieces()
        {
            // 1. Solwarden Astral Greatsword
            RegisterPiece(new LegendaryPieceDefinition
            {
                ItemId = "item_legendary_solwarden_greatsword",
                Name = "Solwarden Astral Greatsword",
                Slot = "Weapon",
                RequiredLevel = 45,
                AttackBonus = 145,
                DefenseBonus = 0,
                SpecialTrait = "Sunflare Cleave: Crits unleash 120 Holy Damage AOE shockwave"
            });

            // 2. Solwarden Sun-King Cuirass
            RegisterPiece(new LegendaryPieceDefinition
            {
                ItemId = "item_legendary_solwarden_cuirass",
                Name = "Solwarden Sun-King Cuirass",
                Slot = "Armor",
                RequiredLevel = 45,
                AttackBonus = 0,
                DefenseBonus = 120,
                SpecialTrait = "Solar Aegis: When HP falls below 30%, gain 300 HP Holy Shield"
            });

            // 3. Solwarden Crown of Sol
            RegisterPiece(new LegendaryPieceDefinition
            {
                ItemId = "item_legendary_solwarden_crown",
                Name = "Solwarden Crown of Sol",
                Slot = "Head",
                RequiredLevel = 45,
                AttackBonus = 25,
                DefenseBonus = 75,
                SpecialTrait = "Crown Radiance: Increases all Holy ability damage by 15%"
            });
        }

        public void RegisterPiece(LegendaryPieceDefinition piece)
        {
            if (piece != null && !string.IsNullOrEmpty(piece.ItemId))
            {
                _pieces[piece.ItemId] = piece;
            }
        }

        public bool AcquirePiece(string itemId)
        {
            if (!_pieces.TryGetValue(itemId, out var p)) return false;
            if (p.IsAcquired) return true;

            p.IsAcquired = true;
            Core.Logger.Info($"LegendaryEquipmentSet: Player acquired Legendary piece '{p.Name}' ({itemId})! Trait: {p.SpecialTrait}.");
            return true;
        }

        public bool IsFullSetAcquired()
        {
            foreach (var p in _pieces.Values)
            {
                if (!p.IsAcquired) return false;
            }
            return true;
        }

        public LegendaryPieceDefinition? GetPiece(string itemId)
        {
            return _pieces.TryGetValue(itemId, out var p) ? p : null;
        }

        public List<LegendaryPieceDefinition> GetAllPieces()
        {
            return new List<LegendaryPieceDefinition>(_pieces.Values);
        }
    }
}
