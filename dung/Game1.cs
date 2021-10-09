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
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int tmpx = 0, tmpy = 0;
        private GameWorld testworld;
        private SimpleFps fpsc = new SimpleFps();
        private SpriteFont tmpfont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.ApplyChanges();

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;

            _graphics.ApplyChanges();
            
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            _graphics.ApplyChanges();

            this.Window.IsBorderless = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
                
            tmpfont = Content.Load<SpriteFont>("mainfont");

            testworld = new GameWorld(Content/*, "info/worlds/world1"*/);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            testworld.update(Content);

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.LeftAlt)&&ks.IsKeyDown(Keys.F1))
            {
                fpsc.Update(gameTime);
            }

            if(!IsActive)
            {
                testworld.Save("info/worlds/world1");
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            testworld.draw(_spriteBatch, tmpx, tmpy);


            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.F1))
            {
                fpsc.DrawFps(_spriteBatch, tmpfont, new Vector2(0, 0), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
