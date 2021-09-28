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
    public class GameWorld
    {
        public List<List<Block>> blocks;
        const int blockDrawY = 64, BlockWidth = 64;
        private Texture2D darknessEffect;
        private List<MapObject> mapObjects;
        public MapObject referenceToHero { get; private set; }
        public List<Block> sampleBlocks { get; private set; } = new List<Block>();
        public List<Ghost> sampleGhosts { get; private set; } = new List<Ghost>();
        public List<Gun> sampleGuns { get; private set; } = new List<Gun>();

        public GameWorld(ContentManager contentManager)
        {
            darknessEffect = contentManager.Load<Texture2D>("darkness");

            mapObjects = new List<MapObject>();

            for (int i = 0; i < 3; i++)
            {
                sampleBlocks.Add(new Block(i, 0, 0, contentManager));
            }
            
            for (int i = 0; i < 1; i++)
            {
                sampleGhosts.Add(new Ghost(contentManager, i, 0, 0, 0, 0));
            }

            for (int i = 0; i < 4; i++)
            {
                sampleGuns.Add(new Gun(contentManager, i, 0, 0));
            }

            //generating main dungeon
            DungeonSynthesizer ds = new DungeonSynthesizer(contentManager, 480, 480);

            ds.RandomSeeds(175, 256, 30);
            ds.GenerateCorridors(250, 1000);
            ds.ReplaceRooms(17, 17);
            ds.PlaceWalls();

            List<List<int>> tmplist = ds.GetList();

            blocks = new List<List<Block>>();

            for (int i = 0; i < tmplist.Count; i++)
            {
                List<Block> tmpblock = new List<Block>();

                for (int j = 0; j < tmplist[i].Count; j++)
                {
                    tmpblock.Add(new Block(tmplist[i][j], i, j, contentManager, sampleBlocks[tmplist[i][j]]));
                }

                blocks.Add(tmpblock);
            }

            //generating mobs, loot etc.
            List<int> specialRooms = new List<int>();

            var rnd = new Random();

            while(specialRooms.Count<2)
            {
                int tmpi = rnd.Next(0, ds.rooms.Count);

                if(!specialRooms.Contains(tmpi))
                {
                    specialRooms.Add(tmpi);
                }
            }

            AddObject(new Hero(contentManager, ds.rooms[specialRooms[0]].Item1, ds.rooms[specialRooms[0]].Item2));

            referenceToHero = mapObjects[mapObjects.Count - 1];

            
            List<List<int>> fightingRooms = new List<List<int>>();

            for (int i = 0; i < ds.rooms.Count; i++)
            {
                if(!specialRooms.Contains(i))
                {
                    insertRoomObtaclesAt(contentManager, ds.rooms[i].Item1 - 8, ds.rooms[i].Item2 - 8, 17, 17, "", 7, 4, 10);

                    insertMobs(contentManager, ds.rooms[i].Item1, ds.rooms[i].Item2, 17, 17, rnd.Next(12, 20), 0);
                }
            }
        }

        //TODO:
        /// <summary>
        /// DONT USE IT!!!!!!!
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="path"></param>
        public GameWorld(ContentManager contentManager, string path)
        {

        }

        public void update(ContentManager contentManager)
        {
            mapObjects.Sort((a, b) => a.Y.CompareTo(b.Y));

            int l = 1;

            for (int i = 0; i < mapObjects.Count; i += l)
            {
                l = 1;

                mapObjects[i].Update(contentManager, this, i);
                
                if(!mapObjects[i].alive)
                {
                    l = 0;
                    mapObjects.RemoveAt(i);
                }
            }
        }

        public void draw(SpriteBatch spriteBatch, int x, int y)
        {
            int tmpx = -(int)(referenceToHero.X * BlockWidth);
            int tmpy = -(int)(referenceToHero.Y * blockDrawY);

            int drawx = tmpx + x + 960;
            int drawy = tmpy + y + 540;

            int startx = drawx / blocks[0][0].textures[0].Width, endx = startx * -1 + 1920 / blocks[0][0].textures[0].Width, starty = drawy / blockDrawY, endy = starty * -1 + 1080 / blockDrawY + 1;

            startx *= -1;
            starty *= -1;

            startx = Math.Max(startx, 0);
            starty = Math.Max(starty, 0);

            endx = Math.Min(endx, blocks.Count);
            endy = Math.Min(endy, blocks[0].Count);

            int mapObjectsJ = 0, l = 1;

            for (int j = starty; j < endy; j += l)
            {
                l = 1;

                if (mapObjectsJ<mapObjects.Count && mapObjects[mapObjectsJ].Y < j)
                {
                    l = 0;

                    mapObjects[mapObjectsJ].Draw(spriteBatch, drawx + (int)(mapObjects[mapObjectsJ].X * BlockWidth), drawy + (int)(mapObjects[mapObjectsJ].Y * blockDrawY));

                    mapObjectsJ++;
                }
                else
                {
                    for (int i = startx; i < endx; i++)
                    {
                        if (blocks[i][j].type != 0)
                        {
                            blocks[i][j].draw(spriteBatch, drawx + i * BlockWidth, drawy + j * blockDrawY - blocks[i][j].textures[0].Height + blockDrawY);
                        }
                    }
                }
            }

            //effects
            spriteBatch.Draw(darknessEffect, new Vector2(0, 0), Color.White);

            //hero hp, inventory & other
            ((Hero)referenceToHero).DrawInterface(spriteBatch);
        }

        public double GetDist(double x, double y, double x1, double y1)
        {
            double a = Math.Abs(x - x1);
            double b = Math.Abs(y - y1);

            return Math.Sqrt(a * a + b * b);
        }

        public MapObject GetClosestObject(double x, double y, int indexToIgnore)
        {
            int mi = 0;
            double md = -1;

            for (int i = 0; i < mapObjects.Count; i++)
            {
                double tmpd = this.GetDist(x, y, mapObjects[i].X, mapObjects[i].Y);

                if (tmpd < md)
                {
                    mi = i;

                    md = tmpd;
                }
            }

            if(md>-1)
            {
                return mapObjects[mi];
            }

            return null;
        }

        /// <summary>
        /// Get closest object of given type
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="indexToIgnore"></param>
        /// <param name="typeAsString"></param>
        /// <returns></returns>
        public MapObject GetClosestObject(double x, double y, int indexToIgnore, string typeAsString)
        {
            int mi = 0;
            double md = double.MaxValue;

            for (int i = 0; i < mapObjects.Count; i++)
            {
                if (mapObjects[i].GetTypeAsString() == typeAsString)
                {
                    double tmpd = this.GetDist(x, y, mapObjects[i].X, mapObjects[i].Y);

                    if (tmpd < md)
                    {
                        mi = i;

                        md = tmpd;
                    }
                }
            }

            if (md > -1)
            {
                return mapObjects[mi];
            }

            return null;
        }
        
        public void AddObject(MapObject mapObject)
        {
            mapObjects.Add(mapObject);

            mapObjects.Sort((a, b) => a.Y.CompareTo(b.Y));
        }

        private void insertRoomObtaclesAt(ContentManager contentManager, int x, int y, int xsize, int ysize, string roomType, int maxSize, int minObtacleNumber, int maxObtacleNumber)
        {
            var rnd = new Random();

            int obtaclesNumber = rnd.Next(minObtacleNumber, maxObtacleNumber);

            for (int k = 0; k < obtaclesNumber; k++)
            {
                int x1 = rnd.Next(x + 1, x + xsize - 1);
                int y1 = rnd.Next(y + 1, y + ysize - 1);
                int x2 = rnd.Next(x1 + 1, x1 + 1 + maxSize);
                int y2 = rnd.Next(y1 + 1, y1 + 1 + maxSize);

                for (int i = x1; i < x2; i++)
                {
                    for (int j = y1; j < y2; j++)
                    {
                        if (i >= 0 && j >= 0 && i < blocks.Count && j < blocks[i].Count && i > x && j > y && i < x + xsize - 2 && j < y + ysize - 2)
                        {
                            blocks[i][j] = new Block(2, i, j, contentManager, sampleBlocks[2]);
                        }
                    }
                }
            }
        }
        
        private void insertMobs(ContentManager contentManager, int x, int y, int xsize, int ysize, int number, int type)
        {
            int c = 0;

            var rnd = new Random();

            while (c < number)
            {
                double tmpx = x + rnd.NextDouble() * xsize;
                double tmpy = y + rnd.NextDouble() * ysize;

                if (blocks[(int)tmpx][(int)tmpy].passable)
                {
                    AddObject(new Ghost(contentManager, type, tmpx, tmpy, tmpx, tmpy, sampleGhosts[type]));

                    c++;
                }
            }
        }
    }
}