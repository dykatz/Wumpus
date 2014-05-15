using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace HuntTheWumpus
{
    class AnimatedSprite : Sprite
    {
        float agregateTime = 0;
        int current = 0;
        Vector2i frame;
        Vector2i imageFrameCount;
        float totalTime;
        int total;

        public AnimatedSprite(Texture texture_, Vector2i frame_, float speed_, int total_)
            : base(texture_)
        {
            frame = frame_;
            totalTime = 1 / speed_;
            total = total_;
            imageFrameCount = new Vector2i((int)texture_.Size.X / frame_.X, (int)texture_.Size.Y / frame_.Y);
        }

        public void Update(float dt)
        {
            agregateTime += dt;

            while (agregateTime > totalTime)
            {
                current++;
                agregateTime -= totalTime;
            }

            while (current > total)
                current -= total;

            TextureRect = new IntRect((current % imageFrameCount.X) * frame.X, (current / imageFrameCount.Y) * frame.Y, frame.X, frame.Y);
        }
    }
}
