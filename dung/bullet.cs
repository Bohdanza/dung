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
    public class Bullet:MapObject
    {
        public override double X { get; protected set; }
        public override double Y { get; protected set; }

        public override double Radius { get; protected set; }
        public override int Type { get; protected set; }
        public override List<Texture2D> Textures { get; protected set; }
        public int damage { get; protected set; }

        /// <summary>
        /// With file reading
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Bullet(ContentManager contentManager, int type, double x, double y)
        {
            X = x;
            Y = y;

            Type = type;

          //  using(StreamReader)
        }
    }
}