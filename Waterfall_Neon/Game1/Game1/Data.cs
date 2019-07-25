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
    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static RenderTarget2D RT2D;
        public static Game1 GAME;
        public static Texture2D refTexture;

        public static float friction = 0.9f;
        public static float gravity = 1.0f;
        public static float terminalAcceleration = 10.0f;
    }

    public class Collider
    {   
        public float X, Y= 0;
        public int Width = 4, Height = 4;

        public Vector2 lastPos = new Vector2();
        public Vector2 currentPos = new Vector2();
        public Vector2 projectedPos = new Vector2();
        public Vector2 acceleration = new Vector2();
        public Vector2 velocity = new Vector2();

        public float mass = 1.0f;
        public Boolean active = false;
        public Boolean movingRight = true;
        public Boolean movingDown = true;
        public Color color;
        public Boolean acceptsPush = true;

        public void SetPos(float X, float Y)
        {
            lastPos.X = X; lastPos.Y = Y;
            currentPos.X = X; currentPos.Y = Y;
            projectedPos.X = X; projectedPos.Y = Y;
        }
    }

    public class Circle
    {
        public float X, Y, radius;
        public Boolean movingRight = true;
    }

    public static class Pool
    {
        public static List<Collider> colliders;
        public static int size = 10000;

        static Pool()
        {
            colliders = new List<Collider>();
            for(int i = 0; i < size; i++)
            {
                colliders.Add(new Collider());
            }
        }
    }
}
