using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.entities;
using purge_v0_4_0.game.script.systems;

namespace purge_v0_4_0.game.script.ui
{
    public class uimanager
    {
        private game1 _game;
        private localization _loc;
        private SpriteFont _font;
        private SpriteFont _smallfont;
        private SpriteFont _largefont;

        private float _healthbarwidth = 0f;
        private float _staminabarwidth = 0f;
        private float _targethealthwidth = 0f;
        private float _targetstaminawidth = 0f;

        public uimanager(game1 game)
        {
            _game = game;
            _loc = game.localization;
        }

        public void loadcontent(SpriteBatch SpriteBatch, Texture2D pixel)
        {
            // 使用默认字体
            _font = SpriteFont.default;
            _smallfont = SpriteFont.default;
            _largefont = SpriteFont.default;
        }

        public SpriteFont getfont() => _font;
        public SpriteFont getsmallfont() => _smallfont;
        public SpriteFont getlargefont() => _largefont;

        public void updatebars(player player)
        {
            var maxwidth = 290f;
            _targethealthwidth = maxwidth * (player.health / player.maxhealth);
            _healthbarwidth = _healthbarwidth + (_targethealthwidth - _healthbarwidth) * 0.15f;

            _targetstaminawidth = maxwidth * (player.stamina / player.maxstamina);
            _staminabarwidth = _staminabarwidth + (_targetstaminawidth - _staminabarwidth) * 0.15f;
        }

        public void draw(SpriteBatch SpriteBatch, player player, wavesystem wavesystem, int bits, int maxbits, int mapwidth, int mapheight, int enemycount)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;
            var margin = 30;

            // 血条
            drawbar(SpriteBatch, margin, h - 35 * 2 - 10 - margin, _healthbarwidth, player.health, player.maxhealth, Color.red, Color.white);

            // 体力条
            var staminaalpha = player.stamina < 30 ? 0.5f : 1f;
            drawbar(SpriteBatch, margin, h - 35 - margin, _staminabarwidth, player.stamina, player.maxstamina, Color.white * staminaalpha, Color.white);

            // 波次信息
            var wavecolor = wavesystem.isbosswave ? Color.red : Color.white;
            if (wavesystem.isactive)
            {
                SpriteBatch.drawstring(_smallfont, $"{_loc.get("wave")} {wavesystem.currentwave}{(wavesystem.isbosswave ? " boss" : "")}", new Vector2(margin, 20), wavecolor);
                SpriteBatch.drawstring(_smallfont, $"{_loc.get("enemies")}: {enemycount}/{wavesystem.enemiestospawn}", new Vector2(margin, 45), Color.white);
            }
            else
            {
                if (wavesystem.currentwave <= wavesystem.getmaxwaves(wavesystem.mode))
                    SpriteBatch.drawstring(_smallfont, $"{_loc.get("next_wave")}: {math.ceiling(wavesystem.waveimer)}s", new Vector2(margin, 20), Color.white);
                else
                    SpriteBatch.drawstring(_smallfont, _loc.get("game_complete"), new Vector2(margin, 20), Color.green);
            }

            SpriteBatch.drawstring(_smallfont, $"{_loc.get("map")}: {mapwidth}x{mapheight}", new Vector2(margin, 70), Color.cornflowerblue);

            // 比特显示
            SpriteBatch.drawstring(_largefont, $"{_loc.get("bits")}: {bits}", new Vector2(w - 250, 20), Color.yellow);
            SpriteBatch.drawstring(_smallfont, $"{_loc.get("wallet")}: {maxbits}", new Vector2(w - 250, 60), Color.lightgray);

            // 武器UI
            drawweaponui(SpriteBatch, player, margin, h);

            // 技能UI
            drawabilityui(SpriteBatch, player, w, h);

            // 投掷物UI
            drawthrowableui(SpriteBatch, player, w, h);
        }

        private void drawbar(SpriteBatch SpriteBatch, int x, int y, float width, float current, float max, Color fillcolor, Color bordercolor)
        {
            var barwidth = 300;
            var barheight = 35;
            var padding = 5;

            primitives2d.drawrectangle(SpriteBatch, new Rectangle(x, y, barwidth, barheight), bordercolor, 2);
            primitives2d.fillrectangle(SpriteBatch, new Rectangle(x + padding, y + padding, (int)width, barheight - padding * 2), fillcolor);

            SpriteBatch.drawstring(_smallfont, $"{math.floor(current)}/{math.floor(max)}", new Vector2(x + barwidth + 10, y), Color.white);
        }

        private void drawweaponui(SpriteBatch SpriteBatch, player player, int margin, int h)
        {
            var weapon = player.getcurrentweapon();
            var ybase = h - 260;

            if (weapon != null)
            {
                SpriteBatch.drawstring(_smallfont, $"{weapon.name} equipped", new Vector2(margin, ybase), weapon.Color);

                if (player.weapontype == "soulreaper")
                {
                    SpriteBatch.drawstring(_smallfont, $"{_loc.get("soul_charge")}: {math.floor(player.soulreapercharge)}/500", new Vector2(margin, ybase + 30), Color.yellow);
                    var notreadycolor = player.soulreapercharge < 500 ? Color.red : Color.green;
                    var notreadytext = player.soulreapercharge < 500 ? _loc.get("not_ready") : _loc.get("ready_to_fire");
                    SpriteBatch.drawstring(_smallfont, notreadytext, new Vector2(margin, ybase + 60), notreadycolor);
                }
                else if (player.weapontype == "lasergun")
                {
                    SpriteBatch.drawstring(_smallfont, $"{_loc.get("laser_charge")}: {math.floor(player.laserguncharge)}/{player.maxlaserguncharge}", new Vector2(margin, ybase + 30), Color.red);
                    SpriteBatch.drawstring(_smallfont, _loc.get("auto_recharges"), new Vector2(margin, ybase + 60), Color.lightgray);
                }
                else if (player.weapontype == "feast")
                {
                    SpriteBatch.drawstring(_smallfont, $"{_loc.get("feast_ammo")}: {weapon.ammo}/{weapon.maxammo}", new Vector2(margin, ybase + 30), Color.orange);
                    if (weapon.isreloading)
                        SpriteBatch.drawstring(_smallfont, $"{_loc.get("reloading")}... {weapon.reloadimer:f1}s", new Vector2(margin, ybase + 60), Color.yellow);
                    else
                        SpriteBatch.drawstring(_smallfont, _loc.get("attracts_enemies"), new Vector2(margin, ybase + 60), Color.lightgray);
                }
                else if (player.weapontype == "lifedrain")
                {
                    SpriteBatch.drawstring(_smallfont, $"{_loc.get("life_drain_ammo")}: {weapon.ammo}/{weapon.maxammo}", new Vector2(margin, ybase + 30), Color.crimson);
                    if (weapon.isreloading)
                        SpriteBatch.drawstring(_smallfont, $"{_loc.get("reloading")}... {weapon.reloadimer:f1}s", new Vector2(margin, ybase + 60), Color.yellow);
                    else
                        SpriteBatch.drawstring(_smallfont, $"{_loc.get("permanent_dmg")}: +{player.lifedraindamagebonus}", new Vector2(margin, ybase + 60), Color.lightgray);
                }
                else
                {
                    if (weapon.isreloading)
                        SpriteBatch.drawstring(_smallfont, $"{_loc.get("reloading")}... {weapon.reloadimer:f1}s", new Vector2(margin, ybase + 30), Color.yellow);
                    else
                        SpriteBatch.drawstring(_smallfont, $"{_loc.get("ammo")}: {weapon.ammo}/{weapon.maxammo}", new Vector2(margin, ybase + 30), Color.white);
                    SpriteBatch.drawstring(_smallfont, _loc.get("press_r_reload"), new Vector2(margin, ybase + 60), Color.lightgray);
                }
            }
            else
            {
                SpriteBatch.drawstring(_smallfont, _loc.get("press_1_4_weapons"), new Vector2(margin, ybase), Color.gray);
                SpriteBatch.drawstring(_smallfont, _loc.get("press_r_reload"), new Vector2(margin, ybase + 30), Color.gray);
            }
        }

        private void drawabilityui(SpriteBatch SpriteBatch, player player, int w, int h)
        {
            var slotsize = 40;
            var spacing = 10;
            var padding = 20;

            var totalactive = slotsize * 4 + spacing * 3;
            var x = w - totalactive - padding - 20;
            var y = h - 200;

            // 主动技能背景
            primitives2d.fillrectangle(SpriteBatch, new Rectangle(x - 10, y - 10, totalactive + 20, slotsize + 20), Color.darkblue * 0.8f, 10);
            SpriteBatch.drawstring(_smallfont, _loc.get("active_skills"), new Vector2(x, y - 25), Color.white);

            var Keys = new[] { "q", "e", "z", "x" };
            for (int i = 0; i < 4; i++)
            {
                var slotx = x + i * (slotsize + spacing);
                var abilityid = player.activeskillslots[i];
                var cooldown = player.activecooldowns[i];

                var bgcolor = abilityid != null ? Color.purple * 0.7f : Color.darkgray * 0.5f;
                primitives2d.fillrectangle(SpriteBatch, new Rectangle(slotx, y, slotsize, slotsize), bgcolor, 5);
                primitives2d.drawrectangle(SpriteBatch, new Rectangle(slotx, y, slotsize, slotsize), Color.white, 2, 5);

                if (abilityid != null)
                {
                    var icon = abilityid.substring(0, 2).toupper();
                    SpriteBatch.drawstring(_smallfont, icon, new Vector2(slotx + 12, y + 12), Color.white);

                    if (cooldown > 0)
                    {
                        var percent = cooldown / 30f;
                        primitives2d.fillrectangle(SpriteBatch, new Rectangle(slotx, y, slotsize, (int)(slotsize * percent)), Color.black * 0.5f, 0);
                        SpriteBatch.drawstring(_smallfont, math.ceiling(cooldown).tostring(), new Vector2(slotx + 14, y + 14), Color.yellow);
                    }
                }
                else
                {
                    SpriteBatch.drawstring(_smallfont, "?", new Vector2(slotx + 15, y + 12), Color.gray);
                }

                SpriteBatch.drawstring(_smallfont, Keys[i], new Vector2(slotx + 15, y - 15), Color.lightgray);
            }

            // 虹吸激活指示
            if (player.siphonactive)
            {
                var alph = 0.5f + 0.5f * (float)math.sin(DateTime.now.timeofday.totalseconds * 5);
                SpriteBatch.drawstring(_smallfont, $"siphon active: {math.ceiling(player.siphonimer)}s", new Vector2(x, y + slotsize + 15), Color.violet * alph);
            }
        }

        private void drawthrowableui(SpriteBatch SpriteBatch, player player, int w, int h)
        {
            var slotsize = 40;
            var spacing = 10;
            var padding = 20;

            var totalwidth = slotsize * 3 + spacing * 2;
            var x = w - totalwidth - padding - 20;
            var y = h - 270;

            primitives2d.fillrectangle(SpriteBatch, new Rectangle(x - 10, y - 10, totalwidth + 20, slotsize + 20), Color.darkblue * 0.8f, 10);
            SpriteBatch.drawstring(_smallfont, _loc.get("throwables"), new Vector2(x, y - 25), Color.white);

            for (int i = 0; i < 3; i++)
            {
                var slotx = x + i * (slotsize + spacing);
                var throwableid = player.throwableslots[i];
                var hascharge = player.throwablecharges[i] > 0;

                var bgcolor = throwableid != null ? (hascharge ? Color.cyan * 0.7f : Color.darkcyan * 0.5f) : Color.darkgray * 0.5f;
                primitives2d.fillrectangle(SpriteBatch, new Rectangle(slotx, y, slotsize, slotsize), bgcolor, 5);
                primitives2d.drawrectangle(SpriteBatch, new Rectangle(slotx, y, slotsize, slotsize), Color.white, 2, 5);

                if (throwableid != null)
                {
                    var icon = throwableid.substring(0, 2).toupper();
                    SpriteBatch.drawstring(_smallfont, icon, new Vector2(slotx + 12, y + 12), Color.white);

                    var chargecolor = hascharge ? Color.green : Color.red;
                    primitives2d.fillcircle(SpriteBatch, new Vector2(slotx + slotsize - 8, y + 8), 4, 6, chargecolor);
                }
                else
                {
                    SpriteBatch.drawstring(_smallfont, "?", new Vector2(slotx + 15, y + 12), Color.gray);
                }

                SpriteBatch.drawstring(_smallfont, (i + 5).tostring(), new Vector2(slotx + 15, y - 15), Color.lightgray);
            }
        }
    }
}
