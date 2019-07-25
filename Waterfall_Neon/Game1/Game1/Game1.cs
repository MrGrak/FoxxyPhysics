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
        Circle circ1;
        Circle circ2;

        //should be able to move the top collider with mouse
        //this will help to direct the blocks into a specific hole
        //we can track how blocks hit that hole, and put a score on screen

        public static MouseState currentMouseState = new MouseState();

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
                /*
                Pool.colliders[i].color = new Color(
                    Functions.Rand.Next(50, 100),
                    Functions.Rand.Next(150, 200),
                    Functions.Rand.Next(250, 255), 255);
                */
                Pool.colliders[i].color = new Color(15, 115, 255);
                Pool.colliders[i].mass = Functions.Rand.Next(50, 100) * 0.01f;
            }

            /*
            Pool.colliders[0].active = true;
            Pool.colliders[0].color = new Color(0, 255, 255, 255);
            Pool.colliders[0].Width = 128;
            Pool.colliders[0].Height = 16;
            Pool.colliders[0].SetPos(Functions.Rand.Next(128, 512), 100);
            Pool.colliders[0].acceptsPush = false;
            */

            circ1 = new Circle();
            circ1.X = 380; circ1.Y = 200;
            circ1.radius = 50;

            circ2 = new Circle();
            circ2.X = 220; circ2.Y = 430;
            circ2.radius = 150;
        }
        protected override void UnloadContent() { }

        int spawnX = 128;
        int spawnTotal = 10;
        int spawnCounter = 0;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentMouseState = Mouse.GetState();

            //move top circle with mouse
            //circ2.X = currentMouseState.X - circ2.radius /2;
            //move circle left and right
            if (circ2.movingRight)
            {
                circ2.X += 0.5f;
                if (circ2.X > 370)
                { circ2.movingRight = false; }
            }
            else
            {
                circ2.X -= 0.5f;
                if (circ2.X < 270)
                { circ2.movingRight = true; }
            }


            //randomly spawn colliders at top
            spawnCounter = 0;
            for (int i = 0; i < Pool.size; i++)
            {
                if(Pool.colliders[i].active == false)
                {
                    //set collider active, randomly place
                    Pool.colliders[i].active = true;
                    //Pool.colliders[i].SetPos(spawnX, 20);
                    Pool.colliders[i].SetPos(Functions.Rand.Next(128, 512), 20);
                    //spawnX += 4;
                    //if (spawnX > 512) { spawnX = 128; }
                    //end spawn loop
                    //spawnCounter++;
                    //if (spawnCounter >= spawnTotal)
                    //{ i = Pool.size; }
                }
            }

            /*
            //offset gravity
            Pool.colliders[0].acceleration.Y -= Data.gravity;

            //move blocker left and right
            if (Pool.colliders[0].movingRight)
            {
                Pool.colliders[0].acceleration.X += 0.2f;
                if (Pool.colliders[0].currentPos.X > 512)
                { Pool.colliders[0].movingRight = false; }
            }
            else
            {
                Pool.colliders[0].currentPos.X -= 0.2f;
                if (Pool.colliders[0].currentPos.X < 64)
                { Pool.colliders[0].movingRight = true; }
            }
            */

            //move active colliders
            for (int i = 0; i < Pool.size; i++)
            {
                if (Pool.colliders[i].active)
                {
                    Functions.Calc_Position(Pool.colliders[i]);

                    Functions.Intersects(circ1, Pool.colliders[i]);
                    Functions.Intersects(circ2, Pool.colliders[i]);

                    /*
                    //alter color based on velocity
                    if(Pool.colliders[i].velocity.Y < 1.0f)
                    {
                        //Pool.colliders[i].color = new Color(75, 175, 255);

                        if (Pool.colliders[i].color.R < 255 - 10)
                        { Pool.colliders[i].color.R += 10; }
                        if (Pool.colliders[i].color.R > 254)
                        { Pool.colliders[i].color.R = 255; }

                        if (Pool.colliders[i].color.G < 255 - 45)
                        { Pool.colliders[i].color.G += 45; }
                        if (Pool.colliders[i].color.G > 254)
                        { Pool.colliders[i].color.G = 255; }
                    }
                    else
                    {
                        if (Pool.colliders[i].color.R > 60)
                        { Pool.colliders[i].color.R -= 20; }
                        if (Pool.colliders[i].color.R < 60)
                        { Pool.colliders[i].color.R = 60; }

                        if (Pool.colliders[i].color.G > 175)
                        { Pool.colliders[i].color.G -= 45; }
                        if (Pool.colliders[i].color.G < 175)
                        { Pool.colliders[i].color.G = 175; }
                    }
                    */

                    //Functions.Check_Blocker_Collisions(Pool.colliders[i]);
                    //allow all movement

                    //set last pos
                    Pool.colliders[i].lastPos.X = Pool.colliders[i].currentPos.X;
                    Pool.colliders[i].lastPos.Y = Pool.colliders[i].currentPos.Y;
                    //get curr pos
                    Pool.colliders[i].currentPos.X = Pool.colliders[i].projectedPos.X;
                    Pool.colliders[i].currentPos.Y = Pool.colliders[i].projectedPos.Y;


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
                BlendState.Additive, SamplerState.PointClamp);

            for (int i = 0; i < Pool.size; i++)
            {
                if (Pool.colliders[i].active)
                { Functions.Draw(Pool.colliders[i]); }
            }

            Data.SB.End();
        }
    }
}