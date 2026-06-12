using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using purge_v0_4_0.game.script.core;
using purge_v0_4_0.game.script.systems;
using purge_v0_4_0.game.script.ui;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0
{
    public class game1 : game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spritebatch;
        private Texture2D _pixel;
        private SpriteFont _font;
        private SpriteFont _smallfont;
        private SpriteFont _titlefont;

        // ????
        private gameconfig _config;
        private settings _settings;
        private localization _loc;
        private datamanager _dataManager;
        private objectpool _objectPool;

        // ????
        private wavesystem _waveSystem;
        private abilitysystem _abilitySystem;
        private throwablesystem _throwableSystem;
        private shopsystem _shopSystem;
        private inventorysystem _inventorySystem;
        private uimanager _uiManager;
        private menumanager _menuManager;

        // ??
        private player _player;
        private List<enemy> _enemies;
        private List<bullet> _bullets;
        private List<enemybullet> _enemyBullets;
        private List<throwablefield> _throwableFields;

        // ????
        public enum gamestate { menu, difficulty, playing, inventory, bestiary, shop }
        public gamestate currentstate = gamestate.menu;
        public string selecteddifficulty = "easy";
        public bool paused = false;
        private int _bits = 0;
        private int _maxbits = 0;

        // ????
        private int _mapwidth = 1000;
        private int _mapheight = 1000;
        private float _mapminx = -500f;
        private float _mapmaxx = 500f;
        private float _mapminy = -500f;
        private float _mapmaxy = 500f;
        private bool _mapexpanded = false;
        private Rectangle _obstacle;

        // ??
        private KeyboardState _prevkeyboard;
        private MouseState _prevmouse;

        // ??
        private float _currentcooldown = 0f;
        private float _currentangle = 0f;
        private float _staminatimer = 0f;
        private float _recoverydelaytimer = 0f;
        private bool _canrecover = true;

        // ??
        private List<trail> _trails = new List<trail>();

        // ??
        private float _lastautoshot = 0f;
        private float _frametime = 0f;

        public game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            content.rootdirectory = "content";
            ismousevisible = true;
            window.allouserresizing = true;
        }

        protected override void initialize()
        {
            _graphics.preferredbackbufferwidth = 800;
            _graphics.preferredbackbufferheight = 600;
            _graphics.applychanges();

            // ???????
            _config = new gameconfig();
            _settings = new settings();
            _loc = new localization();
            _dataManager = new datamanager();
            _objectPool = new objectpool(_config);

            // ????
            _settings.load();
            _loc.load(_settings.language);

            // ???????
            _waveSystem = new wavesystem(this);
            _abilitySystem = new abilitysystem(this);
            _throwableSystem = new throwablesystem(this);
            _shopSystem = new shopsystem(this);
            _inventorySystem = new inventorysystem(this);
            _uiManager = new uimanager(this);
            _menuManager = new menumanager(this);

            // ???????
            _enemies = new List<enemy>();
            _bullets = new List<bullet>();
            _enemyBullets = new List<enemybullet>();
            _throwableFields = new List<throwablefield>();

            // ??????
            _obstacle = new Rectangle(-150, -150, 300, 300);

            // ?????
            _player = new player(_config);

            // ????
            _dataManager.load(_player, ref _maxbits);

            // ????
            _player.applymods();
            _abilitySystem.applypassiveeffects(_player);

            base.initialize();
        }

        protected override void loadcontent()
        {
            _spritebatch = new SpriteBatch(graphicsdevice);
            _pixel = new Texture2D(graphicsdevice, 1, 1);
            _pixel.setdata(new[] { Color.white });

            primitives2d.init(graphicsdevice);

            // ??????
            var fontcolor = new[] { Color.white };
            var fonttex = new Texture2D(graphicsdevice, 256, 256);
            _font = SpriteFont.default;
            _smallfont = SpriteFont.default;
            _titlefont = SpriteFont.default;
        }

        protected override void update(GameTime GameTime)
        {
            var dt = (float)GameTime.elapsedgametime.totalseconds;
            _frametime = dt;
            var keyboard = keyboard.getstate();
            var mouse = MouseState.getstate();

            // ??ESC??
            if (keyboard.iskeydown(Keys.escape) && !_prevkeyboard.iskeydown(Keys.escape))
            {
                handleescape();
            }

            // ??M???(???????)
            if (currentstate == gamestate.playing && !paused && !_player.isdead)
            {
                if (keyboard.iskeydown(Keys.m) && !_prevkeyboard.iskeydown(Keys.m))
                {
                    currentstate = gamestate.menu;
                }
            }

            // ??R???(??)
            if (currentstate == gamestate.playing && !paused && !_player.isdead)
            {
                if (keyboard.iskeydown(Keys.r) && !_prevkeyboard.iskeydown(Keys.r))
                {
                    reloadweapon();
                }
            }

            // ???????
            if (currentstate == gamestate.playing && !paused && !_player.isdead)
            {
                for (int i = 1; i <= 4; i++)
                {
                    var key = (Keys)((int)Keys.d1 + i - 1);
                    if (keyboard.iskeydown(key) && !_prevkeyboard.iskeydown(key))
                    {
                        switchweapon(i - 1);
                    }
                }

                // ????? 5-7
                for (int i = 5; i <= 7; i++)
                {
                    var key = (Keys)((int)Keys.d5 + i - 5);
                    if (keyboard.iskeydown(key) && !_prevkeyboard.iskeydown(key))
                    {
                        usethrowable(i - 5);
                    }
                }

                // ???? Q/E/Z/X
                var skillkeys = new[] { Keys.q, Keys.e, Keys.z, Keys.x };
                for (int i = 0; i < skillkeys.length; i++)
                {
                    if (keyboard.iskeydown(skillkeys[i]) && !_prevkeyboard.iskeydown(skillkeys[i]))
                    {
                        useactiveskill(i);
                    }
                }

                // ???????? (7???????,????7?????)
                if (keyboard.iskeydown(Keys.d7) && !_prevkeyboard.iskeydown(Keys.d7))
                {
                    _throwableSystem.trytriggerbeacon();
                }
            }

            // ??????
            if (currentstate == gamestate.playing && _player.isdead)
            {
                if (keyboard.iskeydown(Keys.x) && !_prevkeyboard.iskeydown(Keys.x))
                {
                    restartgame();
                }
            }

            switch (currentstate)
            {
                case gamestate.menu:
                    updatemenu(mouse);
                    break;
                case gamestate.difficulty:
                    updatedifficulty(mouse);
                    break;
                case gamestate.playing:
                    if (!paused && !_player.isdead)
                    {
                        updategameplaying(dt, keyboard, mouse);
                    }
                    else if (paused)
                    {
                        updatepause(mouse);
                    }
                    break;
                case gamestate.inventory:
                    _inventorySystem.update(mouse, _prevmouse, keyboard, _prevkeyboard, this);
                    break;
                case gamestate.bestiary:
                    _menuManager.updatebestiary(mouse, _prevmouse, keyboard, _prevkeyboard);
                    break;
                case gamestate.shop:
                    _shopSystem.update(mouse, _prevmouse, keyboard, _prevkeyboard, this);
                    break;
            }

            // ??UI???
            _uiManager.updatebars(_player);

            _prevkeyboard = keyboard;
            _prevmouse = mouse;

            base.update(GameTime);
        }

        private void updatemenu(MouseState mouse)
        {
            var w = graphicsdevice.viewport.width;
            var h = graphicsdevice.viewport.height;
            var bw = 200;
            var bh = 50;
            var starty = h / 2 - 100;

            var buttons = new[]
            {
                new { text = _loc.get("play"), y = starty, x = (w - bw) / 2 },
                new { text = _loc.get("inventory"), y = starty + 70, x = (w - bw) / 2 },
                new { text = _loc.get("bestiary"), y = starty + 140, x = (w - bw) / 2 },
                new { text = _loc.get("shop"), y = starty + 210, x = (w - bw) / 2 }
            };

            if (mouse.leftbutton == ButtonState.pressed && _prevmouse.leftbutton == ButtonState.released)
            {
                for (int i = 0; i < buttons.length; i++)
                {
                    var btn = buttons[i];
                    if (mouse.x >= btn.x && mouse.x <= btn.x + bw && mouse.y >= btn.y && mouse.y <= btn.y + bh)
                    {
                        if (i == 0) currentstate = gamestate.difficulty;
                        else if (i == 1) currentstate = gamestate.inventory;
                        else if (i == 2) currentstate = gamestate.bestiary;
                        else if (i == 3) currentstate = gamestate.shop;
                    }
                }
            }
        }

        private void updatedifficulty(MouseState mouse)
        {
            var w = graphicsdevice.viewport.width;
            var h = graphicsdevice.viewport.height;
            var bw = 200;
            var bh = 50;
            var starty = h / 2 - 120;

            var difficulties = new[]
            {
                new { text = "easy", y = starty, x = (w - bw) / 2 },
                new { text = "medium", y = starty + 60, x = (w - bw) / 2 },
                new { text = "hard", y = starty + 120, x = (w - bw) / 2 },
                new { text = "endless", y = starty + 180, x = (w - bw) / 2 },
                new { text = "bossrush", y = starty + 240, x = (w - bw) / 2 }
            };

            if (mouse.leftbutton == ButtonState.pressed && _prevmouse.leftbutton == ButtonState.released)
            {
                for (int i = 0; i < difficulties.length; i++)
                {
                    var d = difficulties[i];
                    if (mouse.x >= d.x && mouse.x <= d.x + bw && mouse.y >= d.y && mouse.y <= d.y + bh)
                    {
                        selecteddifficulty = d.text;
                        startnewgame();
                    }
                }
            }
        }

        private void updatepause(MouseState mouse)
        {
            var w = graphicsdevice.viewport.width;
            var h = graphicsdevice.viewport.height;

            var resume = new Rectangle(w / 2 - 50, h / 2 - 20, 100, 30);
            var menu = new Rectangle(w / 2 - 70, h / 2 + 20, 140, 30);
            var quitgame = new Rectangle(w / 2 - 40, h / 2 + 60, 80, 30);

            if (mouse.leftbutton == ButtonState.pressed && _prevmouse.leftbutton == ButtonState.released)
            {
                if (resume.contains(mouse.x, mouse.y)) paused = false;
                else if (menu.contains(mouse.x, mouse.y)) { currentstate = gamestate.menu; paused = false; }
                else if (quitgame.contains(mouse.x, mouse.y)) exit();
            }
        }

        private void updategameplaying(float dt, KeyboardState keyboard, MouseState mouse)
        {
            if (_player.health <= 0 && !_player.isdead)
            {
                _player.isdead = true;
                _player.deathtimer = 2f;
                _dataManager.save(_player, _maxbits);
                return;
            }

            if (_player.isdead)
            {
                _player.deathtimer -= dt;
                _player.blinktimer += dt;
                if (_player.blinktimer >= 0.2f) _player.blinktimer = 0;
                return;
            }

            // ????
            if (_waveSystem.currentwave >= 30 && !_mapexpanded)
                expandmap();

            // ?????
            if (_player.haslasergun)
            {
                var currenttime = (float)DateTime.now.timeofday.totalseconds;
                if (currenttime - _player.lastlasershottime > 4f)
                {
                    _player.laserguncharge = math.min(_player.maxlaserguncharge, _player.laserguncharge + 10 * dt);
                }
            }

            // ????
            var move = Vector2.zero;
            if (keyboard.iskeydown(Keys.w)) move.y -= 1;
            if (keyboard.iskeydown(Keys.s)) move.y += 1;
            if (keyboard.iskeydown(Keys.a)) move.x -= 1;
            if (keyboard.iskeydown(Keys.d)) move.x += 1;
            if (move != Vector2.zero) move.normalize();

            // ????
            var shiftpressed = keyboard.iskeydown(Keys.leftshift);
            var moving = move != Vector2.zero;

            if (_player.hasconstantmotion)
            {
                _player.speed = 260;
                if (_canrecover)
                {
                    _staminatimer += dt;
                    if (_staminatimer >= 0.025f)
                    {
                        _staminatimer = 0;
                        _player.stamina = math.min(_player.maxstamina, _player.stamina + 1);
                    }
                }
            }
            else
            {
                if (shiftpressed && moving)
                {
                    _player.stamina -= 20 * dt;
                    if (_player.stamina < 0) _player.stamina = 0;
                    _recoverydelaytimer = 0;
                    _canrecover = false;
                }

                if (!_canrecover)
                {
                    _recoverydelaytimer += dt;
                    if (_recoverydelaytimer >= 1f) { _canrecover = true; _recoverydelaytimer = 0; }
                }

                if (_canrecover)
                {
                    _staminatimer += dt;
                    if (_staminatimer >= 0.025f)
                    {
                        _staminatimer = 0;
                        _player.stamina = math.min(_player.maxstamina, _player.stamina + 1);
                    }
                }

                _player.speed = (shiftpressed && moving && _player.stamina > 0) ? _player.runspeed : _player.walkspeed;
            }

            _player.position += move * _player.speed * dt;
            clampplayer();

            // ????
            if (_player.weapontype != "lasergun")
            {
                var cameraworld = _player.center - new Vector2(graphicsdevice.viewport.width / 2, graphicsdevice.viewport.height / 2);
                var worldmouse = new Vector2(mouse.x, mouse.y) + cameraworld;
                var dx = worldmouse.x - _player.center.x;
                var dy = worldmouse.y - _player.center.y;
                _currentangle = (float)math.atan2(dy, dx);
            }

            // ??
            if (mouse.leftbutton == ButtonState.pressed && _player.weaponequipped)
            {
                shootweapon();
            }

            // ????
            if (_player.speed == _player.runspeed && !_player.hasconstantmotion && moving && shiftpressed)
            {
                if (DateTime.now.timeofday.totalseconds - _lastautoshot > 0.1f)
                {
                    _lastautoshot = (float)DateTime.now.timeofday.totalseconds;
                    _trails.add(new trail { position = _player.position, imer = 0, maximer = 1f });
                }
            }

            for (int i = _trails.count - 1; i >= 0; i--)
            {
                _trails[i].imer += dt;
                if (_trails[i].imer >= _trails[i].maximer)
                    _trails.removeat(i);
            }

            // ????
            updateweaponreload(dt);
            _waveSystem.update(dt, _enemies, ref _enemyBullets);
            updatebullets(dt);
            updateenemybullets(dt);
            updatecooldowns(dt);
            _abilitySystem.update(dt, _player, _enemies);
            _throwableSystem.update(dt, _throwableFields, _player, _enemies, this);
        }

        private void switchweapon(int slot)
        {
            var weapontype = _player.weaponslots[slot];
            if (string.isnullorempty(weapontype)) return;

            if (_player.weaponequipped && _player.weapontype == weapontype)
            {
                var weapon = _player.getcurrentweapon();
                if (weapon != null) weapon.isreloading = false;
                _player.weaponequipped = false;
                _player.weapontype = null;
            }
            else
            {
                if (_player.weaponequipped)
                {
                    var oldweapon = _player.getcurrentweapon();
                    if (oldweapon != null) oldweapon.isreloading = false;
                }
                _player.weaponequipped = true;
                _player.weapontype = weapontype;
                _currentcooldown = 0;
            }
        }

        private void reloadweapon()
        {
            var weapon = _player.getcurrentweapon();
            if (weapon == null) return;
            if (weapon.isreloading) return;
            if (weapon.ammo >= weapon.maxammo) return;

            weapon.isreloading = true;
            weapon.reloadimer = weapon.reloadtime;
        }

        private void updateweaponreload(float dt)
        {
            var weapon = _player.getcurrentweapon();
            if (weapon != null && weapon.isreloading)
            {
                weapon.reloadimer -= dt;
                if (weapon.reloadimer <= 0)
                {
                    weapon.isreloading = false;
                    weapon.ammo = weapon.maxammo;
                }
            }
        }

        private void updatecooldowns(float dt)
        {
            if (_currentcooldown > 0)
                _currentcooldown -= dt;

            for (int i = 0; i < _player.activecooldowns.length; i++)
            {
                if (_player.activecooldowns[i] > 0)
                    _player.activecooldowns[i] -= dt;
            }
        }

        private void shootweapon()
        {
            if (_currentcooldown > 0) return;

            var weapon = _player.getcurrentweapon();
            if (weapon == null) return;
            if (weapon.isreloading) return;

            if (_player.weapontype == "soulreaper")
            {
                if (_player.soulreapercharge < 500) return;
                _player.soulreapercharge = 0;
                _currentcooldown = weapon.cooldownmax;

                var bullet = _objectPool.getbullet();
                if (bullet != null)
                {
                    var dir = new Vector2((float)math.cos(_currentangle), (float)math.sin(_currentangle));
                    var pos = _player.center + dir * weapon.distance * 1.5f;
                    bullet.initialize(pos, dir, weapon.bulletspeed, weapon.damage, weapon.bulletsize, weapon.Color, weapon.pierce, weapon.maxpierce);
                    bullet.sourceweapon = "soulreaper";
                    _bullets.add(bullet);
                }
            }
            else if (_player.weapontype == "lasergun")
            {
                if (_player.laserguncharge < 1) return;
                _player.laserguncharge -= 1;
                _currentcooldown = weapon.cooldownmax;
                _player.lastlasershottime = (float)DateTime.now.timeofday.totalseconds;

                var mouse = MouseState.getstate();
                var cameraworld = _player.center - new Vector2(graphicsdevice.viewport.width / 2, graphicsdevice.viewport.height / 2);
                var worldmouse = new Vector2(mouse.x, mouse.y) + cameraworld;
                var direction = worldmouse - _player.center;
                if (direction != Vector2.zero) direction.normalize();

                for (int i = _enemies.count - 1; i >= 0; i--)
                {
                    var e = _enemies[i];
                    var toenemy = e.center - _player.center;
                    var t = Vector2.dot(toenemy, direction);
                    if (t > 0 && t < 1000)
                    {
                        var proj = _player.center + direction * t;
                        var perpdist = Vector2.distance(e.center, proj);
                        if (perpdist <= 20)
                        {
                            var damage = weapon.damage;
                            if (e.type.contains("boss") && _player.hasdragonslayer)
                                damage *= 1.1f;
                            e.takedamage(damage);
                            if (e.health <= 0)
                                handleenemydeath(e, "lasergun");
                        }
                    }
                }
            }
            else if (_player.weapontype == "feast")
            {
                if (weapon.ammo <= 0) return;
                weapon.ammo--;
                _currentcooldown = weapon.cooldownmax;

                var bullet = _objectPool.getfeastbullet();
                if (bullet != null)
                {
                    var dir = new Vector2((float)math.cos(_currentangle), (float)math.sin(_currentangle));
                    var pos = _player.center + dir * weapon.distance * 1.5f;
                    bullet.initialize(pos, dir, weapon.bulletspeed, weapon.damage, weapon.bulletsize, weapon.Color,
                        weapon.feastduration, weapon.feastradius, weapon.feastdamagepersecond, weapon.feastexplosiondamage);
                    _bullets.add(bullet);
                }
            }
            else if (_player.weapontype == "lifedrain")
            {
                if (weapon.ammo <= 0) return;
                weapon.ammo--;
                _currentcooldown = weapon.cooldownmax;

                for (int i = 0; i < weapon.burstcount; i++)
                {
                    var bullet = _objectPool.getbullet();
                    if (bullet != null)
                    {
                        var dir = new Vector2((float)math.cos(_currentangle), (float)math.sin(_currentangle));
                        var pos = _player.center + dir * weapon.distance * 1.5f;
                        var damage = weapon.damage + _player.lifedraindamagebonus;
                        bullet.initialize(pos, dir, weapon.bulletspeed, damage, weapon.bulletsize, weapon.Color, false, 0);
                        bullet.sourceweapon = "lifedrain";
                        _bullets.add(bullet);
                    }
                }
            }
            else
            {
                if (weapon.ammo <= 0) return;
                weapon.ammo--;
                _currentcooldown = weapon.cooldownmax;

                var bullet = _objectPool.getbullet();
                if (bullet != null)
                {
                    var dir = new Vector2((float)math.cos(_currentangle), (float)math.sin(_currentangle));
                    var pos = _player.center + dir * weapon.distance * 1.5f;
                    bullet.initialize(pos, dir, weapon.bulletspeed, weapon.damage, weapon.bulletsize, weapon.Color, weapon.pierce, weapon.maxpierce);
                    bullet.sourceweapon = _player.weapontype;
                    _bullets.add(bullet);
                }
            }
        }

        private void updatebullets(float dt)
        {
            for (int i = _bullets.count - 1; i >= 0; i--)
            {
                var b = _bullets[i];
                b.update(dt);

                if (b is feastbullet feast && feast.shouldexplode())
                {
                    var explosionpos = feast.position;
                    var explosionradius = 120f;
                    var explosiondamage = feast.feastexplosiondamage;

                    for (int j = _enemies.count - 1; j >= 0; j--)
                    {
                        var e = _enemies[j];
                        if (Vector2.distance(e.center, explosionpos) < explosionradius)
                        {
                            e.takedamage(explosiondamage);
                            if (e.health <= 0)
                                handleenemydeath(e, "feast");
                        }
                    }

                    var field = _objectPool.geteffectfield();
                    if (field != null)
                    {
                        field.type = "explosion";
                        field.position = explosionpos;
                        field.radius = explosionradius;
                        field.duration = 0.5f;
                        field.active = true;
                        _throwableFields.add(field);
                    }

                    _objectPool.returnbullet(b);
                    _bullets.removeat(i);
                    continue;
                }

                if (b.lifetime <= 0)
                {
                    _objectPool.returnbullet(b);
                    _bullets.removeat(i);
                    continue;
                }

                bool hit = false;
                float totaldamage = 0;

                for (int j = _enemies.count - 1; j >= 0; j--)
                {
                    var e = _enemies[j];
                    if (Vector2.distance(b.position, e.center) <= e.size / 2 + b.size)
                    {
                        if (b.hitenemies.contains(j)) continue;

                        var damage = b.damage;
                        if (b.sourceweapon == "soulreaper" && b.piercecount > 0)
                            damage /= (float)math.pow(2, b.piercecount);

                        if (e.type.contains("boss") && _player.hasdragonslayer)
                            damage *= 1.1f;

                        totaldamage += damage;
                        e.takedamage(damage);
                        b.hitenemies.add(j);

                        if (b.pierce)
                        {
                            b.piercecount++;
                            if (b.piercecount >= b.maxpierce)
                            {
                                _objectPool.returnbullet(b);
                                _bullets.removeat(i);
                                hit = true;
                                break;
                            }
                        }
                        else
                        {
                            _objectPool.returnbullet(b);
                            _bullets.removeat(i);
                            hit = true;
                            break;
                        }

                        if (e.health <= 0)
                            handleenemydeath(e, b.sourceweapon);
                    }
                }

                if (totaldamage > 0 && _player.siphonactive)
                {
                    var heal = (int)(totaldamage / 2);
                    _player.health = math.min(_player.maxhealth, _player.health + heal);
                }

                if (!hit && b.pierce && b.lifetime <= 0)
                {
                    _objectPool.returnbullet(b);
                    _bullets.removeat(i);
                }
            }
        }

        private void updateenemybullets(float dt)
        {
            for (int i = _enemyBullets.count - 1; i >= 0; i--)
            {
                var b = _enemyBullets[i];
                b.update(dt, _player.center);

                if (Vector2.distance(b.position, _player.center) < 15)
                {
                    _player.takedamage(b.damage);
                    _enemyBullets.removeat(i);
                }
                else if (b.lifetime <= 0)
                {
                    _enemyBullets.removeat(i);
                }
            }
        }

        private void handleenemydeath(enemy e, string sourceweapon)
        {
            _maxbits += e.score;
            _bits = _maxbits;

            if (sourceweapon != "soulreaper" && _player.hassoulreaper)
            {
                var chargegain = (int)(e.maxhealth / 2);
                _player.soulreapercharge = math.min(500, _player.soulreapercharge + chargegain);
            }

            if (_player.hasbloodthirst)
            {
                var heal = e.type.contains("boss") ? 20 : 1;
                _player.health = math.min(_player.maxhealth, _player.health + heal);
            }

            if (sourceweapon == "lifedrain" && _player.haslifedrain)
            {
                _player.lifedraindamagebonus++;
                _player.lifedrainkillcounter++;
                if (_player.lifedrainkillcounter >= 25)
                {
                    _player.lifedrainkillcounter = 0;
                    _player.maxhealth++;
                    _player.health++;
                }
            }

            _enemies.remove(e);
            _objectPool.returnenemy(e);
        }

        private void usethrowable(int slot)
        {
            var mouse = MouseState.getstate();
            var cameraworld = _player.center - new Vector2(graphicsdevice.viewport.width / 2, graphicsdevice.viewport.height / 2);
            var worldmouse = new Vector2(mouse.x, mouse.y) + cameraworld;
            var dir = worldmouse - _player.center;
            if (dir != Vector2.zero) dir.normalize();
            var target = _player.center + dir * 300;

            target.x = math.clamp(target.x, _mapminx + 20, _mapmaxx - 20);
            target.y = math.clamp(target.y, _mapminy + 20, _mapmaxy - 20);

            _throwableSystem.usethrowable(slot, target);
        }

        private void useactiveskill(int slot)
        {
            var skillid = _player.activeskillslots[slot];
            if (string.isnullorempty(skillid)) return;

            if (_player.activecooldowns[slot] > 0) return;

            bool success = false;
            switch (skillid)
            {
                case "heal":
                    if (_player.health < _player.maxhealth)
                    {
                        _player.heal(25);
                        success = true;
                    }
                    break;
                case "harvest":
                    success = true;
                    for (int i = _enemies.count - 1; i >= 0; i--)
                    {
                        var e = _enemies[i];
                        if (e.health / e.maxhealth < 0.05f)
                        {
                            e.health = 0;
                            handleenemydeath(e, "harvest");
                        }
                        else
                        {
                            e.takedamage(50);
                            if (e.health <= 0)
                                handleenemydeath(e, "harvest");
                        }
                    }
                    break;
                case "siphon":
                    if (!_player.siphonactive)
                    {
                        _player.siphonactive = true;
                        _player.siphonimer = 10;
                        success = true;
                    }
                    break;
                case "forcefield":
                    var center = _player.center;
                    for (int i = 0; i < _enemies.count; i++)
                    {
                        var e = _enemies[i];
                        if (Vector2.distance(e.center, center) < 40)
                        {
                            var dir = e.center - center;
                            if (dir != Vector2.zero) dir.normalize();
                            e.position += dir * 100;
                            e.stunned = true;
                            e.stunimer = 1.5f;
                            e.originalspeed = e.speed;
                            e.speed = 0;
                        }
                    }
                    success = true;
                    break;
                case "degradation":
                    for (int i = 0; i < _enemies.count; i++)
                    {
                        var e = _enemies[i];
                        var healthpercent = e.health / e.maxhealth;
                        if (e.type.contains("boss"))
                        {
                            e.type = "boss_easy";
                            e.maxhealth = 300;
                            e.health = math.max(1, (int)(300 * healthpercent));
                            e.damage = 40;
                        }
                        else
                        {
                            e.type = "tank";
                            e.maxhealth = 80;
                            e.health = math.max(1, (int)(80 * healthpercent));
                            e.damage = 30;
                        }
                    }
                    success = true;
                    break;
            }

            if (success)
            {
                var cooldown = skillid switch
                {
                    "heal" => 30,
                    "harvest" => 90,
                    "siphon" => 80,
                    "forcefield" => 30,
                    "degradation" => 150,
                    _ => 0
                };
                _player.activecooldowns[slot] = cooldown;
            }
        }

        private void clampplayer()
        {
            _player.position.x = math.clamp(_player.position.x, _mapminx, _mapmaxx - 20);
            _player.position.y = math.clamp(_player.position.y, _mapminy, _mapmaxy - 20);

            if (_mapexpanded)
            {
                var left = _obstacle.x - _obstacle.width / 2;
                var right = _obstacle.x + _obstacle.width / 2;
                var top = _obstacle.y - _obstacle.height / 2;
                var bottom = _obstacle.y + _obstacle.height / 2;

                if (_player.position.x >= left && _player.position.x <= right &&
                    _player.position.y >= top && _player.position.y <= bottom)
                {
                    var leftdist = math.abs(_player.position.x - left);
                    var rightdist = math.abs(_player.position.x - right);
                    var topdist = math.abs(_player.position.y - top);
                    var bottomdist = math.abs(_player.position.y - bottom);
                    var mindist = math.min(leftdist, rightdist, topdist, bottomdist);

                    if (mindist == leftdist) _player.position.x = left - 1;
                    else if (mindist == rightdist) _player.position.x = right + 1;
                    else if (mindist == topdist) _player.position.y = top - 1;
                    else _player.position.y = bottom + 1;
                }
            }
        }

        private void expandmap()
        {
            if (_mapexpanded) return;
            _mapwidth = 1750;
            _mapheight = 1750;
            _mapminx = -875;
            _mapmaxx = 875;
            _mapminy = -875;
            _mapmaxy = 875;
            _player.position = new Vector2(0, 250);
            _mapexpanded = true;
        }

        public void startnewgame()
        {
            _player.reset(selecteddifficulty);
            _player.applymods();
            _abilitySystem.applypassiveeffects(_player);

            _waveSystem.start(1, selecteddifficulty);
            _enemies.clear();
            _bullets.clear();
            _enemyBullets.clear();
            _throwableFields.clear();
            _trails.clear();
            _bits = 0;

            _mapwidth = 1000;
            _mapheight = 1000;
            _mapminx = -500;
            _mapmaxx = 500;
            _mapminy = -500;
            _mapmaxy = 500;
            _mapexpanded = false;

            currentstate = gamestate.playing;
            paused = false;
        }

        private void restartgame()
        {
            _dataManager.save(_player, _maxbits);
            startnewgame();
        }

        private void handleescape()
        {
            switch (currentstate)
            {
                case gamestate.playing:
                    paused = !paused;
                    break;
                case gamestate.menu:
                    _dataManager.save(_player, _maxbits);
                    exit();
                    break;
                default:
                    currentstate = gamestate.menu;
                    break;
            }
        }

        public void addbits(int amount)
        {
            _maxbits += amount;
            _bits += amount;
        }

        protected override void draw(GameTime GameTime)
        {
            graphicsdevice.clear(Color.black);

            _spritebatch.begin();

            switch (currentstate)
            {
                case gamestate.menu:
                    drawmenu();
                    break;
                case gamestate.difficulty:
                    drawdifficulty();
                    break;
                case gamestate.playing:
                    drawgameplaying();
                    if (paused) drawpausemenu();
                    break;
                case gamestate.inventory:
                    _inventorySystem.draw(_spritebatch, _player, _maxbits);
                    break;
                case gamestate.bestiary:
                    _menuManager.drawbestiary(_spritebatch);
                    break;
                case gamestate.shop:
                    _shopSystem.draw(_spritebatch, _player, _maxbits);
                    break;
            }

            _spritebatch.end();

            base.draw(GameTime);
        }

        private void drawmenu()
        {
            var w = graphicsdevice.viewport.width;
            var h = graphicsdevice.viewport.height;
            var bw = 200;
            var bh = 50;
            var starty = h / 2 - 100;

            var title = "purge";
            var titlesize = _titlefont.measurestring(title);
            _spritebatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, 100), Color.white);

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
                primitives2d.fillrectangle(_spritebatch, new Rectangle(x, btn.y, bw, bh), Color.cornflowerblue, 10);
                _spritebatch.drawstring(_font, btn.text, new Vector2(x + bw / 2 - _font.measurestring(btn.text).x / 2, btn.y + 12), Color.white);
            }

            _spritebatch.drawstring(_smallfont, $"{_loc.get("bits")}: {_maxbits}", new Vector2(10, 10), Color.yellow);
        }

        private void drawdifficulty()
        {
            var w = graphicsdevice.viewport.width;
            var h = graphicsdevice.viewport.height;
            var bw = 200;
            var bh = 50;
            var starty = h / 2 - 120;

            var title = _loc.get("select_difficulty");
            var titlesize = _titlefont.measurestring(title);
            _spritebatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, 100), Color.white);

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
                primitives2d.fillrectangle(_spritebatch, new Rectangle(x, d.y, bw, bh), Color.cornflowerblue, 10);
                _spritebatch.drawstring(_font, d.text, new Vector2(x + bw / 2 - _font.measurestring(d.text).x / 2, d.y + 12), Color.white);
            }

            _spritebatch.drawstring(_smallfont, _loc.get("press_esc_return"), new Vector2(10, h - 30), Color.lightgray);
        }

        private void drawpausemenu()
        {
            var w = graphicsdevice.viewport.width;
            var h = graphicsdevice.viewport.height;

            var title = _loc.get("paused");
            var titlesize = _titlefont.measurestring(title);
            _spritebatch.drawstring(_titlefont, title, new Vector2(w / 2 - titlesize.x / 2, h / 2 - 100), Color.white);

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
                _spritebatch.drawstring(_font, item.text, new Vector2(x, item.y), Color.white);
            }
        }

        private void drawgameplaying()
        {
            // ??????
            var camerapos = _player.center - new Vector2(graphicsdevice.viewport.width / 2, graphicsdevice.viewport.height / 2);
            var transform = matrix.createtranslation(-camerapos.x, -camerapos.y, 0);
            _spritebatch.end();
            _spritebatch.begin(transformmatrix: transform);

            // ????
            primitives2d.drawrectangle(_spritebatch, new Rectangle((int)_mapminx, (int)_mapminy, _mapwidth, _mapheight), Color.white * 0.3f, 2);

            if (_mapexpanded)
            {
                primitives2d.fillrectangle(_spritebatch, _obstacle, Color.gray * 0.5f);
                primitives2d.drawrectangle(_spritebatch, _obstacle, Color.white, 2);
            }

            // ??
            foreach (var b in _bullets)
                b.draw(_spritebatch, _pixel);
            foreach (var b in _enemyBullets)
                b.draw(_spritebatch, _pixel);

            // ??
            foreach (var e in _enemies)
                e.draw(_spritebatch, _pixel, _pixel);

            // ?????
            foreach (var f in _throwableFields)
                f.draw(_spritebatch, _pixel);

            // ??
            foreach (var t in _trails)
            {
                var alpha = 1 - t.imer / t.maximer;
                primitives2d.drawrectangle(_spritebatch, new Rectangle((int)t.position.x, (int)t.position.y, 20, 20), Color.white * alpha, 2);
            }

            // ??
            _player.draw(_spritebatch, _pixel, _currentangle);

            _spritebatch.end();
            _spritebatch.begin();

            // UI
            _uiManager.draw(_spritebatch, _player, _waveSystem, _bits, _maxbits, _mapwidth, _mapheight, _enemies.count);

            if (_player.isdead)
            {
                var text = _loc.get("you_died");
                var size = _titlefont.measurestring(text);
                _spritebatch.drawstring(_titlefont, text, new Vector2(graphicsdevice.viewport.width / 2 - size.x / 2, graphicsdevice.viewport.height / 2 - 70), Color.red);
                text = _loc.get("press_x_restart");
                size = _font.measurestring(text);
                _spritebatch.drawstring(_font, text, new Vector2(graphicsdevice.viewport.width / 2 - size.x / 2, graphicsdevice.viewport.height / 2 + 10), Color.white);
            }

            _spritebatch.drawstring(_smallfont, _loc.get("press_m_for_menu"), new Vector2(10, graphicsdevice.viewport.height - 30), Color.gray);
        }

        // ????
        public GraphicsDeviceManager graphicsmanager => _graphics;
        public SpriteBatch SpriteBatch => _spritebatch;
        public gameconfig config => _config;
        public localization localization => _loc;
        public objectpool objectpool => _objectPool;
        public wavesystem wavesystem => _waveSystem;
        public abilitysystem abilitysystem => _abilitySystem;
        public throwablesystem throwablesystem => _throwableSystem;
        public shopsystem shopsystem => _shopSystem;
        public inventorysystem inventorysystem => _inventorySystem;
        public uimanager uimanager => _uiManager;
        public player player => _player;
        public List<enemy> enemies => _enemies;
        public List<bullet> bullets => _bullets;
        public List<enemybullet> enemybullets => _enemyBullets;
        public List<throwablefield> throwablefields => _throwableFields;
        public int bits { get => _bits; set => _bits = value; }
        public int maxbits { get => _maxbits; set => _maxbits = value; }
        public float currentangle { get => _currentangle; set => _currentangle = value; }

        private class trail
        {
            public Vector2 position;
            public float imer;
            public float maximer;
        }
    }
}
