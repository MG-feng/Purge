using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace purge_v0_4_0.game.script.core
{
    public class localization
    {
        private Dictionary<string, string> _strings;
        private string _currentlanguage;

        public localization()
        {
            _strings = new Dictionary<string, string>();
        }

        public void load(string language)
        {
            _currentlanguage = language;
            var langpath = path.combine(appdomain.currentdomain.basedirectory, "game", "lang", $"{language}.json");

            try
            {
                if (file.exists(langpath))
                {
                    var json = file.readalltext(langpath);
                    _strings = jsonserializer.deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                else
                {
                    loaddefaults();
                }
            }
            catch
            {
                loaddefaults();
            }
        }

        private void loaddefaults()
        {
            _strings = new Dictionary<string, string>
            {
                { "play", "play" },
                { "inventory", "inventory" },
                { "bestiary", "bestiary" },
                { "shop", "shop" },
                { "bits", "bits" },
                { "wave", "wave" },
                { "enemies", "enemies" },
                { "next_wave", "next wave" },
                { "game_complete", "game complete!" },
                { "map", "map" },
                { "wallet", "wallet" },
                { "you_died", "you died" },
                { "press_x_restart", "press x to restart" },
                { "press_m_for_menu", "press m for menu" },
                { "paused", "paused" },
                { "resume", "resume" },
                { "main_menu", "main menu" },
                { "quit", "quit" },
                { "select_difficulty", "select difficulty" },
                { "easy", "easy" },
                { "medium", "medium" },
                { "hard", "hard" },
                { "endless", "endless" },
                { "boss_rush", "boss rush" },
                { "press_esc_return", "press esc to return" },
                { "active_skills", "active skills" },
                { "passive_skills", "passive skills" },
                { "throwables", "throwables" },
                { "reloading", "reloading" },
                { "not_ready", "not ready" },
                { "ready_to_fire", "ready to fire" },
                { "auto_recharges", "auto-recharges after 4s" },
                { "attracts_enemies", "attracts enemies | explodes after 5s" },
                { "permanent_dmg", "permanent dmg" },
                { "press_r_reload", "press r to reload" },
                { "press_1_4_weapons", "press 1-4 for weapons" },
                { "soul_charge", "soul charge" },
                { "laser_charge", "laser charge" },
                { "feast_ammo", "feast ammo" },
                { "life_drain_ammo", "life drain ammo" },
                { "esc_return", "esc: return" },
                { "up_down_scroll", "↑/↓: scroll" },
                { "click_item_bind", "click item to bind" },
                { "owned", "owned" },
                { "bind_weapon", "press 1-4 to bind" },
                { "bind_active_skill", "press q/e/z/x to bind" },
                { "bind_throwable", "press 5-7 or click slot to bind" },
                { "empty_slot", "empty slot" },
                { "equipped", "equipped" },
                { "damage", "damage" },
                { "fire_rate", "fire rate" },
                { "ammo", "ammo" },
                { "bullet_speed", "bullet speed" },
                { "description", "description" },
                { "health", "health" },
                { "speed", "speed" },
                { "score", "score" },
                { "price", "price" },
                { "type", "type" },
                { "cooldown", "cooldown" },
                { "duration", "duration" },
                { "effect", "effect" },
                { "enemy_stats", "enemy stats" },
                { "special_enemies", "special enemies" },
                { "reward_multiplier", "reward multiplier" }
            };
        }

        public string get(string key)
        {
            return _strings.getvalueordefault(key, key);
        }

        public string this[string key] => get(key);
    }
}
