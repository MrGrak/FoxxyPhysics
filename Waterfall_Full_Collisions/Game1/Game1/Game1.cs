using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace Game1
{
    public class Game1 : Game
    {
        //testing basic physics using pools

        public Game1()
        {
            Data.GDM = new GraphicsDeviceManager(this);
            Data.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Data.CM = Content;
            Data.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Data.GDM.PreferredBackBufferWidth = 800;
            Data.GDM.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent()
        {
            Data.SB = new SpriteBatch(GraphicsDevice);
            Data.refTexture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
            Data.refTexture.SetData<Color>(new Color[] { Color.White });

            for (int i = 0; i < Pool.size; i++)
            {
                Pool.colliders[i].color = new Color(
                    Functions.Rand.Next(250, 255),
                    Functions.Rand.Next(100, 200),
                    Functions.Rand.Next(100, 200), 255);
            }

            Pool.colliders[0].active = true;
            Pool.colliders[0].color = new Color(0, 255, 255, 255);
            Pool.colliders[0].Width = 32;
            Pool.colliders[0].Height = 16;
            Pool.colliders[0].SetPos(Functions.Rand.Next(128, 512), 150);

            Pool.colliders[1].active = true;
            Pool.colliders[1].color = new Color(0, 255, 255, 255);
            Pool.colliders[1].Width = 64;
            Pool.colliders[1].Height = 16;
            Pool.colliders[1].SetPos(Functions.Rand.Next(128, 512), 275);

            Pool.colliders[2].active = true;
            Pool.colliders[2].color = new Color(0, 255, 255, 255);
            Pool.colliders[2].Width = 128;
            Pool.colliders[2].Height = 16;
            Pool.colliders[2].SetPos(Functions.Rand.Next(128, 512), 400);

        }
        protected override void UnloadContent() { }

        int spawnX = 128;
        int spawnTotal = 10;
        int spawnCounter = 0;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //randomly spawn colliders at top
            spawnCounter = 0;
            for (int i = 0; i < Pool.size; i++)
            {
                if(Pool.colliders[i].active == false)
                {
                    //set collider active, randomly place
                    Pool.colliders[i].active = true;
                    Pool.colliders[i].SetPos(spawnX, 20);
                    spawnX += 4;
                    if (spawnX > 512) { spawnX = 128; }
                    //end spawn loop
                    spawnCounter++;
                    if (spawnCounter >= spawnTotal)
                    { i = Pool.size; }
                }
            }

            for(int i = 0; i < 3; i++)
            {
                //offset gravity
                Pool.colliders[i].acceleration.Y -= Data.gravity;

                //move blocker left and right
                if (Pool.colliders[i].movingRight)
                {
                    Pool.colliders[i].acceleration.X += 0.2f;
                    if (Pool.colliders[i].currentPos.X > 512)
                    { Pool.colliders[i].movingRight = false; }
                }
                else
                {
                    Pool.colliders[i].currentPos.X -= 0.2f;
                    if (Pool.colliders[i].currentPos.X < 64)
                    { Pool.colliders[i].movingRight = true; }
                }
            }
            



            //move active colliders
            for (int i = 0; i < Pool.size; i++)
            {
                if (Pool.colliders[i].active)
                {
                    Functions.Calc_Position(Pool.colliders[i]);
                    Functions.Check_Blocker_Collisions(Pool.colliders[i]);

                    //check falling off bounds of level
                    if (Pool.colliders[i].projectedPos.Y > 500)
                    { Pool.colliders[i].active = false; }

                    //set collider position
                    Pool.colliders[i].X = Pool.colliders[i].currentPos.X;
                    Pool.colliders[i].Y = Pool.colliders[i].currentPos.Y;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Data.SB.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int i = 0; i < Pool.size; i++)
            {
                if (Pool.colliders[i].active)
                { Functions.Draw(Pool.colliders[i]); }
            }

            Data.SB.End();
        }
    }
}