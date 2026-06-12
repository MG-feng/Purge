using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using purge_v0_4_0.game.script.core;

namespace purge_v0_4_0.game.script.entities
{
    public class weaponstate
    {
        public string name = "";
        public float damage = 10f;
        public float bulletspeed = 800f;
        public float bulletsize = 5f;
        public float cooldownmax = 0.3f;
        public float ammo = 7f;
        public float maxammo = 7f;
        public bool isreloading = false;
        public float reloadtimer = 0f;
        public float reloadtime = 2f;
        public float distance = 15f;
        public float size = 15f;
        public color color = color.white;
        public bool pierce = false;
        public int maxpierce = 0;
        public float feastduration = 5f;
        public float feastradius = 80f;
        public float feastdamagepersecond = 25f;
        public float feastexplosiondamage = 1000f;
        public int burstcount = 3;
        public float burstdelay = 0.05f;

        public void reset()
        {
            isreloading = false;
            reloadtimer = 0f;
            ammo = maxammo;
        }
    }

    public class player
    {
        private gameconfig _config;

        public vector2 position = vector2.zero;
        public float health = 100f;
        public float maxhealth = 100f;
        public float stamina = 100f;
        public float maxstamina = 100f;
        public float walkspeed = 125f;
        public float runspeed = 250f;
        public float speed = 125f;

        public bool isdead = false;
        public float deathtimer = 0f;
        public float blinktimer = 0f;

        public bool weaponequipped = false;
        public string weapontype = "";

        public string[] weaponslots = new string[4];
        public dictionary<string, weaponstate> weaponstates = new dictionary<string, weaponstate>();

        // 技能相关
        public hashset<string> ownedskills = new hashset<string>();
        public string[] activeskillslots = new string[4];
        public float[] activecooldowns = new float[4];
        public string[] passiveskillslots = new string[2];
        public bool siphonactive = false;
        public float siphonimer = 0f;

        // 被动技能状态
        public bool hasdragonslayer = false;
        public bool hasbloodthirst = false;
        public bool hasconstantmotion = false;

        // 武器相关
        public float soulreapercharge = 0f;
        public float laserguncharge = 50f;
        public float maxlaserguncharge = 50f;
        public float lastlasershottime = 0f;
        public float lifedraindamagebonus = 0f;
        public int lifedrainkillcounter = 0;

        // 背包
        public bool hasrifle = false;
        public bool hassniper = false;
        public bool hassoulreaper = false;
        public bool haslasergun = false;
        public bool hasfeast = false;
        public bool haslifedrain = false;

        // 模组
        public bool pistol_fastmag = false;
        public bool pistol_extmag = false;
        public bool pistol_damage = false;
        public bool rifle_fastmag = false;
        public bool rifle_extmag = false;
        public bool rifle_damage = false;
        public bool sniper_damage = false;
        public bool sniper_pierce = false;
        public bool soulreaper_pierce = false;
        public bool soulreaper_damage = false;
        public bool lasergun_capacity = false;
        public bool feast_dualcore = false;
        public bool feast_highexplosive = false;
        public bool lifedrain_extendedmag = false;
        public bool lifedrain_soulnourish = false;
        public bool speedwalk = false;
        public bool speedrun = false;

        // 投掷物
        public bool gravityanchor = false;
        public bool soulshard = false;
        public bool phasebeacon = false;
        public string[] throwableslots = new string[3];
        public int[] throwablecharges = new int[3];
        public int[] throwablelastused = new int[3];

        public vector2 center => position + new vector2(10, 10);

        public player(gameconfig config)
        {
            _config = config;
            initweaponstates();
        }

        private void initweaponstates()
        {
            weaponstates["pistol"] = new weaponstate
            {
                name = "pistol",
                damage = _config.pistol_base_damage,
                bulletspeed = 800f,
                bulletsize = 5f,
                cooldownmax = _config.pistol_firerate,
                maxammo = _config.pistol_maxammo,
                ammo = _config.pistol_maxammo,
                reloadtime = _config.pistol_reloadtime,
                distance = 15f,
                size = 15f,
                color = color.white
            };

            weaponstates["rifle"] = new weaponstate
            {
                name = "rifle",
                damage = _config.rifle_base_damage,
                bulletspeed = 1500f,
                bulletsize = 7f,
                cooldownmax = _config.rifle_firerate,
                maxammo = _config.rifle_maxammo,
                ammo = _config.rifle_maxammo,
                reloadtime = _config.rifle_reloadtime,
                distance = 20f,
                size = 20f,
                color = color.gold
            };

            weaponstates["sniper"] = new weaponstate
            {
                name = "sniper",
                damage = _config.sniper_base_damage,
                bulletspeed = 3000f,
                bulletsize = 10f,
                cooldownmax = _config.sniper_firerate,
                maxammo = _config.sniper_maxammo,
                ammo = _config.sniper_maxammo,
                reloadtime = _config.sniper_reloadtime,
                distance = 25f,
                size = 25f,
                color = color.skyblue
            };

            weaponstates["soulreaper"] = new weaponstate
            {
                name = "soul reaper",
                damage = _config.soulreaper_base_damage,
                bulletspeed = 3000f,
                bulletsize = 20f,
                cooldownmax = _config.soulreaper_cooldown,
                maxammo = 1,
                ammo = 1,
                reloadtime = _config.soulreaper_cooldown,
                distance = 30f,
                size = 30f,
                color = color.purple,
                pierce = false,
                maxpierce = 0
            };

            weaponstates["lasergun"] = new weaponstate
            {
                name = "laser gun",
                damage = _config.lasergun_base_damage,
                bulletspeed = 0f,
                bulletsize = 5f,
                cooldownmax = 0.1f,
                maxammo = _config.lasergun_maxcharge,
                ammo = _config.lasergun_maxcharge,
                reloadtime = 0f,
                distance = 30f,
                size = 20f,
                color = color.red
            };
            maxlaserguncharge = _config.lasergun_maxcharge;
            laserguncharge = maxlaserguncharge;

            weaponstates["feast"] = new weaponstate
            {
                name = "feast",
                damage = _config.feast_base_damage,
                bulletspeed = 100f,
                bulletsize = 30f,
                cooldownmax = _config.feast_cooldown,
                maxammo = 1,
                ammo = 1,
                reloadtime = 4f,
                distance = 30f,
                size = 35f,
                color = color.orange,
                feastduration = _config.feast_duration,
                feastradius = _config.feast_radius,
                feastdamagepersecond = _config.feast_dot,
                feastexplosiondamage = _config.feast_explosiondamage
            };

            weaponstates["lifedrain"] = new weaponstate
            {
                name = "life drain",
                damage = _config.lifedrain_base_damage,
                bulletspeed = 1750f,
                bulletsize = 8f,
                cooldownmax = 1f,
                maxammo = _config.lifedrain_maxammo,
                ammo = _config.lifedrain_maxammo,
                reloadtime = _config.lifedrain_reloadtime,
                distance = 20f,
                size = 20f,
                color = color.crimson,
                burstcount = 3,
                burstdelay = _config.lifedrain_burstdelay
            };
        }

        public weaponstate getcurrentweapon()
        {
            if (!weaponequipped || string.isnullorempty(weapontype))
                return null;
            return weaponstates.getvalueordefault(weapontype);
        }

        public void takedamage(float damage)
        {
            health -= damage;
            if (health < 0) health = 0;
        }

        public void heal(float amount)
        {
            health = math.min(maxhealth, health + amount);
        }

        public void reset(string difficulty)
        {
            position = vector2.zero;
            isdead = false;
            deathtimer = 0;
            blinktimer = 0;

            var mult = _config.getdifficultymultiplier(difficulty);

            maxhealth = _config.player_default_maxhealth;
            health = maxhealth;
            maxstamina = _config.player_default_maxstamina;
            stamina = maxstamina;
            walkspeed = _config.player_default_walkspeed;
            runspeed = _config.player_default_runspeed;

            if (speedwalk) walkspeed += 50;
            if (speedrun) runspeed += 75;

            soulreapercharge = 0;
            laserguncharge = maxlaserguncharge;
            lastlasershottime = 0;
            lifedraindamagebonus = 0;
            lifedrainkillcounter = 0;

            for (int i = 0; i < activecooldowns.length; i++)
                activecooldowns[i] = 0;
            siphonactive = false;
            siphonimer = 0;

            // 重置武器状态
            foreach (var w in weaponstates.values)
                w.reset();

            // 装备第一个武器
            weaponequipped = false;
            for (int i = 0; i < weaponslots.length; i++)
            {
                if (!string.isnullorempty(weaponslots[i]))
                {
                    weapontype = weaponslots[i];
                    weaponequipped = true;
                    break;
                }
            }

            // 重置投掷物充能
            for (int i = 0; i < throwableslots.length; i++)
            {
                if (!string.isnullorempty(throwableslots[i]))
                    throwablecharges[i] = 1;
                else
                    throwablecharges[i] = 0;
                throwablelastused[i] = 0;
            }
        }

        public void applymods()
        {
            if (pistol_fastmag)
            {
                weaponstates["pistol"].maxammo += 3;
                weaponstates["pistol"].ammo += 3;
            }
            if (pistol_extmag)
                weaponstates["pistol"].reloadtime = math.max(0.5f, weaponstates["pistol"].reloadtime - 0.5f);
            if (pistol_damage)
                weaponstates["pistol"].damage += 3;

            if (rifle_fastmag)
            {
                weaponstates["rifle"].maxammo += 15;
                weaponstates["rifle"].ammo += 15;
            }
            if (rifle_extmag)
                weaponstates["rifle"].reloadtime = math.max(0.5f, weaponstates["rifle"].reloadtime - 0.5f);
            if (rifle_damage)
                weaponstates["rifle"].damage += 2;

            if (sniper_damage)
                weaponstates["sniper"].damage += 10;
            if (sniper_pierce)
                weaponstates["sniper"].pierce = true;

            if (soulreaper_pierce)
            {
                weaponstates["soulreaper"].pierce = true;
                weaponstates["soulreaper"].maxpierce = 3;
            }
            if (soulreaper_damage)
                weaponstates["soulreaper"].damage += 750;

            if (lasergun_capacity)
            {
                maxlaserguncharge += 25;
                laserguncharge = maxlaserguncharge;
                weaponstates["lasergun"].maxammo = maxlaserguncharge;
            }

            if (feast_dualcore)
            {
                weaponstates["feast"].maxammo = 2;
                weaponstates["feast"].ammo = 2;
            }
            if (feast_highexplosive)
            {
                weaponstates["feast"].feastdamagepersecond = 40;
                weaponstates["feast"].feastexplosiondamage = 2222;
            }

            if (lifedrain_extendedmag)
            {
                weaponstates["lifedrain"].maxammo = 50;
                weaponstates["lifedrain"].ammo = 50;
            }
        }

        public void draw(spritebatch spritebatch, texture2d pixel, float angle)
        {
            var color = isdead ? (blinktimer < 0.1f ? color.darkred * 0.7f : color.white * 0.3f) : color.white;
            primitives2d.drawrectangle(spritebatch, new rectangle((int)position.x, (int)position.y, 20, 20), color, 2);

            if (weaponequipped && !isdead && weapontype != "lasergun")
            {
                var weapon = getcurrentweapon();
                if (weapon != null)
                {
                    var wx = position.x + 10 + (float)math.cos(angle) * weapon.distance;
                    var wy = position.y + 10 + (float)math.sin(angle) * weapon.distance;

                    var matrix = matrix.createtranslation(wx, wy, 0) * matrix.createrotationz(angle);
                    spritebatch.end();
                    spritebatch.begin(transformmatrix: matrix);

                    primitives2d.fillrectangle(spritebatch, new rectangle(0, (int)-weapon.size / 2, (int)weapon.size, (int)weapon.size / 2), weapon.color);

                    spritebatch.end();
                    spritebatch.begin();
                }
            }
        }
    }
}
