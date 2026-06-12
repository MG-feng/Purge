using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace purge_v0_4_0.game.script.data
{
    public class bestiarydata
    {
        public List<enemyentry> enemies = new List<enemyentry>();
        public List<weaponentry> weapons = new List<weaponentry>();
        public List<modentry> mods = new List<modentry>();
        public List<abilityentry> abilities = new List<abilityentry>();
        public List<difficultyentry> difficulties = new List<difficultyentry>();

        public bestiarydata()
        {
            // 敌人数据
            enemies.add(new enemyentry { name = "basic", description = "最常见的错误代码，攻击力一般，移动速度一般。", health = 30, damage = 10, speed = 180, score = 5, Color = Color.red });
            enemies.add(new enemyentry { name = "fast", description = "速度很快的错误代码，虽然血量较低但难以击中。", health = 20, damage = 8, speed = 300, score = 10, Color = Color.orange });
            enemies.add(new enemyentry { name = "tank", description = "高血量的重型错误代码，移动缓慢但伤害高。", health = 80, damage = 30, speed = 90, score = 20, Color = Color.darkred });
            enemies.add(new enemyentry { name = "elite", description = "精英级错误代码，中等难度中出现。各项属性均衡且较高。", health = 80, damage = 20, speed = 220, score = 30, Color = Color.magenta });
            enemies.add(new enemyentry { name = "assault", description = "突击型错误代码，困难难度中出现。速度快且伤害高。", health = 50, damage = 15, speed = 280, score = 20, Color = Color.crimson });
            enemies.add(new enemyentry { name = "healer", description = "治疗周围友军，优先消灭。", health = 50, damage = 5, speed = 120, score = 30, Color = Color.green });
            enemies.add(new enemyentry { name = "boss_easy", description = "终极错误代码，第10波出现。拥有极高的血量和伤害。", health = 300, damage = 40, speed = 70, score = 75, Color = Color.purple });
            enemies.add(new enemyentry { name = "boss_medium", description = "中等难度下的Boss，血量更高，会发射多颗子弹。", health = 700, damage = 60, speed = 65, score = 150, Color = Color.violet });
            enemies.add(new enemyentry { name = "boss_hard", description = "困难难度Boss，拥有多阶段攻击模式。", health = 1500, damage = 80, speed = 60, score = 200, Color = Color.tomato });

            // 武器数据
            weapons.add(new weaponentry { name = "pistol", description = "初始武器。射速快，弹药充足，适合清理小怪。", damage = "10", firerate = "0.3秒 (≈3发/秒)", ammo = "7", bulletspeed = "800", Color = Color.white });
            weapons.add(new weaponentry { name = "rifle", description = "解锁武器。射速极快，适合对付群体敌人。", damage = "6", firerate = "0.1秒 (≈10发/秒)", ammo = "35", bulletspeed = "1500", Color = Color.gold });
            weapons.add(new weaponentry { name = "sniper", description = "解锁武器。单发伤害高，子弹速度快，适合对付精英和Boss。", damage = "25", firerate = "1.0秒 (1发/秒)", ammo = "5", bulletspeed = "2000", Color = Color.skyblue });
            weapons.add(new weaponentry { name = "soul reaper", description = "终极收割武器。需要500点灵魂充能才能发射。", damage = "750", firerate = "5.0秒", ammo = "充能系统", bulletspeed = "3000", Color = Color.purple });
            weapons.add(new weaponentry { name = "laser gun", description = "高科技激光武器。发射即中激光，4秒不攻击后每秒恢复10点充能。", damage = "15", firerate = "0.1秒 (10发/秒)", ammo = "充能50", bulletspeed = "即时命中", Color = Color.red });
            weapons.add(new weaponentry { name = "feast", description = "饕宴武器。射出一个巨大的子弹，吸附附近敌人到中心，每秒造成伤害，5秒后爆炸。", damage = "25", firerate = "1.5秒", ammo = "1", bulletspeed = "100", Color = Color.orange });
            weapons.add(new weaponentry { name = "life drain", description = "噬命武器。三连发，每击杀一名敌人永久提升1点伤害。", damage = "10", firerate = "1.0秒 (3连发)", ammo = "30", bulletspeed = "1750", Color = Color.crimson });

            // 技能数据
            abilities.add(new abilityentry { name = "heal", type = "主动技能", description = "使用后回复25点生命值", price = 1500, cooldown = 30, effect = "立即回复25点生命值", Color = Color.green });
            abilities.add(new abilityentry { name = "harvest", type = "主动技能", description = "杀死血量低于5%的敌人，其他敌人受到伤害", price = 4000, cooldown = 90, effect = "处决低血量敌人", Color = Color.orange });
            abilities.add(new abilityentry { name = "siphon", type = "主动技能", description = "10秒内，恢复所造成伤害的一半血量", price = 6000, cooldown = 80, duration = 10, effect = "造成伤害的50%转化为生命值", Color = Color.violet });
            abilities.add(new abilityentry { name = "force field", type = "主动技能", description = "弹开周围敌人并使其无法移动1.5秒", price = 2000, cooldown = 30, effect = "击退并眩晕周围敌人", Color = Color.cyan });
            abilities.add(new abilityentry { name = "degradation", type = "主动技能", description = "将场上所有敌人转化为tank，Boss转化为easy_boss", price = 10000, cooldown = 150, effect = "转化所有敌人类型", Color = Color.darkred });
            abilities.add(new abilityentry { name = "dragon slayer", type = "被动技能", description = "对Boss造成的伤害+10%", price = 3000, effect = "对Boss伤害提高10%", Color = Color.gold });
            abilities.add(new abilityentry { name = "bloodthirst", type = "被动技能", description = "每杀死一名敌人恢复1点血量，杀死Boss恢复20点", price = 8000, effect = "击杀回血", Color = Color.red });
            abilities.add(new abilityentry { name = "constant motion", type = "被动技能", description = "无法跑步，但速度提升至260，不消耗体力", price = 3000, effect = "恒定速度260", Color = Color.yellow });

            // 难度数据
            difficulties.add(new difficultyentry { name = "easy", description = "适合新手。敌人属性基础，没有特殊单位。", enemystats = "基础血量、速度", specialenemies = "无", rewardmultiplier = "1x" });
            difficulties.add(new difficultyentry { name = "medium", description = "适合有一定经验的玩家。敌人更强，出现精英敌人。", enemystats = "血量+50%，速度+10-20%", specialenemies = "精英敌人", rewardmultiplier = "1.5x" });
            difficulties.add(new difficultyentry { name = "hard", description = "挑战模式。敌人大幅增强，出现突击敌人。", enemystats = "血量+100%，速度+30%", specialenemies = "精英、突击敌人", rewardmultiplier = "2x" });
            difficulties.add(new difficultyentry { name = "endless", description = "无尽模式。敌人越来越强，挑战极限。", enemystats = "逐渐增强", specialenemies = "所有类型", rewardmultiplier = "按波次递增" });
            difficulties.add(new difficultyentry { name = "boss rush", description = "首领连战模式。直接挑战所有Boss。", enemystats = "Boss级", specialenemies = "仅Boss", rewardmultiplier = "高额奖励" });
        }
    }

    public class enemyentry
    {
        public string name = "";
        public string description = "";
        public int health = 0;
        public int damage = 0;
        public int speed = 0;
        public int score = 0;
        public Color Color = Color.white;
    }

    public class weaponentry
    {
        public string name = "";
        public string description = "";
        public string damage = "";
        public string firerate = "";
        public string ammo = "";
        public string bulletspeed = "";
        public Color Color = Color.white;
    }

    public class modentry
    {
        public string name = "";
        public string weapon = "";
        public string description = "";
        public int price = 0;
        public string icon = "";
    }

    public class abilityentry
    {
        public string name = "";
        public string type = "";
        public string description = "";
        public int price = 0;
        public int cooldown = 0;
        public int duration = 0;
        public string effect = "";
        public Color Color = Color.white;
    }

    public class difficultyentry
    {
        public string name = "";
        public string description = "";
        public string enemystats = "";
        public string specialenemies = "";
        public string rewardmultiplier = "";
    }
}
