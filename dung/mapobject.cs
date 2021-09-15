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
    public abstract class MapObject
    {
        public virtual double X { get; protected set; }
        public virtual double Y { get; protected set; }

        public virtual int Type { get; protected set; }

        public virtual string Action { get; protected set; }
        public virtual List<Texture2D> Textures { get; protected set; }

        public virtual void Update(ContentManager contentManager, GameWorld gameWorld)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, int x, int y)
        {

        }
    }
}
