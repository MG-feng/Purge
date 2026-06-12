using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace purge_v0_4_0.game.script.core
{
    public class gameconfig
    {
        // 玩家属性默认值
        public float player_default_health = 100f;
        public float player_default_maxhealth = 100f;
        public float player_default_stamina = 100f;
        public float player_default_maxstamina = 100f;
        public float player_default_walkspeed = 125f;
        public float player_default_runspeed = 250f;

        // 难度倍率
        public float easy_healthmultiplier = 1f;
        public float easy_speedmultiplier = 1f;
        public float easy_scoremultiplier = 1f;

        public float medium_healthmultiplier = 1.5f;
        public float medium_speedmultiplier = 1.15f;
        public float medium_scoremultiplier = 1.5f;

        public float hard_healthmultiplier = 2f;
        public float hard_speedmultiplier = 1.3f;
        public float hard_scoremultiplier = 2f;

        public float endless_healthmultiplier = 1.2f;
        public float endless_speedmultiplier = 1.1f;
        public float endless_scoremultiplier = 1f;

        public float bossrush_healthmultiplier = 1.5f;
        public float bossrush_speedmultiplier = 1.2f;

        // 玩家成长
        public int lifedrain_killforhealth = 25;
        public int lifedrain_healthgain = 1;
        public int bloodthirst_bossheal = 20;
        public int bloodthirst_normalheal = 1;

        // 技能属性
        public float heal_amount = 25f;
        public float heal_cooldown = 30f;

        public float harvest_damage = 50f;
        public float harvest_healthpercent = 0.05f;
        public float harvest_cooldown = 90f;

        public float siphon_duration = 10f;
        public float siphon_lifestealpercent = 0.5f;
        public float siphon_cooldown = 80f;

        public float forcefield_radius = 40f;
        public float forcefield_pushforce = 100f;
        public float forcefield_stunduration = 1.5f;
        public float forcefield_cooldown = 30f;

        public float degradation_cooldown = 150f;

        public float dragonslayer_bonus = 0.1f;

        public float constantmotion_speed = 260f;

        // 投掷物属性
        public float gravityanchor_radius = 100f;
        public float gravityanchor_slowamount = 0.5f;
        public float gravityanchor_duration = 8f;
        public int gravityanchor_cooldownrounds = 2;

        public float soulshard_damage = 300f;
        public float soulshard_radius = 60f;
        public int soulshard_cooldownrounds = 1;

        public float phasebeacon_duration = 5f;
        public int phasebeacon_cooldownrounds = 1;

        // 武器属性
        public int pistol_base_damage = 10;
        public int pistol_maxammo = 7;
        public float pistol_reloadtime = 2f;
        public float pistol_firerate = 0.3f;

        public int rifle_base_damage = 6;
        public int rifle_maxammo = 35;
        public float rifle_reloadtime = 3.4f;
        public float rifle_firerate = 0.1f;

        public int sniper_base_damage = 25;
        public int sniper_maxammo = 5;
        public float sniper_reloadtime = 4f;
        public float sniper_firerate = 1f;

        public int soulreaper_base_damage = 750;
        public int soulreaper_maxcharge = 500;
        public float soulreaper_cooldown = 5f;

        public int lasergun_base_damage = 15;
        public int lasergun_maxcharge = 50;
        public float lasergun_rechargerate = 10f;

        public int feast_base_damage = 25;
        public float feast_cooldown = 1.5f;
        public float feast_duration = 5f;
        public float feast_radius = 80f;
        public float feast_dot = 25f;
        public int feast_explosiondamage = 1000;

        public int lifedrain_base_damage = 10;
        public int lifedrain_maxammo = 30;
        public float lifedrain_reloadtime = 4f;
        public float lifedrain_burstdelay = 0.05f;

        // 对象池上限 (玩家可调节)
        public int max_bulletpool = 500;
        public int max_enemypool = 200;
        public int max_effectpool = 100;

        // 波次配置
        public int map_expand_wave = 30;
        public float wave_break_time = 5f;
        public float boss_bonus_multiplier = 0.75f;

        // 保存加密
        public string save_key = "purge_game_save_key_2024";

        // 获取难度倍率
        public (float health, float speed, float score) getdifficultymultiplier(string difficulty)
        {
            switch (difficulty)
            {
                case "easy": return (easy_healthmultiplier, easy_speedmultiplier, easy_scoremultiplier);
                case "medium": return (medium_healthmultiplier, medium_speedmultiplier, medium_scoremultiplier);
                case "hard": return (hard_healthmultiplier, hard_speedmultiplier, hard_scoremultiplier);
                case "endless": return (endless_healthmultiplier, endless_speedmultiplier, endless_scoremultiplier);
                case "bossrush": return (bossrush_healthmultiplier, bossrush_speedmultiplier, 2f);
                default: return (1f, 1f, 1f);
            }
        }
    }
}
