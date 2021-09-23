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
    public class Gun : Item
    {
        public override double X { get; protected set; }
        public override double Y { get; protected set; }
        public override int Type { get; protected set; }
        public List<Bullet> bulletsShooting { get; protected set; }
        public int FireSpeed { get; protected set; }
        public int TimeSinceLastShoot { get; protected set; }

        /// <summary>
        /// With file reading
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Gun(ContentManager contentManager, int type, double x, double y)
        {
            TimeSinceLastShoot = 0;

            Type = type;

            X = x;
            Y = y;

            bulletsShooting = new List<Bullet>();

            using(StreamReader sr = new StreamReader("info/global/items/guns/"+Type.ToString()+"/m.info"))
            {
                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                FireSpeed = Int32.Parse(tmplist[0]);

                int tmpn = Int32.Parse(tmplist[1]);

                for (int i = 2; i < tmpn+2; i++)
                {
                    bulletsShooting.Add(new Bullet(contentManager, Int32.Parse(tmplist[i]), 0, 0, 0));
                }
            }

            base.updateTexture(contentManager, true);
        }

        /// <summary>
        /// With sample
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sampleGun"></param>
        public Gun(ContentManager contentManager, int type, double x, double y, Gun sampleGun)
        {
            TimeSinceLastShoot = 0;

            Type = type;

            X = x;
            Y = y;

            FireSpeed = sampleGun.FireSpeed;

            bulletsShooting = sampleGun.bulletsShooting;

            base.updateTexture(contentManager, true);
        }

        public override void Update(ContentManager contentManager, GameWorld gameWorld, int myIndex)
        {
            TimeSinceLastShoot++;

            base.Update(contentManager, gameWorld, myIndex);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(spriteBatch, x, y);
        }

        public void ShootInDirection(GameWorld gameWorld, ContentManager contentManager, double x, double y, double direction, double radius)
        {
            if (TimeSinceLastShoot >= FireSpeed)
            {
                TimeSinceLastShoot = 0;

                for (int i = 0; i < bulletsShooting.Count; i++)
                {
                    double tmpbx = x + Math.Cos(direction) * (radius + bulletsShooting[i].Radius);
                    double tmpby = y + Math.Sin(direction) * (radius + bulletsShooting[i].Radius);

                    gameWorld.AddObject(new Bullet(contentManager, 0, tmpbx, tmpby, direction, bulletsShooting[i]));
                }
            }
        }

        public override string GetTypeAsString()
        {
            return "Gun";
        }
    }
}