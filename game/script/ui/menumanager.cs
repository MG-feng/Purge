using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.data;

namespace purge_v0_4_0.game.script.ui
{
    public class menumanager
    {
        private game1 _game;
        private localization _loc;
        private SpriteFont _font;
        private SpriteFont _smallfont;
        private SpriteFont _titlefont;

        // 百科状态
        public string bestiarycategory = "enemies";
        public int bestiaryindex = 1;
        public int bestiaryscroll = 0;

        public menumanager(game1 game)
        {
            _game = game;
            _loc = game.localization;
        }

        public void loadcontent(SpriteBatch SpriteBatch, Texture2D pixel)
        {
            _font = SpriteFont.default;
            _smallfont = SpriteFont.default;
            _titlefont = SpriteFont.default;
        }

        public void updatemenu(MouseState mouse, MouseState prevmouse, game1 game)
        {
            // 菜单点击处理在 game1 中实现
        }

        public void updatedifficulty(MouseState mouse, MouseState prevmouse, ref string selected, game1 game)
        {
            // 难度选择点击处理在 game1 中实现
        }

        public void updatepause(MouseState mouse, MouseState prevmouse, game1 game)
        {
            // 暂停菜单点击处理在 game1 中实现
        }

        public void updatebestiary(MouseState mouse, MouseState prevmouse, KeyboardState keyboard, KeyboardState prevkeyboard)
        {
            var list = getbestiarylist();

            if (keyboard.iskeydown(Keys.up) && !prevkeyboard.iskeydown(Keys.up))
            {
                bestiaryindex = math.max(1, bestiaryindex - 1);
            }
            if (keyboard.iskeydown(Keys.down) && !prevkeyboard.iskeydown(Keys.down))
            {
                bestiaryindex = math.min(list.count, bestiaryindex + 1);
            }
        }

        public void drawmenu(SpriteBatch SpriteBatch, int maxbits)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;
            var bw = 200f;
            var bh = 50f;
            var starty = h / 2 - 100;

            _game.graphicsdevice.clear(Color.black);

            var title = "purge";
            var titlesize = _titlefont.measurestring(title);
            SpriteBatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, 100), Color.white);

            var buttons = new[]
            {
                new { text = _loc.get("play"), y = starty },
                new { text = _loc.get("inventory"), y = starty + 70 },
                new { text = _loc.get("bestiary"), y = starty + 140 },
                new { text = _loc.get("shop"), y = starty + 210 }
            };

            foreach (var btn in buttons)
            {
                var x = (w - bw) / 2;
                var y = btn.y;
                primitives2d.fillrectangle(SpriteBatch, new Rectangle((int)x, (int)y, (int)bw, (int)bh), Color.cornflowerblue, 10);
                SpriteBatch.drawstring(_font, btn.text, new Vector2(x + bw / 2 - _font.measurestring(btn.text).x / 2, y + 12), Color.white);
            }

            SpriteBatch.drawstring(_smallfont, $"{_loc.get("bits")}: {maxbits}", new Vector2(10, 10), Color.yellow);
        }

        public void drawdifficulty(SpriteBatch SpriteBatch)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;
            var bw = 200f;
            var bh = 50f;
            var starty = h / 2 - 120;

            _game.graphicsdevice.clear(Color.black);

            var title = _loc.get("select_difficulty");
            var titlesize = _titlefont.measurestring(title);
            SpriteBatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, 100), Color.white);

            var difficulties = new[]
            {
                new { text = _loc.get("easy"), y = starty },
                new { text = _loc.get("medium"), y = starty + 60 },
                new { text = _loc.get("hard"), y = starty + 120 },
                new { text = _loc.get("endless"), y = starty + 180 },
                new { text = _loc.get("boss_rush"), y = starty + 240 }
            };

            foreach (var d in difficulties)
            {
                var x = (w - bw) / 2;
                var y = d.y;
                primitives2d.fillrectangle(SpriteBatch, new Rectangle((int)x, (int)y, (int)bw, (int)bh), Color.cornflowerblue, 10);
                SpriteBatch.drawstring(_font, d.text, new Vector2(x + bw / 2 - _font.measurestring(d.text).x / 2, y + 12), Color.white);
            }

            SpriteBatch.drawstring(_smallfont, _loc.get("press_esc_return"), new Vector2(10, h - 30), Color.lightgray);
        }

        public void drawpausemenu(SpriteBatch SpriteBatch)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;

            var title = _loc.get("paused");
            var titlesize = _titlefont.measurestring(title);
            SpriteBatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, h / 2 - 100), Color.white);

            var items = new[]
            {
                new { text = _loc.get("resume"), y = h / 2 - 20 },
                new { text = _loc.get("main_menu"), y = h / 2 + 20 },
                new { text = _loc.get("quit"), y = h / 2 + 60 }
            };

            foreach (var item in items)
            {
                var size = _font.measurestring(item.text);
                var x = (w - size.x) / 2;
                SpriteBatch.drawstring(_font, item.text, new Vector2(x, item.y), Color.white);
            }
        }

        public void drawbestiary(SpriteBatch SpriteBatch)
        {
            var w = _game.graphicsdevice.viewport.width;
            var h = _game.graphicsdevice.viewport.height;

            _game.graphicsdevice.clear(Color.darkblue);

            var title = "bestiary";
            var titlesize = _titlefont.measurestring(title);
            SpriteBatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, 10), Color.white);

            // 分类标签
            var tabwidth = 100f;
            var tabheight = 40f;
            var starty = 90f;
            var spacing = 5f;
            var totalwidth = tabwidth * 5 + spacing * 4;
            var firstx = math.max(10, (w - totalwidth) / 2);

            var categories = new[]
            {
                new { name = "enemies", cat = "enemies", x = firstx },
                new { name = "weapons", cat = "weapons", x = firstx + tabwidth + spacing },
                new { name = "mods", cat = "mods", x = firstx + (tabwidth + spacing) * 2 },
                new { name = "abilities", cat = "abilities", x = firstx + (tabwidth + spacing) * 3 },
                new { name = "difficulties", cat = "difficulties", x = firstx + (tabwidth + spacing) * 4 }
            };

            foreach (var cat in categories)
            {
                var bgcolor = bestiarycategory == cat.cat ? Color.lightblue : Color.darkblue;
                primitives2d.fillrectangle(SpriteBatch, new Rectangle((int)cat.x, (int)starty, (int)tabwidth, (int)tabheight), bgcolor, 10);
                SpriteBatch.drawstring(_smallfont, cat.name, new Vector2(cat.x + tabwidth / 2 - _smallfont.measurestring(cat.name).x / 2, starty + 10), Color.white);
            }

            // 左侧列表
            var list = getbestiarylist();
            var listx = 50;
            var listy = 150;
            var listw = 200;
            var listh = h - 200;

            primitives2d.fillrectangle(SpriteBatch, new Rectangle(listx, listy, listw, listh), Color.darkblue * 0.8f, 10);

            var itemheight = 30;
            var visibleitems = listh / itemheight;
            var startidx = bestiaryscroll;
            var endidx = math.min(startidx + visibleitems, list.count);

            for (int i = startidx; i < endidx; i++)
            {
                var entry = list[i];
                var y = listy + (i - startidx) * itemheight;
                var isselected = (i + 1) == bestiaryindex;

                if (isselected)
                    primitives2d.fillrectangle(SpriteBatch, new Rectangle(listx + 5, y, listw - 15, itemheight - 4), Color.cornflowerblue, 5);

                SpriteBatch.drawstring(_smallfont, entry.name, new Vector2(listx + 10, y + 5), Color.white);
            }

            // 右侧详情
            var detailx = 270;
            var detaily = 150;
            var detailw = w - 320;
            primitives2d.fillrectangle(SpriteBatch, new Rectangle(detailx, detaily, detailw, listh), Color.darkblue * 0.8f, 10);

            if (bestiaryindex <= list.count && bestiaryindex > 0)
            {
                var entry = list[bestiaryindex - 1];
                SpriteBatch.drawstring(_titlefont, entry.name, new Vector2(detailx + 20, detaily + 20), Color.white);
                SpriteBatch.drawstring(_smallfont, entry.description, new Vector2(detailx + 20, detaily + 80), Color.lightgray);
            }

            SpriteBatch.drawstring(_smallfont, $"{_loc.get("esc_return")} | ↑/↓: {_loc.get("up_down_scroll")}", new Vector2(20, h - 30), Color.gray);
        }

        private List<bestiaryentry> getbestiarylist()
        {
            var list = new List<bestiaryentry>();
            var data = new bestiarydata();

            if (bestiarycategory == "enemies")
            {
                foreach (var e in data.enemies)
                    list.add(new bestiaryentry { name = e.name, description = e.description });
            }
            else if (bestiarycategory == "weapons")
            {
                foreach (var w in data.weapons)
                    list.add(new bestiaryentry { name = w.name, description = w.description });
            }

            return list;
        }

        public void bestiarywheel(int delta)
        {
            var list = getbestiarylist();
            var visibleitems = 5;
            var maxscroll = math.max(0, list.count - visibleitems);
            bestiaryscroll = math.clamp(bestiaryscroll + (delta > 0 ? -1 : 1), 0, maxscroll);
        }

        private class bestiaryentry
        {
            public string name = "";
            public string description = "";
        }
    }
}
