using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace purge_v0_4_0.game.script.data
{
    public class waveconfigdata
    {
        public Dictionary<string, waveconfigbymode> modes = new Dictionary<string, waveconfigbymode>();

        public waveconfigdata()
        {
            modes["easy"] = new waveconfigbymode();
            modes["medium"] = new waveconfigbymode();
            modes["hard"] = new waveconfigbymode();
            modes["endless"] = new waveconfigbymode();
            modes["bossrush"] = new waveconfigbymode();

            // 简单模式配置
            modes["easy"].waves.add(new waveconfig { wave = 1, enemies = new List<(string, int)> { ("basic", 4) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 2, enemies = new List<(string, int)> { ("basic", 5), ("fast", 1) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 3, enemies = new List<(string, int)> { ("basic", 7), ("fast", 2), ("tank", 1) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 4, enemies = new List<(string, int)> { ("basic", 10), ("fast", 4), ("tank", 2) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 5, enemies = new List<(string, int)> { ("elite", 1), ("basic", 3), ("fast", 2) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 6, enemies = new List<(string, int)> { ("basic", 6), ("fast", 3), ("tank", 2) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 7, enemies = new List<(string, int)> { ("basic", 5), ("fast", 4), ("elite", 1) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 8, enemies = new List<(string, int)> { ("basic", 7), ("fast", 3), ("tank", 3) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 9, enemies = new List<(string, int)> { ("basic", 6), ("fast", 4), ("elite", 2) }, reward = 25 });
            modes["easy"].waves.add(new waveconfig { wave = 10, enemies = new List<(string, int)> { ("boss_easy", 1) }, reward = 100 });

            // 中等模式配置
            modes["medium"].waves.add(new waveconfig { wave = 1, enemies = new List<(string, int)> { ("basic", 5), ("fast", 2) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 2, enemies = new List<(string, int)> { ("basic", 7), ("fast", 3) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 3, enemies = new List<(string, int)> { ("basic", 8), ("fast", 3), ("tank", 1) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 4, enemies = new List<(string, int)> { ("basic", 10), ("fast", 4), ("tank", 2) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 5, enemies = new List<(string, int)> { ("elite", 2), ("basic", 5), ("fast", 2) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 6, enemies = new List<(string, int)> { ("basic", 7), ("fast", 4), ("tank", 2) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 7, enemies = new List<(string, int)> { ("basic", 6), ("fast", 5), ("elite", 1) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 8, enemies = new List<(string, int)> { ("basic", 8), ("fast", 4), ("tank", 3) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 9, enemies = new List<(string, int)> { ("basic", 7), ("fast", 5), ("elite", 2) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 10, enemies = new List<(string, int)> { ("boss_easy", 1) }, reward = 150 });
            modes["medium"].waves.add(new waveconfig { wave = 11, enemies = new List<(string, int)> { ("basic", 8), ("fast", 5), ("tank", 4) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 12, enemies = new List<(string, int)> { ("basic", 7), ("fast", 6), ("elite", 2) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 13, enemies = new List<(string, int)> { ("basic", 9), ("fast", 5), ("tank", 4), ("elite", 1) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 14, enemies = new List<(string, int)> { ("basic", 8), ("fast", 6), ("elite", 3) }, reward = 30 });
            modes["medium"].waves.add(new waveconfig { wave = 15, enemies = new List<(string, int)> { ("boss_medium", 1), ("fast", 5) }, reward = 200 });

            // 困难模式配置
            modes["hard"].waves.add(new waveconfig { wave = 1, enemies = new List<(string, int)> { ("basic", 5), ("fast", 3) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 2, enemies = new List<(string, int)> { ("basic", 6), ("fast", 4), ("tank", 1) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 3, enemies = new List<(string, int)> { ("basic", 7), ("fast", 4), ("tank", 2) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 4, enemies = new List<(string, int)> { ("basic", 6), ("fast", 5), ("elite", 1) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 5, enemies = new List<(string, int)> { ("boss_easy", 1) }, reward = 200 });
            modes["hard"].waves.add(new waveconfig { wave = 6, enemies = new List<(string, int)> { ("basic", 8), ("fast", 5), ("tank", 3), ("elite", 1) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 7, enemies = new List<(string, int)> { ("basic", 7), ("fast", 6), ("tank", 4), ("elite", 2) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 8, enemies = new List<(string, int)> { ("basic", 9), ("fast", 6), ("tank", 4), ("elite", 2) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 9, enemies = new List<(string, int)> { ("basic", 8), ("fast", 7), ("tank", 5), ("elite", 3) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 10, enemies = new List<(string, int)> { ("boss_medium", 1), ("fast", 5) }, reward = 250 });
            modes["hard"].waves.add(new waveconfig { wave = 11, enemies = new List<(string, int)> { ("basic", 10), ("fast", 7), ("tank", 5), ("elite", 3) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 12, enemies = new List<(string, int)> { ("basic", 9), ("fast", 8), ("tank", 6), ("elite", 4) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 13, enemies = new List<(string, int)> { ("basic", 11), ("fast", 8), ("tank", 6), ("elite", 4) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 14, enemies = new List<(string, int)> { ("basic", 10), ("fast", 9), ("tank", 7), ("elite", 5) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 15, enemies = new List<(string, int)> { ("boss_hard", 1) }, reward = 300 });
            modes["hard"].waves.add(new waveconfig { wave = 16, enemies = new List<(string, int)> { ("basic", 12), ("fast", 9), ("tank", 7), ("elite", 5) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 17, enemies = new List<(string, int)> { ("basic", 11), ("fast", 10), ("tank", 8), ("elite", 6) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 18, enemies = new List<(string, int)> { ("basic", 13), ("fast", 10), ("tank", 8), ("elite", 6) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 19, enemies = new List<(string, int)> { ("basic", 12), ("fast", 11), ("tank", 9), ("elite", 7) }, reward = 40 });
            modes["hard"].waves.add(new waveconfig { wave = 20, enemies = new List<(string, int)> { ("boss_hard", 1), ("fast", 5), ("tank", 10) }, reward = 400 });

            // 首领连战模式
            modes["bossrush"].waves.add(new waveconfig { wave = 1, enemies = new List<(string, int)> { ("boss_easy", 1) }, reward = 25 });
            modes["bossrush"].waves.add(new waveconfig { wave = 2, enemies = new List<(string, int)> { ("boss_medium", 1) }, reward = 50 });
            modes["bossrush"].waves.add(new waveconfig { wave = 3, enemies = new List<(string, int)> { ("boss_hard", 1) }, reward = 75 });
        }
    }

    public class waveconfigbymode
    {
        public List<waveconfig> waves = new List<waveconfig>();

        public waveconfig getwave(int wavenum)
        {
            foreach (var w in waves)
            {
                if (w.wave == wavenum)
                    return w;
            }
            return null;
        }
    }

    public class waveconfig
    {
        public int wave = 0;
        public List<(string type, int count)> enemies = new List<(string, int)>();
        public int reward = 0;
    }
}
