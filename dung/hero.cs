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

        public Hero(ContentManager contentManager, double x, double y)
        {
            X = x;
            Y = y;

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
            spriteBatch.Draw(Textures[texturesPhase], new Vector2(x, y-Textures[texturesPhase].Height), Color.White);
        }

        public override void Update(ContentManager contentManager, GameWorld gameWorld)
        {
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

            if ((int)X != (int)px || (int)Y != (int)py)
            {
                if (X < 0 || X >= gameWorld.blocks.Count || Y < 0 || Y >= gameWorld.blocks[(int)X].Count || !gameWorld.blocks[(int)X][(int)Y].passable)
                {
                    X = px;
                    Y = py;
                }
            }
        }
    }
}