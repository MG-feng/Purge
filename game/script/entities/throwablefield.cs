using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.entities
{
    public class throwablefield
    {
        public string type = "";
        public Vector2 position = Vector2.zero;
        public float radius = 100f;
        public float duration = 5f;
        public float imer = 0f;
        public bool active = true;
        public float slowamount = 0.5f;
        public float angle = 0f;
        public float length = 800f;

        public void reset()
        {
            type = "";
            position = Vector2.zero;
            radius = 100f;
            duration = 5f;
            imer = 0f;
            active = true;
            slowamount = 0.5f;
            angle = 0f;
            length = 800f;
        }

        public bool update(float dt)
        {
            imer += dt;
            return imer >= duration;
        }

        public void draw(SpriteBatch SpriteBatch, Texture2D pixel)
        {
            if (type == "gravity")
            {
                var alpha = 0.3f + 0.2f * (float)math.sin(DateTime.now.timeofday.totalseconds * 3);
                primitives2d.drawcircle(SpriteBatch, position, radius, Color.purple * alpha, 2);
                primitives2d.fillcircle(SpriteBatch, position, radius, 16, Color.purple * (alpha * 0.3f));
                primitives2d.fillcircle(SpriteBatch, position, 5, 8, Color.white);
            }
            else if (type == "explosion")
            {
                var progress = imer / duration;
                var alpha = 1 - progress;
                var rad = radius * (1 + progress * 0.5f);
                primitives2d.drawcircle(SpriteBatch, position, rad, Color.orange * alpha, 3);
                primitives2d.fillcircle(SpriteBatch, position, rad * 0.8f, 16, Color.orangered * (alpha * 0.5f));
            }
            else if (type == "beacon")
            {
                var remaining = duration - imer;
                var pulse = 0.5f + 0.5f * (float)math.sin(DateTime.now.timeofday.totalseconds * 5);
                primitives2d.fillcircle(SpriteBatch, position, 10, 12, Color.cyan * 0.8f);
                primitives2d.drawcircle(SpriteBatch, position, 15 + pulse * 5, Color.cyan * (pulse * 0.5f), 2);
            }
            else if (type == "laser")
            {
                var endpos = position + new Vector2((float)math.cos(angle), (float)math.sin(angle)) * length;
                primitives2d.drawline(SpriteBatch, position, endpos, Color.red * 0.8f, 5);
            }
        }
    }
}
