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
    public static class Functions
    {
        public static Random Rand = new Random();

        public static void Draw(Collider HB)
        {   //draw hitbox rec
            Data.SB.Draw(Data.refTexture,
                new Rectangle(
                    (int)HB.X, (int)HB.Y,
                    HB.Width, HB.Height),
                HB.color * 1.0f);
        }

        public static void Calc_Position(Collider Comp)
        {
            //apply gravity + velocity
            Comp.acceleration.Y += Comp.mass * Data.gravity;
            Comp.acceleration.X += 0 / Comp.mass;
            Comp.velocity.X = Comp.currentPos.X - Comp.lastPos.X;
            Comp.velocity.Y = Comp.currentPos.Y - Comp.lastPos.Y;
            //dampen acceleration + velocity 
            Comp.acceleration.X *= Data.friction;
            Comp.acceleration.Y *= Data.friction;
            Comp.velocity.X *= Data.friction;
            Comp.velocity.Y *= Data.friction;
            //apply terminal acceleration
            if (Comp.acceleration.X > Data.terminalAcceleration)
            { Comp.acceleration.X = Data.terminalAcceleration; }
            if (Comp.acceleration.Y > Data.terminalAcceleration)
            { Comp.acceleration.Y = Data.terminalAcceleration; }
            //trim minimum velocity
            if (Math.Abs(Comp.velocity.X) < 0.09f) { Comp.velocity.X = 0.0f; }
            if (Math.Abs(Comp.velocity.Y) < 0.09f) { Comp.velocity.Y = 0.0f; }
            //calculate projected position, reset acceleration
            Comp.projectedPos.X = Comp.currentPos.X + Comp.velocity.X + Comp.acceleration.X;
            Comp.projectedPos.Y = Comp.currentPos.Y + Comp.velocity.Y + Comp.acceleration.Y;
            Comp.acceleration.X = 0; Comp.acceleration.Y = 0;
        }

        public static bool Intersects(Collider HB1, Collider HB2)
        {
            return HB2.X < (HB1.X + HB1.Width) &&
                   HB1.X < (HB2.X + HB2.Width) &&
                   HB2.Y < (HB1.Y + HB1.Height) &&
                   HB1.Y < (HB2.Y + HB2.Height);
        }

        public static void Intersects(Circle Circ, Collider Col)
        {
            // get distance between the point and circle's center
            // using the Pythagorean Theorem
            float distX = Col.X - Circ.X;
            float distY = Col.Y - Circ.Y;
            float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));
            // if the distance is less than the circle's
            // radius the point is inside!
            if (distance <= Circ.radius)
            {   
                //push collisions away from circle
                if (Circ.X < Col.X)
                { Col.acceleration.X += Rand.Next(0, 10) * 0.1f; }
                else { Col.acceleration.X -= Rand.Next(0, 10) * 0.1f; }
                
                //add randomness to x
                //Col.acceleration.X += Rand.Next(-5, 6) * 0.1f;

                Col.projectedPos.Y = Col.currentPos.Y;
                Col.acceleration.Y = 0.0f;
                Col.acceleration.Y -= Rand.Next(1, 8);
            }
        }


        //collisions use 3 hitboxes
        static Collider obj_hitbox_X = new Collider();
        static Collider obj_hitbox_Y = new Collider();
        static Collider obj_hitbox_XY = new Collider();
        //uses 3 boolean to track axis collisions
        static Boolean obj_collision_Xaxis;
        static Boolean obj_collision_Yaxis;
        static Boolean obj_collision_XYaxis;

        public static void Check_Blocker_Collisions(Collider collider)
        {
            //reset booleans
            obj_collision_Xaxis = false;
            obj_collision_Yaxis = false;
            obj_collision_XYaxis = false;

            //match hitboxes to hero hitbox
            obj_hitbox_X.Width = collider.Width;
            obj_hitbox_X.Height = collider.Height;
            obj_hitbox_Y.Width = collider.Width;
            obj_hitbox_Y.Height = collider.Height;
            obj_hitbox_XY.Width = collider.Width;
            obj_hitbox_XY.Height = collider.Height;


            #region Place XY, X, Y Hitbox instances

            //place X hitbox
            obj_hitbox_X.X = collider.projectedPos.X;
            obj_hitbox_X.Y = collider.currentPos.Y;
            //place Y hitbox
            obj_hitbox_Y.X = collider.currentPos.X;
            obj_hitbox_Y.Y = (float)Math.Ceiling(collider.projectedPos.Y);
            //place XY hitbox
            obj_hitbox_XY.X = collider.projectedPos.X;
            obj_hitbox_XY.Y = (float)Math.Ceiling(collider.projectedPos.Y);

            #endregion

            
            for (int c = 0; c < Pool.colliders.Count; c++)
            {
                if (collider == Pool.colliders[c]) { continue; }

                if (Intersects(obj_hitbox_X, Pool.colliders[c]))
                {
                    obj_collision_Xaxis = true;

                    if (Pool.colliders[c].acceptsPush)
                    {
                        if (Pool.colliders[c].X < collider.X)
                        { Pool.colliders[c].acceleration.X -= 1.0f; }
                        else { Pool.colliders[c].acceleration.X += 1.0f; }
                    }
                }
                if (Intersects(obj_hitbox_Y, Pool.colliders[c]))
                {
                    obj_collision_Yaxis = true;
                    //push colliders apart

                    if (Pool.colliders[c].acceptsPush)
                    {
                        if (Pool.colliders[c].Y < collider.Y)
                        { Pool.colliders[c].acceleration.Y -= 1.0f; }
                        else { Pool.colliders[c].acceleration.Y += 1.0f; }
                    }
                }

                if (Intersects(obj_hitbox_XY, Pool.colliders[c]))
                {
                    obj_collision_XYaxis = true;
                }
            }

            //pusher colliders ignore collisions
            //if (collider.acceptsPush == false) { obj_collision_XYaxis = false; }




            #region Resolve Movement based on collision booleans

            if (obj_collision_XYaxis == false)
            {
                //set last pos
                collider.lastPos.X = collider.currentPos.X;
                collider.lastPos.Y = collider.currentPos.Y;
                //get curr pos
                collider.currentPos.X = obj_hitbox_XY.X;
                collider.currentPos.Y = obj_hitbox_XY.Y;
            }
            else
            {   //there was a XY collision, but maybe we can move on one axis
                if (obj_collision_Xaxis == false)
                {
                    //set last pos
                    collider.lastPos.X = collider.currentPos.X;
                    collider.lastPos.Y = collider.currentPos.Y;
                    //get curr pos
                    collider.currentPos.X = obj_hitbox_X.X;
                    collider.currentPos.Y = obj_hitbox_X.Y;
                    //apply a random bounce up
                    //collider.acceleration.Y -= 2.0f;
                    //collider.acceleration.X += Rand.Next(-2, 2);
                }
                else if (obj_collision_Yaxis == false)
                {
                    //set last pos
                    collider.lastPos.X = collider.currentPos.X;
                    collider.lastPos.Y = collider.currentPos.Y;
                    //get curr pos
                    collider.currentPos.X = obj_hitbox_Y.X;
                    collider.currentPos.Y = obj_hitbox_Y.Y;
                    //apply a random bounce up
                    //collider.acceleration.Y -= 10.0f;
                    //collider.acceleration.X += Rand.Next(-5, 5);
                }
                else
                {
                    collider.currentPos.X = collider.lastPos.X;
                    collider.currentPos.Y = collider.lastPos.Y;
                    //apply a random bounce up
                    //collider.acceleration.Y -= 10.0f;
                    collider.acceleration.X += Rand.Next(-2, 2);
                }
            }

            #endregion


        }



    }
}
