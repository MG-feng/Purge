using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.entities
{
    public class enemybullet
    {
        public Vector2 position = Vector2.zero;
        public Vector2 direction = Vector2.zero;
        public float speed = 400f;
        public float damage = 20f;
        public float size = 8f;
        public float lifetime = 3f;
        public Color Color = Color.magenta;
        public bool ishoming = false;
        public float homingstrength = 0.02f;

        public void initialize(Vector2 pos, Vector2 dir, float spd, float dmg, float sz, Color col)
        {
            position = pos;
            direction = dir;
            speed = spd;
            damage = dmg;
            size = sz;
            Color = col;
            lifetime = 3f;
            ishoming = false;
        }

        public void update(float dt, Vector2 target)
        {
            if (ishoming)
            {
                var toTarget = target - position;
                if (toTarget != Vector2.zero)
                    toTarget.normalize();
                direction = direction + (toTarget - direction) * homingstrength;
                if (direction != Vector2.zero)
                    direction.normalize();
            }

            position += direction * speed * dt;
            lifetime -= dt;
        }

        public void draw(SpriteBatch SpriteBatch, Texture2D pixel)
        {
            primitives2d.fillcircle(SpriteBatch, position, size / 2, 6, Color);
        }
    }
}
