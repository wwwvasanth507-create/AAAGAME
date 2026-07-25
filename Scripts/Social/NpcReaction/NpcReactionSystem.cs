using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Social.Reputation;
using HeroOfEternia.Social.Crime;
using HeroOfEternia.Social.Diplomacy;

namespace HeroOfEternia.Social.NpcReaction
{
    /// <summary>
    /// Context used to evaluate an NPC's reaction to the player.
    /// </summary>
    public class ReactionContext
    {
        public string NpcId { get; set; } = "";
        public string NpcFactionId { get; set; } = "";
        public string NpcOccupation { get; set; } = "";
        public string SettlementId { get; set; } = "";
        public string RegionId { get; set; } = "";
        public double WorldTimeFraction { get; set; } = 0.5;
        public string CurrentWeather { get; set; } = "clear";
        public string PersonalityHook { get; set; } = "";
        public int PlayerLevel { get; set; } = 1;
        public bool IsPlayerHidden { get; set; } = false;
        public bool IsPlayerGuarded { get; set; } = false;
    }

    /// <summary>
    /// Result of an NPC reaction evaluation.
    /// </summary>
    public class ReactionResult
    {
        /// <summary>Overall disposition: -100 (hostile) to +100 (friendly)</summary>
        public int Disposition { get; set; } = 0;
        public string DispositionLabel { get; set; } = "Neutral";
        public bool WillAttack { get; set; } = false;
        public bool WillFlee { get; set; } = false;
        public bool WillReport { get; set; } = false;
        public bool WillTrade { get; set; } = false;
        public string DialogueModifier { get; set; } = "neutral";
        public string GreetingKey { get; set; } = "greet_neutral";
        public List<string> ReactionFlags { get; set; } = new();
    }

    /// <summary>
    /// Evaluates NPC reactions considering faction, reputation, crime history,
    /// time of day, settlement security, occupation, personality, world events, and weather.
    /// </summary>
    public class NpcReactionSystem
    {
        public const string ServiceKey = "NpcReactionSystem";

        private ReputationManager? _reputationManager;
        private CrimeManager? _crimeManager;
        private DiplomacyManager? _diplomacyManager;
        private Factions.FactionDatabase? _factionDb;

        // Disposition thresholds
        private const int AttackThreshold = -60;
        private const int FleeThreshold = -30;
        private const int TradeThreshold = 20;
        private const int ReportThreshold = -20;

        /// <summary>
        /// Initialize with references to other systems.
        /// </summary>
        public void Initialize(
            ReputationManager reputationManager,
            CrimeManager crimeManager,
            DiplomacyManager diplomacyManager,
            Factions.FactionDatabase factionDb)
        {
            _reputationManager = reputationManager;
            _crimeManager = crimeManager;
            _diplomacyManager = diplomacyManager;
            _factionDb = factionDb;
        }

        // ─────────────────────── Reaction Evaluation ───────────────────────

        /// <summary>
        /// Evaluates an NPC's full reaction to the player based on all context.
        /// </summary>
        public ReactionResult EvaluateReaction(ReactionContext context)
        {
            var result = new ReactionResult();
            int disposition = 0;

            // 1. Reputation factors
            disposition += EvaluateReputationFactors(context);

            // 2. Crime history factors
            disposition += EvaluateCrimeFactors(context);

            // 3. Faction alignment factors
            disposition += EvaluateFactionFactors(context);

            // 4. Time of day factors
            disposition += EvaluateTimeOfDayFactors(context);

            // 5. Settlement security factors
            disposition += EvaluateSecurityFactors(context);

            // 6. Occupation factors
            disposition += EvaluateOccupationFactors(context);

            // 7. Personality hooks (future)
            disposition += EvaluatePersonalityFactors(context);

            // 8. World event factors (future)
            disposition += EvaluateWorldEventFactors(context);

            // 9. Weather factors
            disposition += EvaluateWeatherFactors(context);

            // 10. Player level factor
            disposition += EvaluatePlayerLevelFactors(context);

            // Clamp final disposition
            result.Disposition = Math.Clamp(disposition, -100, 100);

            // Determine label
            result.DispositionLabel = result.Disposition switch
            {
                <= -80 => "Murderous",
                <= -60 => "Hostile",
                <= -40 => "Angry",
                <= -20 => "Cold",
                <= 20  => "Neutral",
                <= 40  => "Friendly",
                <= 60  => "Warm",
                <= 80  => "Affectionate",
                _      => "Devoted"
            };

            // Determine behaviors
            result.WillAttack = result.Disposition <= AttackThreshold;
            result.WillFlee = result.Disposition <= FleeThreshold && !result.WillAttack;
            result.WillTrade = result.Disposition >= TradeThreshold;
            result.WillReport = result.Disposition <= ReportThreshold && context.NpcOccupation == "Guard";

            // Determine dialogue modifier
            result.DialogueModifier = result.Disposition switch
            {
                <= -60 => "hostile",
                <= -20 => "cold",
                <= 20  => "neutral",
                <= 60  => "friendly",
                _      => "warm"
            };

            result.GreetingKey = result.DialogueModifier switch
            {
                "hostile" => "greet_hostile",
                "cold" => "greet_cold",
                "neutral" => "greet_neutral",
                "friendly" => "greet_friendly",
                "warm" => "greet_warm",
                _ => "greet_neutral"
            };

            // If hidden, NPC won't react
            if (context.IsPlayerHidden)
            {
                result.WillAttack = false;
                result.WillFlee = false;
                result.WillReport = false;
            }

            return result;
        }

        // ─────────────────────── Factor Evaluations ───────────────────────

        private int EvaluateReputationFactors(ReactionContext context)
        {
            int score = 0;
            if (_reputationManager == null) return 0;

            // Global reputation (small effect)
            int global = _reputationManager.GetGlobal();
            score += global / 50; // -20 to +20

            // Faction reputation (larger effect)
            if (!string.IsNullOrEmpty(context.NpcFactionId))
            {
                int factionRep = _reputationManager.GetFaction(context.NpcFactionId);
                score += factionRep / 20; // -50 to +50
            }

            // Settlement reputation
            if (!string.IsNullOrEmpty(context.SettlementId))
            {
                int settlementRep = _reputationManager.GetSettlement(context.SettlementId);
                score += settlementRep / 30; // -33 to +33
            }

            // Individual reputation (largest effect)
            if (!string.IsNullOrEmpty(context.NpcId))
            {
                int individualRep = _reputationManager.GetIndividual(context.NpcId);
                score += individualRep / 15; // -66 to +66
            }

            return score;
        }

        private int EvaluateCrimeFactors(ReactionContext context)
        {
            int score = 0;
            if (_crimeManager == null) return 0;

            // Active crimes in the same settlement
            if (!string.IsNullOrEmpty(context.SettlementId))
            {
                var crimes = _crimeManager.GetCrimesInSettlement(context.SettlementId);
                foreach (var crime in crimes)
                {
                    score -= crime.Severity switch
                    {
                        CrimeSeverity.Minor => 5,
                        CrimeSeverity.Moderate => 10,
                        CrimeSeverity.Serious => 20,
                        CrimeSeverity.Severe => 35,
                        CrimeSeverity.Capital => 50,
                        _ => 5
                    };
                }
            }

            // Active bounties
            int bounty = _crimeManager.GetTotalBounty("player");
            score -= bounty / 20; // Up to -50 for 1000 bounty

            return score;
        }

        private int EvaluateFactionFactors(ReactionContext context)
        {
            if (_diplomacyManager == null || _factionDb == null) return 0;

            // If the NPC has a faction, check diplomatic relations with player's faction
            if (!string.IsNullOrEmpty(context.NpcFactionId) && !string.IsNullOrEmpty(context.NpcFactionId))
            {
                // Check if the NPC's faction has diplomatic modifiers
                return _diplomacyManager.GetDiplomaticReputationModifier(context.NpcFactionId, "player");
            }

            return 0;
        }

        private int EvaluateTimeOfDayFactors(ReactionContext context)
        {
            // NPCs are more wary at night
            double time = context.WorldTimeFraction;
            if (time >= 0.8 || time <= 0.2) // Night time
            {
                return -10;
            }
            else if (time >= 0.65 || time <= 0.35) // Evening/Dawn
            {
                return -5;
            }
            return 0;
        }

        private int EvaluateSecurityFactors(ReactionContext context)
        {
            // Insecure settlements make NPCs more wary
            // Higher security = more relaxed
            if (!string.IsNullOrEmpty(context.SettlementId))
            {
                // Future: lookup settlement security level
                // For now, return neutral
                return 0;
            }
            return 0;
        }

        private int EvaluateOccupationFactors(ReactionContext context)
        {
            return context.NpcOccupation switch
            {
                "Guard" => -5,  // Guards are professional
                "Bandit" => -15, // Bandits are naturally hostile
                "Priest" => 10,  // Priests are welcoming
                "Merchant" => 5, // Merchants want customers
                "Child" => 0,    // Children are neutral
                _ => 0
            };
        }

        private int EvaluatePersonalityFactors(ReactionContext context)
        {
            // Future: personality-driven modifiers
            return 0;
        }

        private int EvaluateWorldEventFactors(ReactionContext context)
        {
            // Future: world event modifiers (festivals increase friendliness, etc.)
            return 0;
        }

        private int EvaluateWeatherFactors(ReactionContext context)
        {
            return context.CurrentWeather.ToLowerInvariant() switch
            {
                "storm" => -10,   // Everyone is on edge during storms
                "blizzard" => -10,
                "sandstorm" => -10,
                "rain" => -5,
                "clear" => 0,
                "sunny" => 5,     // Nice weather improves mood
                _ => 0
            };
        }

        private int EvaluatePlayerLevelFactors(ReactionContext context)
        {
            // High-level players may intimidate or impress NPCs
            if (context.PlayerLevel >= 50) return 10;
            if (context.PlayerLevel >= 30) return 5;
            if (context.PlayerLevel >= 10) return 2;
            return 0;
        }

        // ─────────────────────── Quick Queries ───────────────────────

        /// <summary>
        /// Quick check if a specific NPC should attack the player.
        /// </summary>
        public bool ShouldAttack(string npcId, string factionId, string settlementId, double timeFraction)
        {
            var ctx = new ReactionContext
            {
                NpcId = npcId,
                NpcFactionId = factionId,
                SettlementId = settlementId,
                WorldTimeFraction = timeFraction
            };
            return EvaluateReaction(ctx).WillAttack;
        }

        /// <summary>
        /// Quick check if an NPC will trade with the player.
        /// </summary>
        public bool WillTrade(string npcId, string factionId, string settlementId)
        {
            var ctx = new ReactionContext
            {
                NpcId = npcId,
                NpcFactionId = factionId,
                SettlementId = settlementId
            };
            return EvaluateReaction(ctx).WillTrade;
        }
    }
}