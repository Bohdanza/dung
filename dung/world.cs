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
        const int blockDrawY = 42, BlockWidth = 64;
        private Texture2D darknessEffect;
        private List<MapObject> mapObjects;
        private MapObject referenceToHero;

        public GameWorld(ContentManager contentManager)
        {
            darknessEffect = contentManager.Load<Texture2D>("darkness");

            mapObjects = new List<MapObject>();

            List<Block> sampleBlocks = new List<Block>();

            for (int i = 0; i < 3; i++)
            {
                sampleBlocks.Add(new Block(i, 0, 0, contentManager));
            }

            DungeonSynthesizer ds = new DungeonSynthesizer(contentManager, 960, 960);

            ds.RandomSeeds(200, 250, 15);
            ds.GenerateCorridors(250, 1000);
            ds.ReplaceRooms(11, 11);
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

            for(int i=0; i<ds.rooms.Count; i++)
            {
                mapObjects.Add(new Robot(0, contentManager, ds.rooms[i].Item1+6, ds.rooms[i].Item2));
            }

            mapObjects.Add(new Hero(contentManager, ds.rooms[0].Item1, ds.rooms[0].Item2));

            referenceToHero = mapObjects[mapObjects.Count - 1];
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

            for(int i=0; i<mapObjects.Count; i++)
            {
                mapObjects[i].Update(contentManager, this);
            }

            /*for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks[i].Count; j++)
                {
                    blocks[i][j].update(contentManager);
                }
            }*/
        }

        public void draw(SpriteBatch spriteBatch, int x, int y)
        {
            int tmpx = -(int)(referenceToHero.X * BlockWidth);
            int tmpy = -(int)(referenceToHero.Y * blockDrawY);

            int drawx = tmpx + x + 960;
            int drawy = tmpy + y + 540;

            int startx = drawx / blocks[0][0].textures[0].Width, endx = startx * -1 + 1920 / blocks[0][0].textures[0].Width, starty = drawy / blockDrawY, endy = starty * -1 + 1080 / blockDrawY;

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
                            blocks[i][j].draw(spriteBatch, drawx + i * blocks[i][j].textures[0].Width + BlockWidth / 2, drawy + j * blockDrawY - blockDrawY);
                        }
                    }
                }
            }

            //effects
            //spriteBatch.Draw(darknessEffect, new Vector2(0, 0), Color.White);
        }
    }
}