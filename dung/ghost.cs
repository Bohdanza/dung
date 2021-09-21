﻿using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace dung
{
    public class Ghost:MapObject
    {
        public override double X { get; protected set; }
        public override double Y { get; protected set; }
        public override List<Texture2D> Textures { get; protected set; }
        public override int Type { get; protected set; }
        public override string Action { get; protected set; }
        private string direction;
        private double degDirection, speed;

        private int texturePhase, timeSinceLastAttack, attackSpeed;
        public override double Radius { get; protected set; }
        public override int HP { get; protected set; }
        protected double viewRadius { get; set; }

        public Ghost(ContentManager contentManager, int type, double x, double y)
        {
            Action = "id";

            degDirection = 0;

            speed = 0.1f;

            direction = "w";

            X = x;
            Y = y;

            HP = 1;

            Radius = 0.5;

            viewRadius = 7;

            timeSinceLastAttack = 0;
            attackSpeed = 500;

            updateTexture(contentManager, true);
        }

        public Ghost(ContentManager contentManager, int type, double x, double y, Ghost sampleGhost)
        {
            Action = "id";

            degDirection = 0;

            speed = 0.1f;

            direction = "w";

            X = x;
            Y = y;

            HP = 1;

            Radius = 0.5;

            viewRadius = 7;

            timeSinceLastAttack = 0;
            attackSpeed = 500;

            updateTexture(contentManager, true);
        }

        private void updateTexture(ContentManager contentManager, bool reload)
        {
            if (reload)
            {
                Textures = new List<Texture2D>();

                texturePhase = 0;

                Textures.Add(contentManager.Load<Texture2D>("tmpghost"));
            }
        }

        public override void Update(ContentManager contentManager, GameWorld gameWorld)
        {
            timeSinceLastAttack++;

            double px = X;
            double py = Y;

            var rnd = new Random();

            if (rnd.Next(0, 100) <= 5)
            {
                int tmpres = rnd.Next(0, 2);

                if (tmpres == 0)
                {
                    tmpres = -1;
                }

                degDirection += rnd.NextDouble()*tmpres;
            }

            if (degDirection >= 2 * Math.PI)
            {
                degDirection = degDirection - 2 * Math.PI;
            }

            if (degDirection < 0)
            {
                degDirection = degDirection + 2 * Math.PI;
            }

            if (gameWorld.GetDist(X, Y, gameWorld.referenceToHero.X, gameWorld.referenceToHero.Y) <= viewRadius)
            {
                double x1 = X - gameWorld.referenceToHero.X, y1 = Y - gameWorld.referenceToHero.Y;

                degDirection = Math.Atan2(y1, x1);

                degDirection += (float)Math.PI;

                degDirection %= (float)(Math.PI * 2);
                /*if (X < gameWorld.referenceToHero.X)
                {
                    degDirection += Math.PI;
                }*/
            }

            X += Math.Cos(degDirection) * speed;
            Y += Math.Sin(degDirection) * speed;

            if (X < 0)
            {
                X = 0;
            }

            if (X >= gameWorld.blocks.Count)
            {
                X = gameWorld.blocks.Count - 1;
            }

            if (Y < 0)
            {
                Y = 0;
            }

            if (Y >= gameWorld.blocks[(int)Math.Floor(X)].Count)
            {
                Y = gameWorld.blocks[(int)Math.Floor(X)].Count - 1;
            }

            if (!gameWorld.blocks[(int)Math.Floor(X)][(int)Math.Floor(Y)].passable)
            {
                X = px;
                Y = py;

                //constant shit
                degDirection += 1.57079633;
            }

            double tmpdist = gameWorld.GetDist(X, Y, gameWorld.referenceToHero.X, gameWorld.referenceToHero.Y);

            if (timeSinceLastAttack >= attackSpeed && tmpdist <= Radius + gameWorld.referenceToHero.Radius)
            {
                timeSinceLastAttack = 0;

                gameWorld.referenceToHero.Attack(1);
            }

            updateTexture(contentManager, false);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(Textures[texturePhase], new Vector2(x - Textures[texturePhase].Width / 2, y - Textures[texturePhase].Height), Color.White);
        }

        public override void Attack(int strenght)
        {
            HP -= strenght;

            if (HP < 0)
            {
                HP = 0;
            }
        }
    }
}
