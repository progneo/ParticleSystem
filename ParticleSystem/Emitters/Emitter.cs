﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ParticleSystem.Particles;
using ParticleSystem.Points;

namespace ParticleSystem.Emitters
{
    public class Emitter
    {
        public List<Particle> Particles = new();
        public List<ImpactPoint> ImpactPoints = new();

        public int MousePositionX;
        public int MousePositionY;

        public float GravitationX = 0;
        public float GravitationY = 1;

        public float X;
        public float Y;
        public int Direction = 0;
        public int Spreading = 360;
        public int SpeedMin = 1;
        public int SpeedMax = 10;
        public int RadiusMin = 2;
        public int RadiusMax = 10;
        public int LifeMin = 20;
        public int LifeMax = 100;
        public int ParticlesPerTick = 1;

        public Color ColorFrom = Color.White;
        public Color ColorTo = Color.FromArgb(0, Color.Black);

        public virtual void ResetParticle(Particle particle)
        {
            particle.Life = Particle.Rand.Next(LifeMin, LifeMax);

            particle.X = X;
            particle.Y = Y;

            var direction = Direction + (double)Particle.Rand.Next(Spreading) - Spreading / 2f;

            if (particle is ParticleColorful colorful)
            {
                colorful.FromColor = ColorFrom;
                colorful.ToColor = ColorTo;
            }

            var speed = Particle.Rand.Next(SpeedMin, SpeedMax);

            particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180 * Math.PI) * speed);

            particle.Radius = Particle.Rand.Next(RadiusMin, RadiusMax);
        }

        public virtual Particle CreateParticle()
        {
            var particle = new ParticleColorful
            {
                FromColor = ColorFrom,
                ToColor = ColorTo
            };

            return particle;
        }

        public virtual void UpdateState()
        {
            var particlesToCreate = ParticlesPerTick;

            foreach (var particle in Particles)
            {
                if (particle.Life <= 0)
                {
                    if (particlesToCreate <= 0) continue;
                    particlesToCreate -= 1;
                    ResetParticle(particle);
                }
                else
                {
                    particle.Life -= 1;
                    foreach (var point in ImpactPoints)
                    {
                        point.ImpactParticle(particle);
                    }

                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;

                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;
                }
            }

            while (particlesToCreate >= 1)
            {
                particlesToCreate -= 1;
                var particle = CreateParticle();
                ResetParticle(particle);
                Particles.Add(particle);
            }
        }

        public void RenderParticles(Graphics graphics)
        {
            foreach (var particle in Particles)
            {
                particle.Draw(graphics);
            }
        }

        public void RenderImpactPoints(Graphics graphics)
        {
            foreach (var point in ImpactPoints)
            {
                point.Render(graphics);
            }
        }
    }
}