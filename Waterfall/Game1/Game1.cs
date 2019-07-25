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
        public Game1()
        {
            Data.GDM = new GraphicsDeviceManager(this);
            Data.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Data.CM = Content;
            Data.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Data.GDM.PreferredBackBufferWidth = 700;
            Data.GDM.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent()
        {
            Data.SB = new SpriteBatch(GraphicsDevice);
            Data.refTexture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
            Data.refTexture.SetData<Color>(new Color[] { Color.White });
            //randomly place small colliders
            for (int i = 0; i < Pool.size; i++)
            {
                Pool.colliders[i].SetPos(
                    Functions.Rand.Next(128, 512),
                    Functions.Rand.Next(20, 700));
            }
            //place large blockers spaced along Y axis, randomly on X
            for(int i = 0; i < 7; i++)
            {
                Collider blocker = new Collider();
                blocker.Width = 16; blocker.Height = 16;
                blocker.X = Functions.Rand.Next(128, 512);
                blocker.Y = 300 + i * 50;
                Pool.blockers.Add(blocker);
            }
        }
        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Pool.size; i++)
            {
                Functions.Calc_Position(Pool.colliders[i]);
                Functions.Check_Blocker_Collisions(Pool.colliders[i]);

                //check falling off bounds of level
                if (Pool.colliders[i].currentPos.Y > 700)
                {
                    Pool.colliders[i].lastPos.Y = 20;
                    Pool.colliders[i].currentPos.Y = 20;
                    Pool.colliders[i].projectedPos.Y = 20;
                    //randomly place on X axis
                    Pool.colliders[i].SetPos(Functions.Rand.Next(128, 512), 20);
                }

                //set collider position
                Pool.colliders[i].X = Pool.colliders[i].currentPos.X;
                Pool.colliders[i].Y = Pool.colliders[i].currentPos.Y;
            }

            //move bockers
            for (int i = 0; i < Pool.blockers.Count; i++)
            {
                Pool.blockers[i].X++;
                if (Pool.blockers[i].X > 512)
                { Pool.blockers[i].X = 128; }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Data.SB.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int i = 0; i < Pool.size; i++)
            {
                Functions.Draw(Pool.colliders[i]);
            }

            for (int i = 0; i < Pool.blockers.Count; i++)
            {
                Functions.Draw(Pool.blockers[i]);
            }

            Data.SB.End();
        }
    }
}