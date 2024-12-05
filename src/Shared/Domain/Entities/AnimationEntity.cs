using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimationEntity
{
    public class Animation
    {
        public enum State { Idle, Walking, Jumping, Falling };
        public State CurrentState { get; set; }
        public int WalkFrame { get; set; }
        private float Time { get; set; }

        public Animation()
        {
            CurrentState = State.Idle;
        }

        public void EvaluateState(float time)
        {
            if (CurrentState == State.Walking)
            {
                Time += time;
                if (Time > 0.25f)
                {
                    WalkFrame++;
                    Time = 0;
                }
                if (WalkFrame > 3) WalkFrame = 0;
            }
            else
            {
                Time = 0;
                WalkFrame = 0;
            }
        }

        public Rectangle GetAnimationFrame(int width, int height)
        {
            int offset = 8;
            Rectangle result = new Rectangle(0, 0, width, height);
            if (CurrentState == State.Walking) result = new Rectangle((width + offset) * WalkFrame, height + offset, width, height);
            else if (CurrentState == State.Jumping) result = new Rectangle(0, (height + offset) * 2, width, height);
            else if (CurrentState == State.Falling) result = new Rectangle(width + offset, (height + offset) * 2, width, height);

            return result;
        }
    }
}
