using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.ui
{
    public static class primitives2d
    {
        private static Texture2D _pixel;

        public static void init(GraphicsDevice device)
        {
            _pixel = new Texture2D(device, 1, 1);
            _pixel.setdata(new[] { Color.white });
        }

        public static void fillrectangle(SpriteBatch SpriteBatch, Rectangle rect, Color Color, int radius = 0)
        {
            if (radius <= 0)
            {
                SpriteBatch.draw(_pixel, rect, Color);
            }
            else
            {
                // 圆角矩形简化版
                SpriteBatch.draw(_pixel, new Rectangle(rect.x + radius, rect.y, rect.width - radius * 2, rect.height), Color);
                SpriteBatch.draw(_pixel, new Rectangle(rect.x, rect.y + radius, radius, rect.height - radius * 2), Color);
                SpriteBatch.draw(_pixel, new Rectangle(rect.x + rect.width - radius, rect.y + radius, radius, rect.height - radius * 2), Color);
                SpriteBatch.draw(_pixel, new Rectangle(rect.x + radius, rect.y, rect.width - radius * 2, radius), Color);
                SpriteBatch.draw(_pixel, new Rectangle(rect.x + radius, rect.y + rect.height - radius, rect.width - radius * 2, radius), Color);
            }
        }

        public static void drawrectangle(SpriteBatch SpriteBatch, Rectangle rect, Color Color, int thickness = 1, int radius = 0)
        {
            fillrectangle(SpriteBatch, new Rectangle(rect.x, rect.y, rect.width, thickness), Color);
            fillrectangle(SpriteBatch, new Rectangle(rect.x, rect.y + rect.height - thickness, rect.width, thickness), Color);
            fillrectangle(SpriteBatch, new Rectangle(rect.x, rect.y, thickness, rect.height), Color);
            fillrectangle(SpriteBatch, new Rectangle(rect.x + rect.width - thickness, rect.y, thickness, rect.height), Color);
        }

        public static void fillcircle(SpriteBatch SpriteBatch, Vector2 center, float radius, int segments, Color Color)
        {
            var vertices = new List<vertexpositioncolor>();
            var step = 2 * math.pi / segments;

            for (int i = 0; i <= segments; i++)
            {
                var angle = i * step;
                var x = center.x + (float)math.cos(angle) * radius;
                var y = center.y + (float)math.sin(angle) * radius;
                vertices.add(new vertexpositioncolor(new vector3(x, y, 0), Color));
            }

            for (int i = 1; i < vertices.count - 1; i++)
            {
                var triangle = new vertexpositioncolor[3];
                triangle[0] = vertices[0];
                triangle[1] = vertices[i];
                triangle[2] = vertices[i + 1];
                SpriteBatch.draw(primitive.typeline, triangle, triangle.length);
            }
        }

        public static void drawcircle(SpriteBatch SpriteBatch, Vector2 center, float radius, Color Color, int thickness = 1)
        {
            var segments = 24;
            var step = 2 * math.pi / segments;

            for (int i = 0; i < segments; i++)
            {
                var angle1 = i * step;
                var angle2 = (i + 1) * step;
                var p1 = center + new Vector2((float)math.cos(angle1) * radius, (float)math.sin(angle1) * radius);
                var p2 = center + new Vector2((float)math.cos(angle2) * radius, (float)math.sin(angle2) * radius);
                drawline(SpriteBatch, p1, p2, Color, thickness);
            }
        }

        public static void drawline(SpriteBatch SpriteBatch, Vector2 p1, Vector2 p2, Color Color, int thickness = 1)
        {
            var angle = (float)math.atan2(p2.y - p1.y, p2.x - p1.x);
            var length = Vector2.distance(p1, p2);

            SpriteBatch.draw(_pixel, p1, null, Color, angle, Vector2.zero, new Vector2(length, thickness), spriteeffects.none, 0);
        }
    }
}
