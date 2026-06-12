using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using purge_v0_4_0.game.script.core;

namespace purge_v0_4_0.game.script.entities
{
    public class enemy
    {
        public string type = "basic";
        public string name = "basic";
        public float size = 20f;
        public float speed = 180f;
        public float health = 30f;
        public float maxhealth = 30f;
        public color color = color.red;
        public float damage = 10f;
        public int score = 5;
        public vector2 position = vector2.zero;

        public bool canhaveshield = true;
        public float shield = 0f;
        public float maxshield = 0f;
        public float shieldregenrate = 0.01f;
        public float lastdamagetime = 0f;

        public bool stunned = false;
        public float stunimer = 0f;
        public float originalspeed = 180f;

        public bool invulnerable = false;
        public bool slowed = false;

        // 射击相关
        public float shootimer = 0f;
        public float shootcooldown = 0f;
        public float bulletspeed = 400f;
        public float bulletsize = 8f;
        public color bulletcolor = color.magenta;
        public float bulletdamage = 20f;

        // 特殊属性
        public bool exploding = false;
        public float explodeimer = 0f;
        public float explosionrange = 50f;
        public float healradius = 100f;
        public float healamount = 5f;
        public float healcooldown = 2f;
        public float healtimer = 0f;
        public float summoncooldown = 5f;
        public float summonimer = 0f;
        public bool phaseshift = false;
        public float phasetimer = 0f;
        public int shielddirection = 1;
        public float teleportcooldown = 4f;
        public float teleportimer = 0f;
        public float invulnerableimer = 0f;

        public vector2 center => position + new vector2(size / 2, size / 2);

        public enemy()
        {
        }

        public void reset()
        {
            stunned = false;
            slowed = false;
            invulnerable = false;
            exploding = false;
            shield = 0;
            health = maxhealth;
            shootimer = 0;
            healtimer = 0;
            summonimer = 0;
            phasetimer = 0;
            teleportimer = 0;
            invulnerableimer = 0;
        }

        public void initialize(string enemytype, float x, float y, float healthmult, float speedmult)
        {
            type = enemytype;
            position = new vector2(x, y);

            var basedata = getbasedata(enemytype);
            if (basedata != null)
            {
                name = basedata.name;
                size = basedata.size;
                speed = basedata.speed * speedmult;
                maxhealth = basedata.health * healthmult;
                health = maxhealth;
                color = basedata.color;
                damage = basedata.damage;
                score = basedata.score;
                canhaveshield = basedata.canhaveshield;

                shootcooldown = basedata.shootcooldown;
                bulletspeed = basedata.bulletspeed;
                bulletsize = basedata.bulletsize;
                bulletcolor = basedata.bulletcolor;
                bulletdamage = basedata.bulletdamage;

                explosionrange = basedata.explosionrange;
                healradius = basedata.healradius;
                healamount = basedata.healamount;
                healcooldown = basedata.healcooldown;
                summoncooldown = basedata.summoncooldown;
            }

            originalspeed = speed;
        }

        private enemydata getbasedata(string enemytype)
        {
            return enemytype switch
            {
                "basic" => new enemydata { name = "basic", size = 20, speed = 180, health = 30, color = color.red, damage = 10, score = 5, canhaveshield = true },
                "fast" => new enemydata { name = "fast", size = 15, speed = 300, health = 20, color = color.orange, damage = 8, score = 10, canhaveshield = true },
                "tank" => new enemydata { name = "tank", size = 30, speed = 90, health = 80, color = color.darkred, damage = 30, score = 20, canhaveshield = true },
                "elite" => new enemydata { name = "elite", size = 25, speed = 220, health = 80, color = color.magenta, damage = 20, score = 30, canhaveshield = true, shootcooldown = 1.5f, bulletspeed = 400, bulletsize = 6, bulletcolor = color.magenta, bulletdamage = 20 },
                "assault" => new enemydata { name = "assault", size = 20, speed = 280, health = 50, color = color.crimson, damage = 15, score = 20, canhaveshield = true, shootcooldown = 0.8f, bulletspeed = 500, bulletsize = 5, bulletcolor = color.crimson, bulletdamage = 15 },
                "healer" => new enemydata { name = "healer", size = 25, speed = 120, health = 50, color = color.green, damage = 5, score = 30, canhaveshield = true, healradius = 100, healamount = 5, healcooldown = 2 },
                "shielder" => new enemydata { name = "shielder", size = 28, speed = 150, health = 70, color = color.cornflowerblue, damage = 15, score = 25, canhaveshield = true },
                "sniper" => new enemydata { name = "sniper", size = 20, speed = 80, health = 40, color = color.purple, damage = 35, score = 30, canhaveshield = true, shootcooldown = 3, bulletspeed = 800, bulletsize = 6, bulletcolor = color.magenta, bulletdamage = 35 },
                "summoner" => new enemydata { name = "summoner", size = 30, speed = 100, health = 60, color = color.orange, damage = 10, score = 40, canhaveshield = true, summoncooldown = 5 },
                "boss_easy" => new enemydata { name = "es-boss", size = 50, speed = 70, health = 300, color = color.purple, damage = 40, score = 100, canhaveshield = true, shootcooldown = 2, bulletspeed = 400, bulletsize = 8, bulletcolor = color.magenta, bulletdamage = 15 },
                "boss_medium" => new enemydata { name = "md-boss", size = 60, speed = 65, health = 700, color = color.violet, damage = 60, score = 150, canhaveshield = true, shootcooldown = 1.5f, bulletspeed = 500, bulletsize = 10, bulletcolor = color.hotpink, bulletdamage = 20 },
                "boss_hard" => new enemydata { name = "hd-boss", size = 70, speed = 60, health = 1500, color = color.tomato, damage = 80, score = 200, canhaveshield = true, shootcooldown = 1.2f, bulletspeed = 600, bulletsize = 12, bulletcolor = color.red, bulletdamage = 25 },
                _ => new enemydata { name = "basic", size = 20, speed = 180, health = 30, color = color.red, damage = 10, score = 5, canhaveshield = true }
            };
        }

        public void takedamage(float damageamount)
        {
            if (invulnerable) return;

            if (shield > 0)
            {
                shield -= damageamount;
                if (shield < 0)
                {
                    health += shield;
                    shield = 0;
                }
            }
            else
            {
                health -= damageamount;
            }

            lastdamagetime = (float)datetime.now.timeofday.totalseconds;

            if (health < 0) health = 0;
        }

        public void applyshield(float chance)
        {
            if (canhaveshield && new random().nextdouble() < chance)
            {
                shield = maxhealth * 0.75f;
                maxshield = shield;
            }
        }

        public void updateshield(float dt)
        {
            if (shield > 0 && shield < maxshield)
            {
                var timesincedamage = (float)datetime.now.timeofday.totalseconds - lastdamagetime;
                if (timesincedamage > 3.0f)
                {
                    shield += maxshield * shieldregenrate * dt;
                    if (shield > maxshield) shield = maxshield;
                }
            }
        }

        public void draw(spritebatch spritebatch, texture2d pixel, texture2d circlepixel)
        {
            var drawcolor = stunned ? color * 0.5f : color;
            primitives2d.drawrectangle(spritebatch, new rectangle((int)position.x, (int)position.y, (int)size, (int)size), drawcolor, 2);

            var healthpercent = health / maxhealth;
            primitives2d.fillrectangle(spritebatch, new rectangle((int)position.x, (int)position.y - 10, (int)(size * healthpercent), 3), color.red);

            if (shield > 0)
            {
                var shieldpercent = shield / maxshield;
                primitives2d.fillrectangle(spritebatch, new rectangle((int)position.x, (int)position.y - 15, (int)(size * shieldpercent), 3), color.cyan);
            }
        }
    }

    public class enemydata
    {
        public string name = "";
        public float size = 20f;
        public float speed = 180f;
        public float health = 30f;
        public color color = color.red;
        public float damage = 10f;
        public int score = 5;
        public bool canhaveshield = true;
        public float shootcooldown = 0f;
        public float bulletspeed = 400f;
        public float bulletsize = 8f;
        public color bulletcolor = color.magenta;
        public float bulletdamage = 20f;
        public float explosionrange = 0f;
        public float healradius = 0f;
        public float healamount = 0f;
        public float healcooldown = 0f;
        public float summoncooldown = 0f;
    }
}
