using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story.Campaign
{
    public enum CharacterRole
    {
        Protagonist,
        Companion,
        Mentor,
        Ally,
        NeutralNPC,
        Antagonist,
        Vendor,
        QuestGiver
    }

    public class CharacterProfile
    {
        public string CharacterId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public CharacterRole Role { get; set; } = CharacterRole.Ally;
        public string FactionId { get; set; } = string.Empty;
        public int Age { get; set; } = 25;
        public string Background { get; set; } = string.Empty;
        public string Motivations { get; set; } = string.Empty;
        public Dictionary<string, string> Relationships { get; set; } = new();
        public string PersonalityTraits { get; set; } = string.Empty;
        public string CombatStyle { get; set; } = "SwordAndShield";
        public string VisualDescription { get; set; } = string.Empty;
        public string VoiceStyle { get; set; } = "heroic_male";
        public int StoryImportanceRank { get; set; } = 1;
        public string DlcModuleId { get; set; } = string.Empty;
    }

    public class CharacterDatabase
    {
        private readonly Dictionary<string, CharacterProfile> _characters = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterCharacter(CharacterProfile profile)
        {
            if (profile != null && !string.IsNullOrEmpty(profile.CharacterId))
            {
                _characters[profile.CharacterId] = profile;
            }
        }

        public CharacterProfile? GetCharacter(string characterId)
        {
            return _characters.TryGetValue(characterId, out var c) ? c : null;
        }

        public IReadOnlyCollection<CharacterProfile> GetAllCharacters() => _characters.Values;

        public void RegisterDefaultCharacters()
        {
            RegisterCharacter(new CharacterProfile
            {
                CharacterId = "char_hero_of_eternia",
                DisplayName = "The Chosen Champion",
                Role = CharacterRole.Protagonist,
                FactionId = "faction_valen_crown",
                Age = 22,
                Background = "An orphan raised in Oakvale carrying the dormant mark of Eternia.",
                Motivations = "Restore peace to Eternia and uncover the mystery of the ancient seals.",
                CombatStyle = "Versatile",
                StoryImportanceRank = 10
            });

            RegisterCharacter(new CharacterProfile
            {
                CharacterId = "char_elder_alden",
                DisplayName = "Elder Alden",
                Role = CharacterRole.Mentor,
                FactionId = "faction_valen_crown",
                Age = 68,
                Background = "Keeper of Oakvale lore and former Royal Archivist.",
                Motivations = "Guide the young champion and preserve ancient knowledge.",
                VoiceStyle = "wise_elder",
                StoryImportanceRank = 8
            });

            RegisterCharacter(new CharacterProfile
            {
                CharacterId = "char_captain_valerius",
                DisplayName = "Captain Valerius",
                Role = CharacterRole.Ally,
                FactionId = "faction_valen_crown",
                Age = 35,
                Background = "Commander of the Valenhold Royal Guard.",
                Motivations = "Protect the citizens from rising monster incursions.",
                CombatStyle = "HeavyGuard",
                StoryImportanceRank = 7
            });
        }
    }
}
