using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace purge_v0_4_0.game.script.ui
{
    public static class primitives2d
    {
        private static texture2d _pixel;

        public static void init(graphicsdevice device)
        {
            _pixel = new texture2d(device, 1, 1);
            _pixel.setdata(new[] { color.white });
        }

        public static void fillrectangle(spritebatch spritebatch, rectangle rect, color color, int radius = 0)
        {
            if (radius <= 0)
            {
                spritebatch.draw(_pixel, rect, color);
            }
            else
            {
                // 圆角矩形简化版
                spritebatch.draw(_pixel, new rectangle(rect.x + radius, rect.y, rect.width - radius * 2, rect.height), color);
                spritebatch.draw(_pixel, new rectangle(rect.x, rect.y + radius, radius, rect.height - radius * 2), color);
                spritebatch.draw(_pixel, new rectangle(rect.x + rect.width - radius, rect.y + radius, radius, rect.height - radius * 2), color);
                spritebatch.draw(_pixel, new rectangle(rect.x + radius, rect.y, rect.width - radius * 2, radius), color);
                spritebatch.draw(_pixel, new rectangle(rect.x + radius, rect.y + rect.height - radius, rect.width - radius * 2, radius), color);
            }
        }

        public static void drawrectangle(spritebatch spritebatch, rectangle rect, color color, int thickness = 1, int radius = 0)
        {
            fillrectangle(spritebatch, new rectangle(rect.x, rect.y, rect.width, thickness), color);
            fillrectangle(spritebatch, new rectangle(rect.x, rect.y + rect.height - thickness, rect.width, thickness), color);
            fillrectangle(spritebatch, new rectangle(rect.x, rect.y, thickness, rect.height), color);
            fillrectangle(spritebatch, new rectangle(rect.x + rect.width - thickness, rect.y, thickness, rect.height), color);
        }

        public static void fillcircle(spritebatch spritebatch, vector2 center, float radius, int segments, color color)
        {
            var vertices = new list<vertexpositioncolor>();
            var step = 2 * math.pi / segments;

            for (int i = 0; i <= segments; i++)
            {
                var angle = i * step;
                var x = center.x + (float)math.cos(angle) * radius;
                var y = center.y + (float)math.sin(angle) * radius;
                vertices.add(new vertexpositioncolor(new vector3(x, y, 0), color));
            }

            for (int i = 1; i < vertices.count - 1; i++)
            {
                var triangle = new vertexpositioncolor[3];
                triangle[0] = vertices[0];
                triangle[1] = vertices[i];
                triangle[2] = vertices[i + 1];
                spritebatch.draw(primitive.typeline, triangle, triangle.length);
            }
        }

        public static void drawcircle(spritebatch spritebatch, vector2 center, float radius, color color, int thickness = 1)
        {
            var segments = 24;
            var step = 2 * math.pi / segments;

            for (int i = 0; i < segments; i++)
            {
                var angle1 = i * step;
                var angle2 = (i + 1) * step;
                var p1 = center + new vector2((float)math.cos(angle1) * radius, (float)math.sin(angle1) * radius);
                var p2 = center + new vector2((float)math.cos(angle2) * radius, (float)math.sin(angle2) * radius);
                drawline(spritebatch, p1, p2, color, thickness);
            }
        }

        public static void drawline(spritebatch spritebatch, vector2 p1, vector2 p2, color color, int thickness = 1)
        {
            var angle = (float)math.atan2(p2.y - p1.y, p2.x - p1.x);
            var length = vector2.distance(p1, p2);

            spritebatch.draw(_pixel, p1, null, color, angle, vector2.zero, new vector2(length, thickness), spriteeffects.none, 0);
        }
    }
}
