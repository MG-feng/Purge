using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text.Json;

namespace purge_v0_4_0.game.script.core
{
    public class settings
    {
        public string language = "en";
        public int screenwidth = 800;
        public int screenheight = 600;
        public bool fullscreen = false;
        public bool vsync = true;
        public float musicvolume = 0.5f;
        public float sfxvolume = 0.7f;
        public int maxbulletpool = 500;
        public int maxenemypool = 200;
        public bool showfps = false;
        public float camerasmoothing = 0.1f;

        private string _settingspath;

        public settings()
        {
            var datapath = path.combine(appdomain.currentdomain.basedirectory, "game", "data");
            if (!directory.exists(datapath))
                directory.createdirectory(datapath);
            _settingspath = path.combine(datapath, "settings.json");
        }

        public void load()
        {
            try
            {
                if (file.exists(_settingspath))
                {
                    var json = file.readalltext(_settingspath);
                    var loaded = jsonserializer.deserialize<settings>(json);
                    if (loaded != null)
                    {
                        language = loaded.language;
                        screenwidth = loaded.screenwidth;
                        screenheight = loaded.screenheight;
                        fullscreen = loaded.fullscreen;
                        vsync = loaded.vsync;
                        musicvolume = loaded.musicvolume;
                        sfxvolume = loaded.sfxvolume;
                        maxbulletpool = loaded.maxbulletpool;
                        maxenemypool = loaded.maxenemypool;
                        showfps = loaded.showfps;
                        camerasmoothing = loaded.camerasmoothing;
                    }
                }
            }
            catch (Exception e)
            {
                system.diagnostics.debug.writeline($"failed to load settings: {e.message}");
            }
        }

        public void save()
        {
            try
            {
                var options = new jsonserializeroptions { writeindented = true };
                var json = jsonserializer.serialize(this, options);
                file.writealltext(_settingspath, json);
            }
            catch (Exception e)
            {
                system.diagnostics.debug.writeline($"failed to save settings: {e.message}");
            }
        }
    }
}
