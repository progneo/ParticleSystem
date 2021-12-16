﻿using System;
using System.Drawing;
using ParticleSystem.Particles;

namespace ParticleSystem.Points
{
    public class SatteliteOrbitPoint : ImpactPoint
    {
        public float Diametr;
        // ReSharper disable once InconsistentNaming
        public Color Color = Color.White;
        public Planet Planet;

        public SatteliteOrbitPoint(Planet planet)
        {
            Planet = planet;
        }
        
        public override void ImpactParticle(Particle particle)
        {
            var gX = X - particle.X;
            var gY = Y - particle.Y;
            var r = Math.Sqrt(gX * gX + gY * gY);
            
            if (r > (Diametr + 5) / 2 || r < (Diametr - 5) / 2) return;

            var r2 = Math.Max(100, gX * gX + gY * gY);

            particle.SpeedX += gX / r2;
            particle.SpeedY += gY / r2;

            particle.X += Planet.SpeedX;
            particle.Y += Planet.SpeedY;
        }

        public override void Render(Graphics graphics)
        {
            graphics.DrawEllipse(
                new Pen(Color),
                X - Diametr / 2f,
                Y - Diametr / 2f,
                Diametr,
                Diametr
            );
        }
    }
}