using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter3
{
    public class Chapter3SaveData
    {
        public List<string> CompletedDungeonRooms { get; set; } = new();
        public List<string> ActivatedCheckpoints { get; set; } = new();
        public BossPhase VarethBossPhase { get; set; } = BossPhase.Intro;
        public bool IsBossDefeated { get; set; } = false;
        public bool IsActIComplete { get; set; } = false;
        public List<string> UnlockedWorldEvents { get; set; } = new();
        public int SaveVersion { get; set; } = 25;
    }
}
