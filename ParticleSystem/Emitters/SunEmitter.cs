﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using ParticleSystem.Particles;
using ParticleSystem.Points;

namespace ParticleSystem.Emitters
{
    public class SunEmitter : Emitter
    {
        public int PlanetsToCreate;
        public float OrbitRadius;
        public List<Asteroid> Asteroids = new();
        public float AsteroidsSpeed = 2;
        public bool IsRingNecessary = false;

        public Point RingPoint = new(0, 0);
        
        public SunPoint SunPoint = new();
        
        public override void ResetParticle(Particle particle)
        {
            particle.X = this.X;
            particle.Y = this.Y;
                
            var direction = particle is Asteroid ? Random.Next(Spreading) : (Random.Next(2) == 1 ? 0 : 180);

            particle.SpeedX = (float)(Math.Cos(direction / 180f * Math.PI) * particle.Speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180f * Math.PI) * particle.Speed);

            particle.OnOverlap += (particle1, particle2) =>
            {
                if (particle1.Speed > particle2.Speed)
                {
                    particle2.SpeedY += particle1.SpeedY * particle1.Radius / particle2.Radius;
                    particle2.SpeedX += particle1.SpeedX * particle1.Radius / particle2.Radius;

                    particle1.SpeedY -= particle2.SpeedY * particle2.Radius / particle1.Radius;
                    particle1.SpeedX -= particle2.SpeedX * particle2.Radius / particle1.Radius;
                }
                else
                {
                    particle1.SpeedY += particle2.SpeedY * particle2.Radius / particle1.Radius;
                    particle1.SpeedX += particle2.SpeedX * particle2.Radius / particle1.Radius;

                    particle2.SpeedY -= particle1.SpeedY * particle1.Radius / particle2.Radius;
                    particle2.SpeedX -= particle1.SpeedX * particle1.Radius / particle2.Radius;
                }
            };
        }

        public override void UpdateState()
        {
            for (var i = 0; i < Particles.Count; i++)
            {
                for (var j = i + 1; j < Particles.Count; j++)
                {
                    if (Particles[i] is not Asteroid && Particles[i].Overlaps(Particles[j]))
                    {
                        Particles[i].Overlap(Particles[j]);
                    }
                }

                if (Particles[i] is Sattelite sattelite)
                {
                    sattelite.IsOnPlanetOrbit = false;
                }

                foreach (var point in ImpactPoints)
                {
                    point.ImpactParticle(Particles[i]);
                }

                Particles[i].X += Particles[i].SpeedX;
                Particles[i].Y += Particles[i].SpeedY;


                if (Particles[i] is Planet planet)
                {
                    planet.ChangeOrbitsPositions();
                }
            }
        }

        //Создаются планеты и их орбиты
        public void CreatePlanets()
        {
            while (PlanetsToCreate > 0)
            {
                var randomColor = Color.FromArgb(Random.Next(256),
                    Random.Next(256), Random.Next(256));

                if (PlanetsToCreate == 1 && IsRingNecessary == true && RingPoint.X == 0)
                {
                    RingPoint = new Point(this.X, (int)(this.Y - OrbitRadius));
                    CreateRing();
                }
                else if (IsRingNecessary == true && RingPoint.X == 0 && Random.Next(10) % 4 == 2)
                {
                    RingPoint = new Point(this.X, (int) (this.Y - OrbitRadius));
                    CreateRing();
                }
                else
                {
                    var particle = new Planet
                    {
                        Color = randomColor,
                        Radius = Random.Next(RadiusMin, RadiusMax),
                        Speed = 2
                    };

                    ResetParticle(particle);
                    particle.Y -= OrbitRadius;

                    CreateSattelitesOfPlanet(particle);
                    Particles.Add(particle);
                }
                
                PlanetsToCreate -= 1;
                OrbitRadius += Random.Next(110, 140);
            }
        }

        //Создаются спутники планеты
        private void CreateSattelitesOfPlanet(Planet planet)
        {
            var sattelitesToCreate = Random.Next(2, 4);
            var orbitRadius = planet.Radius + 11;

            while (sattelitesToCreate > 0)
            {
                var randomColor = Color.FromArgb(Random.Next(256),
                    Random.Next(256), Random.Next(256));
                sattelitesToCreate -= 1;

                var satellite = new Sattelite
                {
                    Color = randomColor,
                    Radius = Random.Next(4, 6),
                    Speed = Random.Next(1, 3)
                };


                ResetParticle(satellite);

                satellite.X = planet.X;
                satellite.Y = planet.Y - orbitRadius;

                Particles.Add(satellite);
                orbitRadius += 13;
            }

            var orbit = new SatteliteOrbitPoint(planet)
            {
                Color = planet.Color,
                X = planet.X,
                Y = planet.Y,
                Diametr = orbitRadius * 2
            };

            planet.SattelitesOrbits.Add(orbit);
            ImpactPoints.Insert(0, orbit);
        }

        private async void CreateRing()
        {
            var asteroidsToCreater = 2 * Math.PI *
                                     Math.Sqrt((RingPoint.X - X) * (RingPoint.X - X) + (RingPoint.Y - Y) * (RingPoint.Y - Y)) / 15;

            while (RingPoint.X != 0 && Particles.Count < asteroidsToCreater)
            {
                var particle = new Asteroid
                {
                    Speed = AsteroidsSpeed,
                    FromColor = Color.White,
                    ToColor = Color.Gray,
                    Radius = Random.Next(5, 9)
                };
                ResetParticle(particle);
                particle.X = RingPoint.X;
                particle.Y = RingPoint.Y;
                Asteroids.Add(particle);
                Particles.Add(particle);
                await Task.Delay(250);
            }
        }
    }
}