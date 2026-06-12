using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.systems
{
    public class inventorysystem
    {
        private game1 _game;
        private localization _loc;

        public string category = "weapons";
        public string subcategory = "pistol";
        public string skilltype = "active";
        public string bindingweapon = null;
        public string bindingskill = null;
        public string bindingthrowable = null;
        public int scrolloffset = 0;

        public inventorysystem(game1 game)
        {
            _game = game;
            _loc = game.localization;
        }

        public void update(MouseState mouse, MouseState prevmouse, KeyboardState keyboard, KeyboardState prevkeyboard, game1 game)
        {
            // 绑定处理
            if (!string.isnullorempty(bindingweapon))
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (keyboard.iskeydown((Keys)((int)Keys.d1 + i - 1)) && !prevkeyboard.iskeydown((Keys)((int)Keys.d1 + i - 1)))
                    {
                        bindweapon(bindingweapon, i - 1);
                        bindingweapon = null;
                        break;
                    }
                }
            }

            if (!string.isnullorempty(bindingskill) && skilltype == "active")
            {
                var Keys = new[] { Keys.q, Keys.e, Keys.z, Keys.x };
                for (int i = 0; i < Keys.length; i++)
                {
                    if (keyboard.iskeydown(Keys[i]) && !prevkeyboard.iskeydown(Keys[i]))
                    {
                        bindactiveskill(bindingskill, i);
                        bindingskill = null;
                        break;
                    }
                }
            }

            if (!string.isnullorempty(bindingthrowable))
            {
                for (int i = 5; i <= 7; i++)
                {
                    if (keyboard.iskeydown((Keys)((int)Keys.d5 + i - 5)) && !prevkeyboard.iskeydown((Keys)((int)Keys.d5 + i - 5)))
                    {
                        bindthrowable(bindingthrowable, i - 5);
                        bindingthrowable = null;
                        break;
                    }
                }
            }
        }

        public void bindweapon(string weapontype, int slot)
        {
            var player = _game.player;

            // 移除旧绑定
            for (int i = 0; i < player.weaponslots.length; i++)
            {
                if (player.weaponslots[i] == weapontype)
                    player.weaponslots[i] = null;
            }

            player.weaponslots[slot] = weapontype;
            system.console.writeline($"bound weapon {weapontype} to slot {slot + 1}");
        }

        public void bindactiveskill(string skillid, int slot)
        {
            var player = _game.player;

            for (int i = 0; i < player.activeskillslots.length; i++)
            {
                if (player.activeskillslots[i] == skillid)
                    player.activeskillslots[i] = null;
            }

            player.activeskillslots[slot] = skillid;
            system.console.writeline($"bound active skill {skillid} to slot {slot + 1}");
        }

        public void bindpassiveskill(string skillid)
        {
            var player = _game.player;

            // 找空槽位
            int slot = -1;
            for (int i = 0; i < player.passiveskillslots.length; i++)
            {
                if (string.isnullorempty(player.passiveskillslots[i]))
                {
                    slot = i;
                    break;
                }
            }

            if (slot == -1) slot = 0;

            player.passiveskillslots[slot] = skillid;
            _game.abilitysystem.applypassiveeffects(player);
            system.console.writeline($"equipped passive skill {skillid} to slot p{slot + 1}");
        }

        public void bindthrowable(string throwableid, int slot)
        {
            var player = _game.player;

            for (int i = 0; i < player.throwableslots.length; i++)
            {
                if (player.throwableslots[i] == throwableid)
                {
                    player.throwableslots[i] = null;
                    player.throwablecharges[i] = 0;
                }
            }

            player.throwableslots[slot] = throwableid;
            player.throwablecharges[slot] = 1;
            system.console.writeline($"bound throwable {throwableid} to slot {slot + 5}");
        }

        public void unequippassiveskill(int slot)
        {
            var player = _game.player;
            var skillid = player.passiveskillslots[slot];
            if (!string.isnullorempty(skillid))
            {
                // 移除效果
                switch (skillid)
                {
                    case "dragonslayer":
                        player.hasdragonslayer = false;
                        break;
                    case "bloodthirst":
                        player.hasbloodthirst = false;
                        break;
                    case "constant":
                        player.hasconstantmotion = false;
                        player.walkspeed = player.speedwalk ? 175 : 125;
                        player.runspeed = player.speedrun ? 325 : 250;
                        break;
                }
                player.passiveskillslots[slot] = null;
            }
        }

        public List<inventoryitem> getitems()
        {
            var items = new List<inventoryitem>();
            var player = _game.player;

            if (category == "weapons")
            {
                if (player.hasrifle) items.add(new inventoryitem { name = "rifle", type = "rifle", icon = "r", Color = Color.gold, owned = true });
                if (player.hassniper) items.add(new inventoryitem { name = "sniper", type = "sniper", icon = "s", Color = Color.skyblue, owned = true });
                if (player.hassoulreaper) items.add(new inventoryitem { name = "soul reaper", type = "soulreaper", icon = "sr", Color = Color.purple, owned = true });
                if (player.haslasergun) items.add(new inventoryitem { name = "laser gun", type = "lasergun", icon = "l", Color = Color.red, owned = true });
                if (player.hasfeast) items.add(new inventoryitem { name = "feast", type = "feast", icon = "ft", Color = Color.orange, owned = true });
                if (player.haslifedrain) items.add(new inventoryitem { name = "life drain", type = "lifedrain", icon = "ld", Color = Color.crimson, owned = true });
            }
            else if (category == "skills")
            {
                var abilities = new[] { "heal", "dragonslayer", "bloodthirst", "constant" };
                foreach (var id in abilities)
                {
                    if (player.ownedskills.contains(id))
                    {
                        var name = id switch
                        {
                            "heal" => "heal",
                            "dragonslayer" => "dragon slayer",
                            "bloodthirst" => "bloodthirst",
                            "constant" => "constant motion",
                            _ => id
                        };
                        var isactive = id == "heal";
                        if ((skilltype == "active" && isactive) || (skilltype == "passive" && !isactive))
                        {
                            items.add(new inventoryitem { name = name, type = id, icon = id.substring(0, 2), Color = Color.white, owned = true, isactive = isactive });
                        }
                    }
                }
            }
            else if (category == "throwables")
            {
                if (player.gravityanchor) items.add(new inventoryitem { name = "gravity anchor", type = "gravityanchor", icon = "ga", Color = Color.purple, owned = true });
                if (player.soulshard) items.add(new inventoryitem { name = "soul shard", type = "soulshard", icon = "ss", Color = Color.magenta, owned = true });
                if (player.phasebeacon) items.add(new inventoryitem { name = "phase beacon", type = "phasebeacon", icon = "pb", Color = Color.cyan, owned = true });
            }

            return items;
        }

        public void draw(SpriteBatch SpriteBatch, player player, int maxbits)
        {
            var w = _game.GraphicsDevice.viewport.width;
            var h = _game.GraphicsDevice.viewport.height;
            var font = _game.uimanager.getfont();
            var smallfont = _game.uimanager.getsmallfont();

            // 标题
            var title = _loc.get("inventory");
            var titlesize = font.measurestring(title);
            SpriteBatch.drawstring(font, title, new Vector2(w / 2 - titlesize.x / 2, 30), Color.white);
            SpriteBatch.drawstring(smallfont, $"{_loc.get("bits")}: {maxbits}", new Vector2(20, 20), Color.yellow);

            // 分类标签
            var tabwidth = 120f;
            var tabheight = 40f;
            var taby = 90f;
            var spacing = 10f;
            var totalwidth = tabwidth * 4 + spacing * 3;
            var firstx = (w - totalwidth) / 2;

            var categories = new[]
            {
                new { name = _loc.get("weapons"), cat = "weapons", x = firstx },
                new { name = _loc.get("skills"), cat = "skills", x = firstx + tabwidth + spacing },
                new { name = _loc.get("throwables"), cat = "throwables", x = firstx + (tabwidth + spacing) * 2 },
                new { name = _loc.get("character"), cat = "character", x = firstx + (tabwidth + spacing) * 3 }
            };

            foreach (var cat in categories)
            {
                var bgcolor = category == cat.cat ? Color.lightblue : Color.darkblue;
                primitives2d.fillrectangle(SpriteBatch, new Rectangle((int)cat.x, (int)taby, (int)tabwidth, (int)tabheight), bgcolor, 10);
                SpriteBatch.drawstring(font, cat.name, new Vector2(cat.x + tabwidth / 2 - font.measurestring(cat.name).x / 2, taby + 10), Color.white);
            }

            // 物品列表
            var items = getitems();
            var starty = taby + 60;
            var itemheight = 80f;
            var visibleitems = (int)((h - starty - 150) / itemheight);
            var startidx = scrolloffset;
            var endidx = math.min(startidx + visibleitems, items.count);

            for (int i = startidx; i < endidx; i++)
            {
                var item = items[i];
                var y = starty + (i - startidx) * itemheight;
                var itemwidth = 500f;
                var itemx = (w - itemwidth) / 2;

                primitives2d.fillrectangle(SpriteBatch, new Rectangle((int)itemx, (int)y, (int)itemwidth, (int)itemheight - 5), Color.darkblue * 0.8f, 10);

                SpriteBatch.drawstring(font, item.icon, new Vector2(itemx + 15, y + 25), item.Color);
                SpriteBatch.drawstring(smallfont, item.name, new Vector2(itemx + 60, y + 20), Color.white);

                // 绑定槽位显示
                if (category == "weapons")
                {
                    for (int s = 0; s < player.weaponslots.length; s++)
                    {
                        if (player.weaponslots[s] == item.type)
                        {
                            SpriteBatch.drawstring(smallfont, $"[{s + 1}]", new Vector2(itemx + itemwidth - 60, y + 25), Color.green);
                            break;
                        }
                    }
                }
            }

            // 提示
            var hint = $"{_loc.get("esc_return")} | {_loc.get("up_down_scroll")} | {_loc.get("click_item_bind")}";
            var hintsize = smallfont.measurestring(hint);
            SpriteBatch.drawstring(smallfont, hint, new Vector2(20, h - 30), Color.gray);
        }

        public void wheelmoved(int delta)
        {
            var items = getitems();
            var visibleitems = 5;
            var maxscroll = math.max(0, items.count - visibleitems);
            scrolloffset = math.clamp(scrolloffset + (delta > 0 ? -1 : 1), 0, maxscroll);
        }

        public class inventoryitem
        {
            public string name = "";
            public string type = "";
            public string icon = "";
            public Color Color = Color.white;
            public bool owned = false;
            public bool isactive = false;
        }
    }
}
