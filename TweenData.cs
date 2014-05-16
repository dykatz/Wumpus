using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace HuntTheWumpus
{
    struct TweenVector2f
    {
        Vector2f end, speed;
        float time;
        public bool Active;

        public TweenVector2f(Vector2f start_, Vector2f end_, float time_)
        {
            end = end_;
            time = time_;
            speed = (end - start_) / time;
            Active = true;
        }

        public void Update(ref Vector2f val, float dt)
        {
            if (Active)
            {
                if (speed.X > 0)
                {
                    if (val.X < end.X)
                        val.X += speed.X * dt;

                    if (val.X >= end.X)
                        val.X = end.X;
                }
                else
                {
                    if (val.X > end.X)
                        val.X += speed.X * dt;

                    if (val.X <= end.X)
                        val.X = end.X;
                }

                if (speed.Y > 0)
                {
                    if (val.Y < end.Y)
                        val.Y += speed.Y * dt;

                    if (val.Y >= end.Y)
                        val.Y = end.Y;
                }
                else
                {
                    if (val.Y > end.Y)
                        val.Y += speed.Y * dt;

                    if (val.Y <= end.Y)
                        val.Y = end.Y;
                }

                if (val.X == end.X && val.Y == end.Y)
                    Active = false;
            }
        }

        public void Update(ref CircleShape shape, float dt)
        {
            Vector2f v = shape.Position;
            Update(ref v, dt);
            shape.Position = v;
        }
    }
}
