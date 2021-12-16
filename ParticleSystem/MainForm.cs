﻿using ParticleSystem.Emitters;
using ParticleSystem.Points;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ParticleSystem
{
    public partial class MainForm : Form
    {
        private readonly List<Emitter> _emitters = new();
        private readonly Emitter _emitter;
        private readonly Random _random = new();
        private bool _isOrbitsActive = true;
        public MainForm()
        {
            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            var particlesToCreate = _random.Next(2, 8);
            var orbitRadius = _random.Next(150, 170);
            
            _emitter = new SunEmitter()
            {
                Direction = 0,
                Spreading = 1,
                RadiusMin = 5,
                RadiusMax = 10,
                GravitationX = 0,
                GravitationY = 0,
                SpeedMin = 1,
                SpeedMax = 1,
                ParticlesToCreate = particlesToCreate,
                OrbitRadius = orbitRadius,
                X = picDisplay.Width / 2f,
                Y = picDisplay.Height / 2f
            };

            _emitters.Add(_emitter);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var graphics = Graphics.FromImage(picDisplay.Image);
            graphics.Clear(Color.Black);

            foreach (var emitter in _emitters)
            {
                if (emitter is SunEmitter sunEmitter)
                {
                    foreach (var planetEmitter in sunEmitter.Emmiters)
                    {
                        planetEmitter.UpdateState();
                        planetEmitter.RenderParticles(graphics);
                        if (_isOrbitsActive == true)
                        {
                            planetEmitter.RenderImpactPoints(graphics);
                        }
                    }

                    sunEmitter.RingEmitter?.UpdateState();
                    sunEmitter.RingEmitter?.RenderParticles(graphics);
                    if (_isOrbitsActive == true)
                    {
                        sunEmitter.RingEmitter?.RenderImpactPoints(graphics);
                    }
                }

                emitter.UpdateState();
                emitter.RenderParticles(graphics);
                if (_isOrbitsActive == true)
                {
                    emitter.RenderImpactPoints(graphics);
                }

                graphics.FillEllipse(
                    new SolidBrush(Color.Orange), 
                    picDisplay.Width / 2 - 75,
                    picDisplay.Height / 2 - 75,
                    150, 
                    150
                    );
            }

            picDisplay.Invalidate();
        }

        private void picDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            _emitter.MousePositionX = e.X;
            _emitter.MousePositionY = e.Y;
        }

        private void changeOrbitsVision_Click(object sender, EventArgs e)
        {
            _isOrbitsActive = !_isOrbitsActive;
        }
    }
}
