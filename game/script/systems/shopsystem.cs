using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.systems
{
    public class shopsystem
    {
        private game1 _game;
        private gameconfig _config;
        private localization _loc;

        public string selectedcategory = "weapons";
        public string selectedweapon = "pistol";
        public string selectedabilitytype = "active";
        public int scrolloffset = 0;
        public string message = "";
        public float messagemeimer = 0f;

        public shopsystem(game1 game)
        {
            _game = game;
            _config = game.config;
            _loc = game.localization;
        }

        public void update(mousestate mouse, mousestate prevmouse, keyboardstate keyboard, keyboardstate prevkeyboard, game1 game)
        {
            if (messagemeimer > 0)
                messagemeimer -= (float)game.time.elapsedgametime.totalseconds;
            else
                message = "";

            // 鼠标点击处理在 game1 中调用
        }

        public bool purchase(string itemid)
        {
            var item = getitem(itemid);
            if (item == null) return false;

            if (!item.canpurchase(_game.player))
            {
                showmessage(_loc.get("already_owned"));
                return false;
            }

            if (_game.maxbits < item.price)
            {
                showmessage($"{_loc.get("not_enough_bits")} {item.price}");
                return false;
            }

            _game.maxbits -= item.price;
            _game.addbits(0);
            item.onpurchase(_game.player, _game);

            showmessage($"{_loc.get("purchased")} {item.name}");
            return true;
        }

        private shopitem getitem(string id)
        {
            return id switch
            {
                "rifle" => new shopitem
                {
                    id = "rifle",
                    name = "rifle",
                    type = "weapon",
                    price = 750,
                    description = "damage: 6 | fire rate: 10/s",
                    icon = "r",
                    color = color.gold,
                    canpurchase = (p) => !p.hasrifle,
                    onpurchase = (p, g) => { p.hasrifle = true; }
                },
                "sniper" => new shopitem
                {
                    id = "sniper",
                    name = "sniper",
                    type = "weapon",
                    price = 2000,
                    description = "damage: 25 | fire rate: 1/s",
                    icon = "s",
                    color = color.skyblue,
                    canpurchase = (p) => !p.hassniper,
                    onpurchase = (p, g) => { p.hassniper = true; }
                },
                "soulreaper" => new shopitem
                {
                    id = "soulreaper",
                    name = "soul reaper",
                    type = "weapon",
                    price = 4000,
                    description = "dmg:750 | cd:5s | charge:500",
                    icon = "sr",
                    color = color.purple,
                    canpurchase = (p) => !p.hassoulreaper,
                    onpurchase = (p, g) => { p.hassoulreaper = true; }
                },
                "lasergun" => new shopitem
                {
                    id = "lasergun",
                    name = "laser gun",
                    type = "weapon",
                    price = 7500,
                    description = "dmg:15 | 10/s | charge:50",
                    icon = "l",
                    color = color.red,
                    canpurchase = (p) => !p.haslasergun,
                    onpurchase = (p, g) => { p.haslasergun = true; }
                },
                "feast" => new shopitem
                {
                    id = "feast",
                    name = "feast",
                    type = "weapon",
                    price = 75000,
                    description = "dmg:25 | 1.5s cd | 5s explosion",
                    icon = "ft",
                    color = color.orange,
                    canpurchase = (p) => !p.hasfeast,
                    onpurchase = (p, g) => { p.hasfeast = true; }
                },
                "lifedrain" => new shopitem
                {
                    id = "lifedrain",
                    name = "life drain",
                    type = "weapon",
                    price = 35000,
                    description = "dmg:10 | triple burst | permanent dmg",
                    icon = "ld",
                    color = color.crimson,
                    canpurchase = (p) => !p.haslifedrain,
                    onpurchase = (p, g) => { p.haslifedrain = true; }
                },
                "heal" => new shopitem
                {
                    id = "heal",
                    name = "heal",
                    type = "ability",
                    price = 1500,
                    description = "heal 25 hp | cd:30s",
                    icon = "hp",
                    color = color.green,
                    canpurchase = (p) => !p.ownedskills.contains("heal"),
                    onpurchase = (p, g) => { p.ownedskills.add("heal"); }
                },
                "dragonslayer" => new shopitem
                {
                    id = "dragonslayer",
                    name = "dragon slayer",
                    type = "ability",
                    price = 3000,
                    description = "+10% damage to bosses",
                    icon = "ds",
                    color = color.gold,
                    canpurchase = (p) => !p.ownedskills.contains("dragonslayer"),
                    onpurchase = (p, g) => { p.ownedskills.add("dragonslayer"); g.abilitysystem.applypassiveeffects(p); }
                },
                "bloodthirst" => new shopitem
                {
                    id = "bloodthirst",
                    name = "bloodthirst",
                    type = "ability",
                    price = 8000,
                    description = "kill heals 1 hp, bosses heal 20 hp",
                    icon = "bt",
                    color = color.red,
                    canpurchase = (p) => !p.ownedskills.contains("bloodthirst"),
                    onpurchase = (p, g) => { p.ownedskills.add("bloodthirst"); g.abilitysystem.applypassiveeffects(p); }
                },
                "gravityanchor" => new shopitem
                {
                    id = "gravityanchor",
                    name = "gravity anchor",
                    type = "throwable",
                    price = 1000,
                    description = "slow field, enemies -50% speed",
                    icon = "ga",
                    color = color.purple,
                    canpurchase = (p) => !p.gravityanchor,
                    onpurchase = (p, g) => { p.gravityanchor = true; }
                },
                _ => null
            };
        }

        public list<shopitem> getcurrentitems()
        {
            var items = new list<shopitem>();
            var allitems = new[] { "rifle", "sniper", "soulreaper", "lasergun", "feast", "lifedrain",
                                   "heal", "dragonslayer", "bloodthirst", "gravityanchor" };

            foreach (var id in allitems)
            {
                var item = getitem(id);
                if (item != null)
                {
                    if (selectedcategory == "weapons" && item.type == "weapon")
                        items.add(item);
                    else if (selectedcategory == "abilities" && item.type == "ability")
                        items.add(item);
                    else if (selectedcategory == "throwables" && item.type == "throwable")
                        items.add(item);
                }
            }

            return items;
        }

        private void showmessage(string msg)
        {
            message = msg;
            messagemeimer = 2f;
        }

        public void draw(spritebatch spritebatch, player player, int maxbits)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;
            var font = _game.uimanager.getfont();
            var smallfont = _game.uimanager.getsmallfont();

            // 标题
            var title = _loc.get("shop");
            var titlesize = font.measurestring(title);
            spritebatch.drawstring(font, title, new vector2(w / 2 - titlesize.x / 2, 30), color.white);

            // 比特显示
            spritebatch.drawstring(smallfont, $"{_loc.get("bits")}: {maxbits}", new vector2(20, 20), color.yellow);

            // 分类标签
            var tabwidth = 120f;
            var tabheight = 40f;
            var taby = 80f;
            var spacing = 10f;
            var totalwidth = tabwidth * 3 + spacing * 2;
            var firstx = (w - totalwidth) / 2;

            var categories = new[]
            {
                new { name = _loc.get("weapons"), cat = "weapons", x = firstx },
                new { name = _loc.get("abilities"), cat = "abilities", x = firstx + tabwidth + spacing },
                new { name = _loc.get("throwables"), cat = "throwables", x = firstx + (tabwidth + spacing) * 2 }
            };

            foreach (var cat in categories)
            {
                var hover = false; // 鼠标检测简化
                var color = selectedcategory == cat.cat ? color.lightblue : (hover ? color.cornflowerblue : color.darkblue);
                primitives2d.fillrectangle(spritebatch, new rectangle((int)cat.x, (int)taby, (int)tabwidth, (int)tabheight), color, 10);
                spritebatch.drawstring(font, cat.name, new vector2(cat.x + tabwidth / 2 - font.measurestring(cat.name).x / 2, taby + 10), color.white);
            }

            // 物品列表
            var items = getcurrentitems();
            var starty = taby + 60;
            var itemheight = 90f;
            var visibleitems = (int)((h - starty - 100) / itemheight);
            var startidx = scrolloffset;
            var endidx = math.min(startidx + visibleitems, items.count);

            for (int i = startidx; i < endidx; i++)
            {
                var item = items[i];
                var y = starty + (i - startidx) * itemheight;
                var itemwidth = 500f;
                var itemx = (w - itemwidth) / 2;

                var owned = !item.canpurchase(player);
                var bgcolor = item.color * (owned ? 0.3f : 0.5f);
                primitives2d.fillrectangle(spritebatch, new rectangle((int)itemx, (int)y, (int)itemwidth, (int)itemheight - 5), bgcolor, 10);

                // 图标
                spritebatch.drawstring(font, item.icon, new vector2(itemx + 15, y + 25), color.white);

                // 名称
                spritebatch.drawstring(smallfont, item.name, new vector2(itemx + 60, y + 15), color.white);

                // 价格/拥有状态
                if (owned)
                    spritebatch.drawstring(smallfont, _loc.get("owned"), new vector2(itemx + itemwidth - 100, y + 30), color.green);
                else
                    spritebatch.drawstring(smallfont, $"{item.price} {_loc.get("bits")}", new vector2(itemx + itemwidth - 120, y + 30), color.yellow);

                // 描述
                spritebatch.drawstring(smallfont, item.description, new vector2(itemx + 60, y + 50), color.lightgray);
            }

            // 消息
            if (!string.isnullorempty(message))
            {
                var msgcolor = color.yellow * math.min(1f, messagemeimer);
                var msgsize = font.measurestring(message);
                spritebatch.drawstring(font, message, new vector2(w / 2 - msgsize.x / 2, h - 80), msgcolor);
            }

            // 提示
            var hint = $"{_loc.get("esc_return")} | {_loc.get("up_down_scroll")}";
            var hintsize = smallfont.measurestring(hint);
            spritebatch.drawstring(smallfont, hint, new vector2(20, h - 30), color.gray);
        }

        public void wheelmoved(int delta)
        {
            var items = getcurrentitems();
            var visibleitems = 5;
            var maxscroll = math.max(0, items.count - visibleitems);
            scrolloffset = math.clamp(scrolloffset + (delta > 0 ? -1 : 1), 0, maxscroll);
        }

        public class shopitem
        {
            public string id = "";
            public string name = "";
            public string type = "";
            public int price = 0;
            public string description = "";
            public string icon = "";
            public color color = color.white;
            public func<player, bool> canpurchase = (p) => true;
            public action<player, game1> onpurchase = (p, g) => { };
        }
    }
}
