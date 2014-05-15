using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace HuntTheWumpus
{
    class TweenFloat
    {
        float start, end;
        float time, speed;

        public TweenFloat(float start_, float end_, float time_)
        {
            start = start_;
            end = end_;
            time = time_;
            speed = (end - start) / time;
        }

        public void Update(ref float val, float dt)
        {
            if (speed > 0)
            {
                if (val < end)
                    val += speed * dt;

                if (val >= end)
                    val = end;
            }
            else
            {
                if (val > end)
                    val += speed * dt;

                if (val <= end)
                    val = end;
            }
        }
    }

    class TweenVector2f
    {
        Vector2f start, end, speed;
        float time;

        public TweenVector2f(Vector2f start_, Vector2f end_, float time_)
        {
            start = start_;
            end = end_;
            time = time_;
            speed = (end - start) / time;
        }

        public void Update(ref Vector2f val, float dt)
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
        }

        public void Update(ref CircleShape shape, float dt)
        {
            Vector2f v = shape.Position;
            Update(ref v, dt);
            shape.Position = v;
        }
    }
}
