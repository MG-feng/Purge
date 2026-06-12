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
        private spritefont _font;
        private spritefont _smallfont;
        private spritefont _largefont;

        private float _healthbarwidth = 0f;
        private float _staminabarwidth = 0f;
        private float _targethealthwidth = 0f;
        private float _targetstaminawidth = 0f;

        public uimanager(game1 game)
        {
            _game = game;
            _loc = game.localization;
        }

        public void loadcontent(spritebatch spritebatch, texture2d pixel)
        {
            // 使用默认字体
            _font = spritefont.default;
            _smallfont = spritefont.default;
            _largefont = spritefont.default;
        }

        public spritefont getfont() => _font;
        public spritefont getsmallfont() => _smallfont;
        public spritefont getlargefont() => _largefont;

        public void updatebars(player player)
        {
            var maxwidth = 290f;
            _targethealthwidth = maxwidth * (player.health / player.maxhealth);
            _healthbarwidth = _healthbarwidth + (_targethealthwidth - _healthbarwidth) * 0.15f;

            _targetstaminawidth = maxwidth * (player.stamina / player.maxstamina);
            _staminabarwidth = _staminabarwidth + (_targetstaminawidth - _staminabarwidth) * 0.15f;
        }

        public void draw(spritebatch spritebatch, player player, wavesystem wavesystem, int bits, int maxbits, int mapwidth, int mapheight, int enemycount)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;
            var margin = 30;

            // 血条
            drawbar(spritebatch, margin, h - 35 * 2 - 10 - margin, _healthbarwidth, player.health, player.maxhealth, color.red, color.white);

            // 体力条
            var staminaalpha = player.stamina < 30 ? 0.5f : 1f;
            drawbar(spritebatch, margin, h - 35 - margin, _staminabarwidth, player.stamina, player.maxstamina, color.white * staminaalpha, color.white);

            // 波次信息
            var wavecolor = wavesystem.isbosswave ? color.red : color.white;
            if (wavesystem.isactive)
            {
                spritebatch.drawstring(_smallfont, $"{_loc.get("wave")} {wavesystem.currentwave}{(wavesystem.isbosswave ? " boss" : "")}", new vector2(margin, 20), wavecolor);
                spritebatch.drawstring(_smallfont, $"{_loc.get("enemies")}: {enemycount}/{wavesystem.enemiestospawn}", new vector2(margin, 45), color.white);
            }
            else
            {
                if (wavesystem.currentwave <= wavesystem.getmaxwaves(wavesystem.mode))
                    spritebatch.drawstring(_smallfont, $"{_loc.get("next_wave")}: {math.ceiling(wavesystem.waveimer)}s", new vector2(margin, 20), color.white);
                else
                    spritebatch.drawstring(_smallfont, _loc.get("game_complete"), new vector2(margin, 20), color.green);
            }

            spritebatch.drawstring(_smallfont, $"{_loc.get("map")}: {mapwidth}x{mapheight}", new vector2(margin, 70), color.cornflowerblue);

            // 比特显示
            spritebatch.drawstring(_largefont, $"{_loc.get("bits")}: {bits}", new vector2(w - 250, 20), color.yellow);
            spritebatch.drawstring(_smallfont, $"{_loc.get("wallet")}: {maxbits}", new vector2(w - 250, 60), color.lightgray);

            // 武器UI
            drawweaponui(spritebatch, player, margin, h);

            // 技能UI
            drawabilityui(spritebatch, player, w, h);

            // 投掷物UI
            drawthrowableui(spritebatch, player, w, h);
        }

        private void drawbar(spritebatch spritebatch, int x, int y, float width, float current, float max, color fillcolor, color bordercolor)
        {
            var barwidth = 300;
            var barheight = 35;
            var padding = 5;

            primitives2d.drawrectangle(spritebatch, new rectangle(x, y, barwidth, barheight), bordercolor, 2);
            primitives2d.fillrectangle(spritebatch, new rectangle(x + padding, y + padding, (int)width, barheight - padding * 2), fillcolor);

            spritebatch.drawstring(_smallfont, $"{math.floor(current)}/{math.floor(max)}", new vector2(x + barwidth + 10, y), color.white);
        }

        private void drawweaponui(spritebatch spritebatch, player player, int margin, int h)
        {
            var weapon = player.getcurrentweapon();
            var ybase = h - 260;

            if (weapon != null)
            {
                spritebatch.drawstring(_smallfont, $"{weapon.name} equipped", new vector2(margin, ybase), weapon.color);

                if (player.weapontype == "soulreaper")
                {
                    spritebatch.drawstring(_smallfont, $"{_loc.get("soul_charge")}: {math.floor(player.soulreapercharge)}/500", new vector2(margin, ybase + 30), color.yellow);
                    var notreadycolor = player.soulreapercharge < 500 ? color.red : color.green;
                    var notreadytext = player.soulreapercharge < 500 ? _loc.get("not_ready") : _loc.get("ready_to_fire");
                    spritebatch.drawstring(_smallfont, notreadytext, new vector2(margin, ybase + 60), notreadycolor);
                }
                else if (player.weapontype == "lasergun")
                {
                    spritebatch.drawstring(_smallfont, $"{_loc.get("laser_charge")}: {math.floor(player.laserguncharge)}/{player.maxlaserguncharge}", new vector2(margin, ybase + 30), color.red);
                    spritebatch.drawstring(_smallfont, _loc.get("auto_recharges"), new vector2(margin, ybase + 60), color.lightgray);
                }
                else if (player.weapontype == "feast")
                {
                    spritebatch.drawstring(_smallfont, $"{_loc.get("feast_ammo")}: {weapon.ammo}/{weapon.maxammo}", new vector2(margin, ybase + 30), color.orange);
                    if (weapon.isreloading)
                        spritebatch.drawstring(_smallfont, $"{_loc.get("reloading")}... {weapon.reloadimer:f1}s", new vector2(margin, ybase + 60), color.yellow);
                    else
                        spritebatch.drawstring(_smallfont, _loc.get("attracts_enemies"), new vector2(margin, ybase + 60), color.lightgray);
                }
                else if (player.weapontype == "lifedrain")
                {
                    spritebatch.drawstring(_smallfont, $"{_loc.get("life_drain_ammo")}: {weapon.ammo}/{weapon.maxammo}", new vector2(margin, ybase + 30), color.crimson);
                    if (weapon.isreloading)
                        spritebatch.drawstring(_smallfont, $"{_loc.get("reloading")}... {weapon.reloadimer:f1}s", new vector2(margin, ybase + 60), color.yellow);
                    else
                        spritebatch.drawstring(_smallfont, $"{_loc.get("permanent_dmg")}: +{player.lifedraindamagebonus}", new vector2(margin, ybase + 60), color.lightgray);
                }
                else
                {
                    if (weapon.isreloading)
                        spritebatch.drawstring(_smallfont, $"{_loc.get("reloading")}... {weapon.reloadimer:f1}s", new vector2(margin, ybase + 30), color.yellow);
                    else
                        spritebatch.drawstring(_smallfont, $"{_loc.get("ammo")}: {weapon.ammo}/{weapon.maxammo}", new vector2(margin, ybase + 30), color.white);
                    spritebatch.drawstring(_smallfont, _loc.get("press_r_reload"), new vector2(margin, ybase + 60), color.lightgray);
                }
            }
            else
            {
                spritebatch.drawstring(_smallfont, _loc.get("press_1_4_weapons"), new vector2(margin, ybase), color.gray);
                spritebatch.drawstring(_smallfont, _loc.get("press_r_reload"), new vector2(margin, ybase + 30), color.gray);
            }
        }

        private void drawabilityui(spritebatch spritebatch, player player, int w, int h)
        {
            var slotsize = 40;
            var spacing = 10;
            var padding = 20;

            var totalactive = slotsize * 4 + spacing * 3;
            var x = w - totalactive - padding - 20;
            var y = h - 200;

            // 主动技能背景
            primitives2d.fillrectangle(spritebatch, new rectangle(x - 10, y - 10, totalactive + 20, slotsize + 20), color.darkblue * 0.8f, 10);
            spritebatch.drawstring(_smallfont, _loc.get("active_skills"), new vector2(x, y - 25), color.white);

            var keys = new[] { "q", "e", "z", "x" };
            for (int i = 0; i < 4; i++)
            {
                var slotx = x + i * (slotsize + spacing);
                var abilityid = player.activeskillslots[i];
                var cooldown = player.activecooldowns[i];

                var bgcolor = abilityid != null ? color.purple * 0.7f : color.darkgray * 0.5f;
                primitives2d.fillrectangle(spritebatch, new rectangle(slotx, y, slotsize, slotsize), bgcolor, 5);
                primitives2d.drawrectangle(spritebatch, new rectangle(slotx, y, slotsize, slotsize), color.white, 2, 5);

                if (abilityid != null)
                {
                    var icon = abilityid.substring(0, 2).toupper();
                    spritebatch.drawstring(_smallfont, icon, new vector2(slotx + 12, y + 12), color.white);

                    if (cooldown > 0)
                    {
                        var percent = cooldown / 30f;
                        primitives2d.fillrectangle(spritebatch, new rectangle(slotx, y, slotsize, (int)(slotsize * percent)), color.black * 0.5f, 0);
                        spritebatch.drawstring(_smallfont, math.ceiling(cooldown).tostring(), new vector2(slotx + 14, y + 14), color.yellow);
                    }
                }
                else
                {
                    spritebatch.drawstring(_smallfont, "?", new vector2(slotx + 15, y + 12), color.gray);
                }

                spritebatch.drawstring(_smallfont, keys[i], new vector2(slotx + 15, y - 15), color.lightgray);
            }

            // 虹吸激活指示
            if (player.siphonactive)
            {
                var alph = 0.5f + 0.5f * (float)math.sin(datetime.now.timeofday.totalseconds * 5);
                spritebatch.drawstring(_smallfont, $"siphon active: {math.ceiling(player.siphonimer)}s", new vector2(x, y + slotsize + 15), color.violet * alph);
            }
        }

        private void drawthrowableui(spritebatch spritebatch, player player, int w, int h)
        {
            var slotsize = 40;
            var spacing = 10;
            var padding = 20;

            var totalwidth = slotsize * 3 + spacing * 2;
            var x = w - totalwidth - padding - 20;
            var y = h - 270;

            primitives2d.fillrectangle(spritebatch, new rectangle(x - 10, y - 10, totalwidth + 20, slotsize + 20), color.darkblue * 0.8f, 10);
            spritebatch.drawstring(_smallfont, _loc.get("throwables"), new vector2(x, y - 25), color.white);

            for (int i = 0; i < 3; i++)
            {
                var slotx = x + i * (slotsize + spacing);
                var throwableid = player.throwableslots[i];
                var hascharge = player.throwablecharges[i] > 0;

                var bgcolor = throwableid != null ? (hascharge ? color.cyan * 0.7f : color.darkcyan * 0.5f) : color.darkgray * 0.5f;
                primitives2d.fillrectangle(spritebatch, new rectangle(slotx, y, slotsize, slotsize), bgcolor, 5);
                primitives2d.drawrectangle(spritebatch, new rectangle(slotx, y, slotsize, slotsize), color.white, 2, 5);

                if (throwableid != null)
                {
                    var icon = throwableid.substring(0, 2).toupper();
                    spritebatch.drawstring(_smallfont, icon, new vector2(slotx + 12, y + 12), color.white);

                    var chargecolor = hascharge ? color.green : color.red;
                    primitives2d.fillcircle(spritebatch, new vector2(slotx + slotsize - 8, y + 8), 4, 6, chargecolor);
                }
                else
                {
                    spritebatch.drawstring(_smallfont, "?", new vector2(slotx + 15, y + 12), color.gray);
                }

                spritebatch.drawstring(_smallfont, (i + 5).tostring(), new vector2(slotx + 15, y - 15), color.lightgray);
            }
        }
    }
}
