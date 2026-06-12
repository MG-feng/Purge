using System;
using System.Collections.Generic;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.systems
{
    public class abilitysystem
    {
        private game1 _game;
        private gameconfig _config;

        public abilitysystem(game1 game)
        {
            _game = game;
            _config = game.config;
        }

        public void update(float dt, player player, list<enemy> enemies)
        {
            // 更新主动技能冷却
            for (int i = 0; i < player.activecooldowns.length; i++)
            {
                if (player.activecooldowns[i] > 0)
                    player.activecooldowns[i] -= dt;
            }

            // 更新虹吸效果
            if (player.siphonactive)
            {
                player.siphonimer -= dt;
                if (player.siphonimer <= 0)
                    player.siphonactive = false;
            }

            // 更新眩晕
            foreach (var e in enemies)
            {
                if (e.stunned)
                {
                    e.stunimer -= dt;
                    if (e.stunimer <= 0)
                    {
                        e.stunned = false;
                        e.speed = e.originalspeed;
                    }
                }
            }
        }

        public bool useskill(string skillid, player player, list<enemy> enemies, ref int bits)
        {
            return skillid switch
            {
                "heal" => useheal(player),
                "harvest" => useharvest(enemies),
                "siphon" => usesiphon(player),
                "forcefield" => useforcefield(player, enemies),
                "degradation" => usedegradation(enemies),
                _ => false
            };
        }

        private bool useheal(player player)
        {
            if (player.health >= player.maxhealth)
                return false;

            player.heal(_config.heal_amount);
            system.console.writeline("heal used! restored 25 hp");
            return true;
        }

        private bool useharvest(list<enemy> enemies)
        {
            int killcount = 0, damagecount = 0;

            for (int i = enemies.count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                var healthpercent = e.health / e.maxhealth;

                if (healthpercent < _config.harvest_healthpercent)
                {
                    e.health = 0;
                    killcount++;
                }
                else
                {
                    e.takedamage(_config.harvest_damage);
                    if (e.health <= 0)
                        killcount++;
                    else
                        damagecount++;
                }
            }

            system.console.writeline($"harvest used! executed {killcount} enemies, damaged {damagecount} enemies");
            return true;
        }

        private bool usesiphon(player player)
        {
            if (player.siphonactive)
                return false;

            player.siphonactive = true;
            player.siphonimer = _config.siphon_duration;
            system.console.writeline("siphon activated! 50% damage converted to hp for 10s");
            return true;
        }

        private bool useforcefield(player player, list<enemy> enemies)
        {
            int stunnedcount = 0;
            var center = player.center;

            foreach (var e in enemies)
            {
                if (vector2.distance(e.center, center) < _config.forcefield_radius)
                {
                    var dir = e.center - center;
                    if (dir != vector2.zero)
                    {
                        dir.normalize();
                        e.position += dir * _config.forcefield_pushforce;
                    }

                    e.stunned = true;
                    e.stunimer = _config.forcefield_stunduration;
                    e.originalspeed = e.speed;
                    e.speed = 0;
                    stunnedcount++;
                }
            }

            system.console.writeline($"force field used! stunned {stunnedcount} enemies");
            return true;
        }

        private bool usedegradation(list<enemy> enemies)
        {
            int convertedcount = 0, bossconverted = 0;

            for (int i = enemies.count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                var healthpercent = e.health / e.maxhealth;

                if (e.type.contains("boss"))
                {
                    if (e.type != "boss_easy")
                    {
                        // 简化版：直接修改属性而不是重新创建
                        e.type = "boss_easy";
                        e.name = "es-boss";
                        e.maxhealth = 300;
                        e.health = math.max(1, (int)(300 * healthpercent));
                        e.damage = 40;
                        e.speed = 70;
                        e.color = color.purple;
                        bossconverted++;
                        convertedcount++;
                    }
                }
                else
                {
                    e.type = "tank";
                    e.name = "tank";
                    e.maxhealth = 80;
                    e.health = math.max(1, (int)(80 * healthpercent));
                    e.damage = 30;
                    e.speed = 90;
                    e.color = color.darkred;
                    convertedcount++;
                }
            }

            system.console.writeline($"degradation used! converted {convertedcount} enemies, including {bossconverted} bosses");
            return true;
        }

        public void applypassiveeffects(player player)
        {
            if (player.ownedskills.contains("dragonslayer"))
            {
                player.hasdragonslayer = true;
                system.console.writeline("dragon slayer activated");
            }

            if (player.ownedskills.contains("bloodthirst"))
            {
                player.hasbloodthirst = true;
                system.console.writeline("bloodthirst activated");
            }

            if (player.ownedskills.contains("constant"))
            {
                player.hasconstantmotion = true;
                player.walkspeed = _config.constantmotion_speed;
                player.runspeed = _config.constantmotion_speed;
                system.console.writeline("constant motion activated");
            }
        }

        public void drawui(spritebatch spritebatch, player player, texture2d pixel)
        {
            // UI绘制在uimanager中实现
        }
    }
}
