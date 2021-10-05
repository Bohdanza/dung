using Microsoft.VisualBasic;
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
    public class Hero : MapObject
    {
        public override double X { get; protected set; }
        public override double Y { get; protected set; }
        public override string Action { get; protected set; }
        public override int Type { get; protected set; }
        public override List<Texture2D> Textures { get; protected set; }
        const double speed = 0.1;
        private int texturesPhase;
        public override double Radius { get; protected set; }
        public override int HP { get; protected set; }

        private Texture2D hpHeart;
        private SpriteFont hpFont;
        public Gun GunInHand;

        private int timeSinceLastAction = 0;

        public Hero(ContentManager contentManager, double x, double y)
        {
            X = x;
            Y = y;

            Radius = 0.5;

            HP = 3;

            hpHeart = contentManager.Load<Texture2D>("hpheart");

            hpFont = contentManager.Load<SpriteFont>("hpfont");

            GunInHand = new Gun(contentManager, 4, 0, 0);

            UpdateTextures(contentManager, true);
        }

        private void UpdateTextures(ContentManager contentManager, bool reload)
        {
            Textures = new List<Texture2D>();

            Textures.Add(contentManager.Load<Texture2D>("tmphero"));

            texturesPhase = 0;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            //dont touch ANYTHING
            //Can U hear me?
            //U don't want to see what would happen, trust me
            spriteBatch.Draw(Textures[texturesPhase], new Vector2(x - Textures[texturesPhase].Width / 2, y - Textures[texturesPhase].Height), Color.White);

            var mouseState = Mouse.GetState();

            double tmpdir = Math.Atan2(540 - mouseState.Y, 960 - mouseState.X);

            tmpdir += 3f * (float)Math.PI;

            tmpdir %= (float)(Math.PI * 2);

            GunInHand.Draw(spriteBatch, x, y - (int)(Textures[texturesPhase].Height * 0.1), tmpdir);
        }

        public void DrawInterface(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < HP; i++)
            {
                spriteBatch.Draw(hpHeart, new Vector2((int)(15+i*hpHeart.Width*1.1), 35), Color.White);
            }

            spriteBatch.DrawString(hpFont, HP.ToString(), new Vector2(15, (int)(35+hpHeart.Height*1.3)), Color.White);
        }

        public override void Update(ContentManager contentManager, GameWorld gameWorld, int myIndex)
        {
            timeSinceLastAction++;

            double px = X;
            double py = Y;

            var keyboardState = Keyboard.GetState();

            if(keyboardState.IsKeyDown(Keys.W))
            {
                Y -= speed; 
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                X -= speed;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Y += speed;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                X += speed;
            }

            if (timeSinceLastAction >= 100 && keyboardState.IsKeyDown(Keys.Space))
            {
                timeSinceLastAction = 0;

                MapObject closestGun = gameWorld.GetClosestObject(X, Y, myIndex, "Gun");

                if (closestGun != null)
                {
                    if (gameWorld.GetDist(X, Y, closestGun.X, closestGun.Y) <= this.Radius + closestGun.Radius)
                    {
                        GunInHand.ChangeCoords(X, Y);
                        gameWorld.AddObject(GunInHand);

                        GunInHand = (Gun)closestGun;

                        gameWorld.RemoveObject(closestGun);
                    }
                }
            }
            
            GunInHand.Update(contentManager, gameWorld, myIndex);

            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (GunInHand.TimeSinceLastShoot >= GunInHand.FireSpeed)
                {
                    double tmpdir = Math.Atan2(540 - mouseState.Y, 960 - mouseState.X);

                    tmpdir += (float)Math.PI;

                    tmpdir %= (float)(Math.PI * 2);

                    GunInHand.ShootInDirection(gameWorld, contentManager, X, Y, tmpdir, Radius);
                }
            }

            if ((int)X != (int)px || (int)Y != (int)py)
            {
                if (X < 0 || X >= gameWorld.blocks.Count || Y < 0 || Y >= gameWorld.blocks[(int)X].Count || !gameWorld.blocks[(int)X][(int)Y].passable)
                {
                    X = px;
                    Y = py;
                }
            }
        }

        public override void Attack(int strenght)
        {
            HP -= strenght;

            if (HP <= 0)
            {
                HP = 0;

                alive = false;
            }
        }

        public override string GetTypeAsString()
        {
            return "Hero";
        }

        public override List<string> SaveList()
        {
            List<string> tmplist = base.SaveList();

            tmplist.Add(HP.ToString());

            List<string> tmpgunlist = GunInHand.SaveList();
                 
            foreach(var currentString in tmpgunlist)
            {
                tmplist.Add(currentString);
            }
            
            return tmplist;
        }
    }
}