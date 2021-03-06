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

        /// <summary>
        /// used to avoid hp texture type each time. Try to cut it out, then you'll understand
        /// </summary>
        private List<int> HpTextures;

        private Texture2D reloadTexture;

        private List<Texture2D> hpHeartTextures;
        private SpriteFont hpFont;
        public Gun GunInHand;

        private int timeSinceLastAction = 0;

        public Hero(ContentManager contentManager, double x, double y)
        {
            X = x;
            Y = y;

            Radius = 0.5;

            HP = 3;

            hpHeartTextures = new List<Texture2D>();
            
            for (int i = 0; i < 5; i++)
            {
                hpHeartTextures.Add(contentManager.Load<Texture2D>(i.ToString() + "hpheart"));
            }

            hpFont = contentManager.Load<SpriteFont>("hpfont");

            reloadTexture = contentManager.Load<Texture2D>("reloadfull");

            GunInHand = new Gun(contentManager, 0, 0, 0);

            HpTextures = new List<int>();

            stabilizeHpList();

            UpdateTextures(contentManager, true);
        }

        public Hero(ContentManager contentManager, List<string> strList, int beginning, List<Gun> sampleGuns)
        {
            Type = Int32.Parse(strList[beginning]);

            X = double.Parse(strList[beginning + 1]);
            Y = double.Parse(strList[beginning + 2]);

            HP = Int32.Parse(strList[beginning + 3]);

            GunInHand = new Gun(contentManager, strList, beginning + 4, sampleGuns);
            
            Radius = 0.5;

            hpHeartTextures = new List<Texture2D>();

            for (int i = 0; i < 5; i++)
            {
                hpHeartTextures.Add(contentManager.Load<Texture2D>("hpheart" + i.ToString()));
            }

            hpFont = contentManager.Load<SpriteFont>("hpfont");

            HpTextures = new List<int>();

            stabilizeHpList();

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

            double tmpdir = Math.Atan2(540 - (int)(Textures[texturesPhase].Height * 0.1) - mouseState.Y, 960 - mouseState.X);

            tmpdir += 3f * (float)Math.PI;

            tmpdir %= (float)(Math.PI * 2);

            GunInHand.Draw(spriteBatch, x, y - (int)(Textures[texturesPhase].Height * 0.05), tmpdir);

            if (GunInHand.TimeSinceLastShoot < GunInHand.FireSpeed)
            {
                spriteBatch.Draw(reloadTexture, new Vector2(x - reloadTexture.Width / 2, (int)(y - Textures[texturesPhase].Height - reloadTexture.Height * 1.2)), Color.White);

                spriteBatch.Draw(reloadTexture,
                new Vector2(x - reloadTexture.Width / 2, (int)(y - Textures[texturesPhase].Height - reloadTexture.Height * 1.2)),
                new Rectangle(0, 0, (int)(reloadTexture.Width * (double)GunInHand.TimeSinceLastShoot / GunInHand.FireSpeed), reloadTexture.Height), Color.White);
            }   
        }

        public void DrawInterface(SpriteBatch spriteBatch)
        {
            int cx = 15;

            for (int i = 0; i < HpTextures.Count; i++)
            {
                spriteBatch.Draw(hpHeartTextures[HpTextures[i]], new Vector2(cx, 35), Color.White);

                cx += (int)(hpHeartTextures[HpTextures[i]].Width * 1.1);
            }

            spriteBatch.DrawString(hpFont, HP.ToString(), new Vector2(15, (int)(35 + hpHeartTextures[0].Height * 1.3)), Color.White);
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
                    double tmpdir = Math.Atan2(540 - (int)(Textures[texturesPhase].Height * 0.1) - mouseState.Y, 960 - mouseState.X);

                    tmpdir += (float)Math.PI;

                    tmpdir %= (float)(Math.PI * 2);

                    GunInHand.ShootInDirection(gameWorld, contentManager, X, Y - ((double)Textures[texturesPhase].Height * 0.1 / GameWorld.blockDrawY), tmpdir, Radius);
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

            stabilizeHpList();
        }

        private void stabilizeHpList()
        {
            while (HpTextures.Count > HP)
            {
                HpTextures.RemoveAt(HpTextures.Count - 1);
            }

            var rnd = new Random();

            while (HpTextures.Count < HP)
            {
                HpTextures.Add(rnd.Next(0, hpHeartTextures.Count));
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