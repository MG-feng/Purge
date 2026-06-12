using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.entities
{
    public class enemybullet
    {
        public vector2 position = vector2.zero;
        public vector2 direction = vector2.zero;
        public float speed = 400f;
        public float damage = 20f;
        public float size = 8f;
        public float lifetime = 3f;
        public color color = color.magenta;
        public bool ishoming = false;
        public float homingstrength = 0.02f;

        public void initialize(vector2 pos, vector2 dir, float spd, float dmg, float sz, color col)
        {
            position = pos;
            direction = dir;
            speed = spd;
            damage = dmg;
            size = sz;
            color = col;
            lifetime = 3f;
            ishoming = false;
        }

        public void update(float dt, vector2 target)
        {
            if (ishoming)
            {
                var toTarget = target - position;
                if (toTarget != vector2.zero)
                    toTarget.normalize();
                direction = direction + (toTarget - direction) * homingstrength;
                if (direction != vector2.zero)
                    direction.normalize();
            }

            position += direction * speed * dt;
            lifetime -= dt;
        }

        public void draw(spritebatch spritebatch, texture2d pixel)
        {
            primitives2d.fillcircle(spritebatch, position, size / 2, 6, color);
        }
    }
}
