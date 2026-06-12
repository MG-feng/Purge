using System;
using System.Collections.Generic;

namespace purge_v0_4_0.game.script.data
{
    [serializable]
    public class savedata
    {
        public int maxbits = 0;

        // 武器
        public bool hasrifle = false;
        public bool hassniper = false;
        public bool hassoulreaper = false;
        public bool haslasergun = false;
        public bool hasfeast = false;
        public bool haslifedrain = false;

        // 手枪模组
        public bool pistol_fastmag = false;
        public bool pistol_extmag = false;
        public bool pistol_damage = false;

        // 步枪模组
        public bool rifle_fastmag = false;
        public bool rifle_extmag = false;
        public bool rifle_damage = false;

        // 狙击枪模组
        public bool sniper_damage = false;
        public bool sniper_pierce = false;

        // 噬魂模组
        public bool soulreaper_pierce = false;
        public bool soulreaper_damage = false;

        // 激光枪模组
        public bool lasergun_capacity = false;

        // 饕宴模组
        public bool feast_dualcore = false;
        public bool feast_highexplosive = false;

        // 噬命模组
        public bool lifedrain_extendedmag = false;
        public bool lifedrain_soulnourish = false;

        // 角色模组
        public bool speedwalk = false;
        public bool speedrun = false;

        // 投掷物
        public bool gravityanchor = false;
        public bool soulshard = false;
        public bool phasebeacon = false;

        // 武器槽位
        public string[] weaponslots = new string[4];

        // 技能
        public string[] ownedskills = new string[0];
        public string[] activeskillslots = new string[4];
        public string[] passiveskillslots = new string[2];

        // 投掷物槽位
        public string[] throwableslots = new string[3];
        public int[] throwablecharges = new int[3];
        public int[] throwablelastused = new int[3];

        public savedata()
        {
            for (int i = 0; i < weaponslots.length; i++)
                weaponslots[i] = null;
            for (int i = 0; i < activeskillslots.length; i++)
                activeskillslots[i] = null;
            for (int i = 0; i < passiveskillslots.length; i++)
                passiveskillslots[i] = null;
            for (int i = 0; i < throwableslots.length; i++)
                throwableslots[i] = null;
        }
    }
}
