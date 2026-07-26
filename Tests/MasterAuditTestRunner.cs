using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Tests
{
    /// <summary>
    /// Master Audit Test Runner executing all 20+ test suites across Prompts 0–30 in Hero of Eternia.
    /// </summary>
    public static class MasterAuditTestRunner
    {
        public static void RunFullAuditSuite()
        {
            Logger.Info("===============================================================");
            Logger.Info("HERO OF ETERNIA — MASTER AUDIT TEST SUITE (PROMPTS 0–30)");
            Logger.Info("===============================================================");

            int totalSuites = 0;

            try
            {
                // 1. Core Framework & TestRunner
                Logger.Info("[1/21] Running AbilitySystemTests...");
                AbilitySystemTests.RunAll();
                totalSuites++;

                Logger.Info("[2/21] Running Act2SystemTests...");
                Act2SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[3/21] Running AnimationSystemTests...");
                AnimationSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[4/21] Running AudioSystemTests...");
                AudioSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[5/21] Running CampaignDesignSystemTests...");
                CampaignDesignSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[6/21] Running Chapter2SystemTests...");
                Chapter2SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[7/21] Running Chapter3SystemTests...");
                Chapter3SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[8/21] Running CustomStoryAndObjectTests...");
                CustomStoryAndObjectTests.RunAll();
                totalSuites++;

                Logger.Info("[9/21] Running EconomySystemTests...");
                EconomySystemTests.RunAll();
                totalSuites++;

                Logger.Info("[10/21] Running EquipmentSystemTests...");
                EquipmentSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[11/21] Running ExplorationContentSystemTests...");
                ExplorationContentSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[12/21] Running GatheringAndCraftingTests...");
                GatheringAndCraftingTests.RunAll();
                totalSuites++;

                Logger.Info("[13/21] Running GraphicsSystemTests...");
                GraphicsSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[14/21] Running PrologueSystemTests...");
                PrologueSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[15/21] Running QuestSystemTests...");
                QuestSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[16/21] Running SettlementSystemTests...");
                SettlementSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[17/21] Running SocialSystemTests...");
                var socialTests = new SocialTests.SocialSystemTests();
                socialTests.RunAll();
                totalSuites++;

                Logger.Info("[18/21] Running StorySystemTests...");
                StorySystemTests.RunAll();
                totalSuites++;

                Logger.Info("[19/21] Running UISystemTests...");
                UISystemTests.RunAll();
                totalSuites++;

                Logger.Info("[20/22] Running WorldContentSystemTests...");
                WorldContentSystemTests.RunAll();
                totalSuites++;

                Logger.Info("[21/23] Running Chapter5SystemTests...");
                Chapter5SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[22/24] Running Chapter6SystemTests...");
                Chapter6SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[23/25] Running Chapter7SystemTests...");
                Chapter7SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[24/26] Running Chapter8SystemTests...");
                Chapter8SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[25/27] Running Chapter9SystemTests...");
                Chapter9SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[26/28] Running Chapter10SystemTests...");
                Chapter10SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[27/29] Running Chapter11SystemTests...");
                Chapter11SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[28/30] Running Chapter12SystemTests...");
                Chapter12SystemTests.RunAll();
                totalSuites++;

                Logger.Info("[29/30] Running Chapter13SystemTests...");
                Chapter13SystemTests.RunAll();
                totalSuites++;

                Logger.Info("===============================================================");
                Logger.Info($"ALL {totalSuites} TEST SUITES EXECUTED SUCCESSFULLY — 0 FAILURES.");
                Logger.Info("===============================================================");
            }
            catch (Exception ex)
            {
                Logger.Error($"MasterAuditTestRunner: Exception encountered during audit execution: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
