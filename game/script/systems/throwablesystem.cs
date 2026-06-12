using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.systems
{
    public class throwablesystem
    {
        private game1 _game;
        private gameconfig _config;

        public throwablesystem(game1 game)
        {
            _game = game;
            _config = game.config;
        }

        public void onwavestart(int currentwave)
        {
            var player = _game.player;

            for (int i = 0; i < player.throwableslots.length; i++)
            {
                var id = player.throwableslots[i];
                if (!string.isnullorempty(id))
                {
                    var lastused = player.throwablelastused[i];
                    var cooldownrounds = getcooldownrounds(id);

                    if (lastused > 0)
                    {
                        var roundspassed = currentwave - lastused;
                        if (roundspassed >= cooldownrounds)
                            player.throwablecharges[i] = 1;
                    }
                    else
                    {
                        player.throwablecharges[i] = 1;
                    }
                }
            }
        }

        public bool usethrowable(int slotindex, Vector2 target)
        {
            var player = _game.player;

            if (slotindex < 0 || slotindex >= player.throwableslots.length)
                return false;

            var id = player.throwableslots[slotindex];
            if (string.isnullorempty(id))
                return false;

            if (player.throwablecharges[slotindex] <= 0)
                return false;

            player.throwablecharges[slotindex]--;
            player.throwablelastused[slotindex] = _game.wavesystem.currentwave;

            switch (id)
            {
                case "gravityanchor":
                    creategravityanchor(target);
                    break;
                case "soulshard":
                    createsoulshard(target);
                    break;
                case "phasebeacon":
                    createphasebeacon(target);
                    break;
            }

            return true;
        }

        private void creategravityanchor(Vector2 position)
        {
            var field = _game.objectpool.geteffectfield();
            if (field != null)
            {
                field.type = "gravity";
                field.position = position;
                field.radius = _config.gravityanchor_radius;
                field.duration = _config.gravityanchor_duration;
                field.slowamount = _config.gravityanchor_slowamount;
                field.active = true;
                _game.throwablefields.add(field);
            }
        }

        private void createsoulshard(Vector2 position)
        {
            // 造成伤害
            foreach (var enemy in _game.enemies)
            {
                if (Vector2.distance(enemy.center, position) < _config.soulshard_radius)
                {
                    enemy.takedamage(_config.soulshard_damage);
                }
            }

            // 爆炸特效
            var field = _game.objectpool.geteffectfield();
            if (field != null)
            {
                field.type = "explosion";
                field.position = position;
                field.radius = _config.soulshard_radius;
                field.duration = 0.3f;
                field.active = true;
                _game.throwablefields.add(field);
            }
        }

        private void createphasebeacon(Vector2 position)
        {
            var field = _game.objectpool.geteffectfield();
            if (field != null)
            {
                field.type = "beacon";
                field.position = position;
                field.duration = _config.phasebeacon_duration;
                field.active = true;
                _game.throwablefields.add(field);
            }
        }

        public bool trytriggerbeacon()
        {
            for (int i = 0; i < _game.throwablefields.count; i++)
            {
                var field = _game.throwablefields[i];
                if (field.type == "beacon" && field.active)
                {
                    _game.player.position = field.position - new Vector2(10, 10);
                    _game.throwablefields.removeat(i);
                    _game.objectpool.returneffectfield(field);
                    return true;
                }
            }
            return false;
        }

        public void update(float dt, List<throwablefield> fields, player player, List<enemy> enemies, game1 game)
        {
            for (int i = fields.count - 1; i >= 0; i--)
            {
                var field = fields[i];
                var shouldremove = field.update(dt);

                // 应用效果
                if (field.type == "gravity")
                {
                    foreach (var e in enemies)
                    {
                        if (Vector2.distance(e.center, field.position) < field.radius)
                        {
                            if (!e.slowed)
                            {
                                e.slowed = true;
                                e.originalspeed = e.speed;
                                e.speed *= (1 - field.slowamount);
                            }
                        }
                        else if (e.slowed)
                        {
                            e.slowed = false;
                            e.speed = e.originalspeed;
                        }
                    }
                }

                if (shouldremove)
                {
                    // 清理效果
                    if (field.type == "gravity")
                    {
                        foreach (var e in enemies)
                        {
                            if (e.slowed)
                            {
                                e.slowed = false;
                                e.speed = e.originalspeed;
                            }
                        }
                    }
                    else if (field.type == "beacon" && field.imer < field.duration)
                    {
                        // 自动传送
                        player.position = field.position - new Vector2(10, 10);
                    }

                    fields.removeat(i);
                    _game.objectpool.returneffectfield(field);
                }
            }
        }

        private int getcooldownrounds(string id)
        {
            return id switch
            {
                "gravityanchor" => _config.gravityanchor_cooldownrounds,
                "soulshard" => _config.soulshard_cooldownrounds,
                "phasebeacon" => _config.phasebeacon_cooldownrounds,
                _ => 1
            };
        }

        public void drawui(SpriteBatch SpriteBatch, player player)
        {
            // UI绘制在uimanager中实现
        }
    }
}
