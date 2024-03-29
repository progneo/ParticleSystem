﻿using System;
using System.Drawing;

namespace ParticleSystem.Particles
{
    public class Asteroid : Particle {
        public Color FromColor;
        public Color ToColor;

        public static Color MixColor(Color color1, Color color2, float k)
        {
            return Color.FromArgb(
                (int)(color2.A * k + color1.A * (1 - k)),
                (int)(color2.R * k + color1.R * (1 - k)),
                (int)(color2.G * k + color1.G * (1 - k)),
                (int)(color2.B * k + color1.B * (1 - k))
            );
        }


        public override void Draw(Graphics g)
        {
            var k = Math.Min(1f, Life / 100);

            var color = MixColor(ToColor, FromColor, k);
            var b = new SolidBrush(color);

            g.FillEllipse(b, X - Radius, Y - Radius, Radius * 2, Radius * 2); if (Selected == true)
            {
                g.DrawEllipse(
                    new Pen(Color.Aqua, 3),
                    X - Radius,
                    Y - Radius,
                    Radius * 2 + 1,
                    Radius * 2 + 1
                );
            }

            b.Dispose();
        }
    }

}
