using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.entities
{
    public class bullet
    {
        public Vector2 position = Vector2.zero;
        public Vector2 direction = Vector2.zero;
        public float speed = 800f;
        public float damage = 10f;
        public float size = 5f;
        public float lifetime = 2f;
        public Color Color = Color.white;
        public string sourceweapon = "";
        public bool pierce = false;
        public int maxpierce = 0;
        public int piercecount = 0;
        public List<int> hitenemies = new List<int>();

        public bullet()
        {
        }

        public virtual void initialize(Vector2 pos, Vector2 dir, float spd, float dmg, float sz, Color col, bool piec, int maxpiec)
        {
            position = pos;
            direction = dir;
            speed = spd;
            damage = dmg;
            size = sz;
            Color = col;
            pierce = piec;
            maxpierce = maxpiec;
            lifetime = 2f;
            piercecount = 0;
            hitenemies.clear();
        }

        public virtual void reset()
        {
            position = Vector2.zero;
            direction = Vector2.zero;
            speed = 800f;
            damage = 10f;
            size = 5f;
            lifetime = 2f;
            pierce = false;
            maxpierce = 0;
            piercecount = 0;
            hitenemies.clear();
        }

        public virtual void update(float dt)
        {
            position += direction * speed * dt;
            lifetime -= dt;
        }

        public virtual void draw(SpriteBatch SpriteBatch, Texture2D pixel)
        {
            primitives2d.fillcircle(SpriteBatch, position, size / 2, 8, Color);
        }
    }

    public class feastbullet : bullet
    {
        public float feastimer = 0f;
        public float feastduration = 5f;
        public float feastradius = 80f;
        public float feastdamagepersecond = 25f;
        public float feastexplosiondamage = 1000f;
        public float damageimer = 0f;

        public void initialize(Vector2 pos, Vector2 dir, float spd, float dmg, float sz, Color col, float duration, float radius, float dot, float expdmg)
        {
            base.initialize(pos, dir, spd, dmg, sz, col, false, 0);
            feastduration = duration;
            feastradius = radius;
            feastdamagepersecond = dot;
            feastexplosiondamage = expdmg;
            feastimer = 0f;
            damageimer = 0f;
        }

        public override void reset()
        {
            base.reset();
            feastimer = 0f;
            damageimer = 0f;
            feastduration = 5f;
            feastradius = 80f;
            feastdamagepersecond = 25f;
            feastexplosiondamage = 1000f;
        }

        public override void update(float dt)
        {
            base.update(dt);
            feastimer += dt;
        }

        public bool shouldexplode => feastimer >= feastduration;
    }
}
