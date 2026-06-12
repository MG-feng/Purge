using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.systems
{
    public class wavesystem
    {
        private game1 _game;
        private gameconfig _config;
        private random _random = new random();

        public int currentwave = 1;
        public bool isactive = false;
        public int enemiestospawn = 0;
        public float waveimer = 0f;
        public float wavebreaktime = 5f;
        public bool isbosswave = false;
        public string mode = "easy";
        public int reward = 0;

        private list<queuedenemy> _spawnqueue = new list<queuedenemy>();

        public wavesystem(game1 game)
        {
            _game = game;
            _config = game.config;
            wavebreaktime = _config.wave_break_time;
        }

        public void start(int wavenum, string gamemode)
        {
            currentwave = wavenum;
            mode = gamemode;
            isactive = true;
            isbosswave = false;

            var config = getwaveconfig(mode, currentwave);
            if (config != null)
            {
                enemiestospawn = 0;
                _spawnqueue.clear();

                foreach (var enemy in config.enemies)
                {
                    enemiestospawn += enemy.count;
                    for (int i = 0; i < enemy.count; i++)
                    {
                        var delay = i * (0.5f + math.max(0.1f, 1 - (currentwave - 1) * 0.02f));
                        _spawnqueue.add(new queuedenemy { enemytype = enemy.type, delay = delay, imer = 0 });
                    }
                }

                reward = config.reward;
            }

            _game.throwablesystem.onwavestart(currentwave);
            system.console.writeline($"=== wave {currentwave}{(isbosswave ? " boss" : "")} start ===");
        }

        public void update(float dt, list<enemy> enemies, ref list<enemybullet> enemybullets)
        {
            // 处理生成队列
            for (int i = _spawnqueue.count - 1; i >= 0; i--)
            {
                var q = _spawnqueue[i];
                q.imer += dt;
                if (q.imer >= q.delay)
                {
                    spawnenemy(q.enemytype, enemies);
                    _spawnqueue.removeat(i);
                }
            }

            // 处理波次
            if (isactive)
            {
                if (enemies.count == 0 && _spawnqueue.count == 0)
                    endwave();
            }
            else
            {
                var maxwaves = getmaxwaves(mode);
                if (currentwave < maxwaves)
                {
                    waveimer -= dt;
                    if (waveimer <= 0)
                        nextwave();
                }
            }

            // 更新敌人
            for (int i = enemies.count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                if (e.health <= 0)
                {
                    _game.objectpool.returnenemy(e);
                    enemies.removeat(i);
                    continue;
                }

                e.updateshield(dt);
                updateenemyai(e, dt, enemies, enemybullets);
            }
        }

        private void updateenemyai(enemy e, float dt, list<enemy> enemies, list<enemybullet> enemybullets)
        {
            var playercenter = _game.player.center;

            // 射击
            if (e.shootcooldown > 0)
            {
                e.shootimer += dt;
                if (e.shootimer >= e.shootcooldown)
                {
                    e.shootimer = 0;
                    shootenemybullet(e, enemybullets);
                }
            }

            // 治疗者
            if (e.type == "healer")
            {
                e.healtimer += dt;
                if (e.healtimer >= e.healcooldown)
                {
                    e.healtimer = 0;
                    foreach (var other in enemies)
                    {
                        if (other != e && vector2.distance(other.center, e.center) < e.healradius)
                        {
                            other.health = math.min(other.maxhealth, other.health + e.healamount);
                        }
                    }
                }
            }

            // 召唤者
            if (e.type == "summoner")
            {
                e.summonimer += dt;
                if (e.summonimer >= e.summoncooldown)
                {
                    e.summonimer = 0;
                    spawnenemy("basic", enemies);
                }
            }

            // 幽灵相位转换
            if (e.type == "ghost")
            {
                e.phasetimer += dt;
                if (e.phasetimer >= 2f)
                {
                    e.phasetimer = 0;
                    e.phaseshift = !e.phaseshift;
                    e.invulnerable = e.phaseshift;
                    e.color = e.phaseshift ? color.purple * 0.3f : color.purple;
                }
            }

            // 移动
            if (!e.stunned && !e.invulnerable)
            {
                var dir = playercenter - e.center;
                if (dir != vector2.zero)
                    dir.normalize();
                e.position += dir * e.speed * dt;
            }

            // 接触伤害
            if (vector2.distance(e.center, playercenter) < 20 && e.damage > 0)
            {
                _game.player.takedamage(e.damage);
            }
        }

        private void shootenemybullet(enemy e, list<enemybullet> enemybullets)
        {
            var bullet = new enemybullet();
            var dir = _game.player.center - e.center;
            if (dir != vector2.zero)
                dir.normalize();
            bullet.initialize(e.center, dir, e.bulletspeed, e.bulletdamage, e.bulletsize, e.bulletcolor);
            enemybullets.add(bullet);
        }

        private void spawnenemy(string enemytype, list<enemy> enemies)
        {
            var mult = _config.getdifficultymultiplier(mode);
            var screenw = _game.graphicsdevice.viewport.width;
            var screenh = _game.graphicsdevice.viewport.height;
            var side = _random.next(1, 5);
            var off = _random.next(100, 200);
            float x = 0, y = 0;

            switch (side)
            {
                case 1: x = _game.player.position.x - screenw / 2 - off; y = _game.player.position.y + _random.next(-screenh / 2, screenh / 2); break;
                case 2: x = _game.player.position.x + screenw / 2 + off; y = _game.player.position.y + _random.next(-screenh / 2, screenh / 2); break;
                case 3: x = _game.player.position.x + _random.next(-screenw / 2, screenw / 2); y = _game.player.position.y - screenh / 2 - off; break;
                default: x = _game.player.position.x + _random.next(-screenw / 2, screenw / 2); y = _game.player.position.y + screenh / 2 + off; break;
            }

            var enemy = _game.objectpool.getenemy(enemytype, x, y, mult.health, mult.speed);
            if (enemy != null)
            {
                if (currentwave >= 5 && !isbosswave)
                {
                    var shieldchance = mode == "hard" ? 0.4f : (mode == "medium" ? 0.3f : 0.2f);
                    enemy.applyshield(shieldchance);
                }
                if (isbosswave)
                    enemy.applyshield(1f);

                enemies.add(enemy);
            }
        }

        private void endwave()
        {
            isactive = false;
            waveimer = wavebreaktime;

            _game.addbits(reward);

            if (isbosswave && mode != "bossrush")
            {
                var healthpercent = _game.player.health / _game.player.maxhealth;
                _game.addbits((int)(750 * healthpercent));
            }

            system.console.writeline($"wave {currentwave} completed");
        }

        private void nextwave()
        {
            var maxwaves = getmaxwaves(mode);
            if (currentwave < maxwaves)
            {
                start(currentwave + 1, mode);
            }
            else
            {
                isactive = false;
                system.console.writeline($"=== game complete! all {maxwaves} waves completed ===");
            }
        }

        private waveconfig getwaveconfig(string mode, int wave)
        {
            // 简化版波次配置 - 完整版应从配置加载
            if (mode == "easy")
            {
                if (wave == 1) return new waveconfig { enemies = new list<(string type, int count)> { ("basic", 4) }, reward = 25 };
                if (wave == 2) return new waveconfig { enemies = new list<(string, int)> { ("basic", 5), ("fast", 1) }, reward = 25 };
                if (wave == 10) return new waveconfig { enemies = new list<(string, int)> { ("boss_easy", 1) }, reward = 100 };
            }
            else if (mode == "medium")
            {
                if (wave == 1) return new waveconfig { enemies = new list<(string, int)> { ("basic", 5), ("fast", 2) }, reward = 30 };
                if (wave == 15) return new waveconfig { enemies = new list<(string, int)> { ("boss_medium", 1), ("fast", 5) }, reward = 200 };
            }
            else if (mode == "hard")
            {
                if (wave == 1) return new waveconfig { enemies = new list<(string, int)> { ("basic", 5), ("fast", 3) }, reward = 40 };
                if (wave == 20) return new waveconfig { enemies = new list<(string, int)> { ("boss_hard", 1), ("fast", 5), ("tank", 10) }, reward = 400 };
            }

            return null;
        }

        private int getmaxwaves(string mode)
        {
            return mode switch
            {
                "easy" => 10,
                "medium" => 15,
                "hard" => 20,
                "endless" => 100,
                "bossrush" => 8,
                _ => 10
            };
        }

        public int getremainingenemies(list<enemy> enemies) => enemies.count + _spawnqueue.count;

        private class queuedenemy
        {
            public string enemytype;
            public float delay;
            public float imer;
        }
    }

    public class waveconfig
    {
        public list<(string type, int count)> enemies = new list<(string, int)>();
        public int reward = 0;
    }
}
