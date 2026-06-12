using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.entities
{
    public class throwablefield
    {
        public string type = "";
        public vector2 position = vector2.zero;
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
            position = vector2.zero;
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

        public void draw(spritebatch spritebatch, texture2d pixel)
        {
            if (type == "gravity")
            {
                var alpha = 0.3f + 0.2f * (float)math.sin(datetime.now.timeofday.totalseconds * 3);
                primitives2d.drawcircle(spritebatch, position, radius, color.purple * alpha, 2);
                primitives2d.fillcircle(spritebatch, position, radius, 16, color.purple * (alpha * 0.3f));
                primitives2d.fillcircle(spritebatch, position, 5, 8, color.white);
            }
            else if (type == "explosion")
            {
                var progress = imer / duration;
                var alpha = 1 - progress;
                var rad = radius * (1 + progress * 0.5f);
                primitives2d.drawcircle(spritebatch, position, rad, color.orange * alpha, 3);
                primitives2d.fillcircle(spritebatch, position, rad * 0.8f, 16, color.orangered * (alpha * 0.5f));
            }
            else if (type == "beacon")
            {
                var remaining = duration - imer;
                var pulse = 0.5f + 0.5f * (float)math.sin(datetime.now.timeofday.totalseconds * 5);
                primitives2d.fillcircle(spritebatch, position, 10, 12, color.cyan * 0.8f);
                primitives2d.drawcircle(spritebatch, position, 15 + pulse * 5, color.cyan * (pulse * 0.5f), 2);
            }
            else if (type == "laser")
            {
                var endpos = position + new vector2((float)math.cos(angle), (float)math.sin(angle)) * length;
                primitives2d.drawline(spritebatch, position, endpos, color.red * 0.8f, 5);
            }
        }
    }
}
