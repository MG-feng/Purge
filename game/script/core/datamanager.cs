using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.core
{
    public class savedata
    {
        public int maxbits { get; set; }
        public bool hasrifle { get; set; }
        public bool hassniper { get; set; }
        public bool hassoulreaper { get; set; }
        public bool haslasergun { get; set; }
        public bool hasfeast { get; set; }
        public bool haslifedrain { get; set; }
        public bool pistol_fastmag { get; set; }
        public bool pistol_extmag { get; set; }
        public bool pistol_damage { get; set; }
        public bool rifle_fastmag { get; set; }
        public bool rifle_extmag { get; set; }
        public bool rifle_damage { get; set; }
        public bool sniper_damage { get; set; }
        public bool sniper_pierce { get; set; }
        public bool soulreaper_pierce { get; set; }
        public bool soulreaper_damage { get; set; }
        public bool lasergun_capacity { get; set; }
        public bool feast_dualcore { get; set; }
        public bool feast_highexplosive { get; set; }
        public bool lifedrain_extendedmag { get; set; }
        public bool lifedrain_soulnourish { get; set; }
        public bool speedwalk { get; set; }
        public bool speedrun { get; set; }
        public bool gravityanchor { get; set; }
        public bool soulshard { get; set; }
        public bool phasebeacon { get; set; }
        public string[] weaponslots { get; set; } = new string[4];
        public string[] activeskillslots { get; set; } = new string[4];
        public string[] passiveskillslots { get; set; } = new string[2];
        public string[] throwableslots { get; set; } = new string[3];
        public string[] ownedskills { get; set; } = new string[0];
    }

    public class datamanager
    {
        private gameconfig _config;
        private string _savepath;

        public datamanager()
        {
            _config = new gameconfig();
            var datapath = path.combine(appdomain.currentdomain.basedirectory, "game", "data");
            if (!directory.exists(datapath))
                directory.createdirectory(datapath);
            _savepath = path.combine(datapath, "save.dat");
        }

        public void save(player player, int maxbits)
        {
            try
            {
                var data = new savedata
                {
                    maxbits = maxbits,
                    hasrifle = player.hasrifle,
                    hassniper = player.hassniper,
                    hassoulreaper = player.hassoulreaper,
                    haslasergun = player.haslasergun,
                    hasfeast = player.hasfeast,
                    haslifedrain = player.haslifedrain,
                    pistol_fastmag = player.pistol_fastmag,
                    pistol_extmag = player.pistol_extmag,
                    pistol_damage = player.pistol_damage,
                    rifle_fastmag = player.rifle_fastmag,
                    rifle_extmag = player.rifle_extmag,
                    rifle_damage = player.rifle_damage,
                    sniper_damage = player.sniper_damage,
                    sniper_pierce = player.sniper_pierce,
                    soulreaper_pierce = player.soulreaper_pierce,
                    soulreaper_damage = player.soulreaper_damage,
                    lasergun_capacity = player.lasergun_capacity,
                    feast_dualcore = player.feast_dualcore,
                    feast_highexplosive = player.feast_highexplosive,
                    lifedrain_extendedmag = player.lifedrain_extendedmag,
                    lifedrain_soulnourish = player.lifedrain_soulnourish,
                    speedwalk = player.speedwalk,
                    speedrun = player.speedrun,
                    gravityanchor = player.gravityanchor,
                    soulshard = player.soulshard,
                    phasebeacon = player.phasebeacon,
                    weaponslots = player.weaponslots,
                    activeskillslots = player.activeskillslots,
                    passiveskillslots = player.passiveskillslots,
                    throwableslots = player.throwableslots,
                    ownedskills = player.ownedskills.ToArray()
                };

                var json = jsonserializer.serialize(data);
                var encrypted = encrypt(json);

                // 原子写入：先写临时文件再重命名
                var temppath = _savepath + ".tmp";
                file.writealltext(temppath, encrypted);
                file.move(temppath, _savepath, true);
            }
            catch (Exception e)
            {
                system.diagnostics.debug.writeline($"failed to save: {e.message}");
            }
        }

        public void load(player player, ref int maxbits)
        {
            try
            {
                if (!file.exists(_savepath))
                {
                    setdefaults(player, ref maxbits);
                    return;
                }

                var encrypted = file.readalltext(_savepath);
                var json = decrypt(encrypted);
                var data = jsonserializer.deserialize<savedata>(json);

                if (data != null)
                {
                    maxbits = data.maxbits;
                    player.hasrifle = data.hasrifle;
                    player.hassniper = data.hassniper;
                    player.hassoulreaper = data.hassoulreaper;
                    player.haslasergun = data.haslasergun;
                    player.hasfeast = data.hasfeast;
                    player.haslifedrain = data.haslifedrain;
                    player.pistol_fastmag = data.pistol_fastmag;
                    player.pistol_extmag = data.pistol_extmag;
                    player.pistol_damage = data.pistol_damage;
                    player.rifle_fastmag = data.rifle_fastmag;
                    player.rifle_extmag = data.rifle_extmag;
                    player.rifle_damage = data.rifle_damage;
                    player.sniper_damage = data.sniper_damage;
                    player.sniper_pierce = data.sniper_pierce;
                    player.soulreaper_pierce = data.soulreaper_pierce;
                    player.soulreaper_damage = data.soulreaper_damage;
                    player.lasergun_capacity = data.lasergun_capacity;
                    player.feast_dualcore = data.feast_dualcore;
                    player.feast_highexplosive = data.feast_highexplosive;
                    player.lifedrain_extendedmag = data.lifedrain_extendedmag;
                    player.lifedrain_soulnourish = data.lifedrain_soulnourish;
                    player.speedwalk = data.speedwalk;
                    player.speedrun = data.speedrun;
                    player.gravityanchor = data.gravityanchor;
                    player.soulshard = data.soulshard;
                    player.phasebeacon = data.phasebeacon;
                    player.weaponslots = data.weaponslots ?? new string[4];
                    player.activeskillslots = data.activeskillslots ?? new string[4];
                    player.passiveskillslots = data.passiveskillslots ?? new string[2];
                    player.throwableslots = data.throwableslots ?? new string[3];
                    player.ownedskills = new HashSet<string>(data.ownedskills ?? new string[0]);
                }
                else
                {
                    setdefaults(player, ref maxbits);
                }
            }
            catch
            {
                setdefaults(player, ref maxbits);
            }
        }

        private void setdefaults(player player, ref int maxbits)
        {
            maxbits = 0;
            player.hasrifle = false;
            player.hassniper = false;
            player.hassoulreaper = false;
            player.haslasergun = false;
            player.hasfeast = false;
            player.haslifedrain = false;
            player.weaponslots = new string[4];
            player.weaponslots[0] = "pistol";
            player.activeskillslots = new string[4];
            player.passiveskillslots = new string[2];
            player.throwableslots = new string[3];
            player.ownedskills = new HashSet<string>();
        }

        private string encrypt(string text)
        {
            using (var aes = aes.create())
            {
                aes.key = encoding.utf8.getbytes(_config.save_key.padright(32).substring(0, 32));
                aes.iv = new byte[16];

                using (var encryptor = aes.createencryptor())
                using (var ms = new memorystream())
                using (var cs = new cryptostream(ms, encryptor, cryptostreammode.write))
                using (var sw = new streamwriter(cs))
                {
                    sw.write(text);
                    sw.flush();
                    cs.flushfinalblock();
                    return convert.tobase64string(ms.toarray());
                }
            }
        }

        private string decrypt(string encrypted)
        {
            using (var aes = aes.create())
            {
                aes.key = encoding.utf8.getbytes(_config.save_key.padright(32).substring(0, 32));
                aes.iv = new byte[16];

                var buffer = convert.frombase64string(encrypted);
                using (var ms = new memorystream(buffer))
                using (var cs = new cryptostream(ms, aes.createdecryptor(), cryptostreammode.read))
                using (var sr = new streamreader(cs))
                {
                    return sr.readtoend();
                }
            }
        }
    }
}
