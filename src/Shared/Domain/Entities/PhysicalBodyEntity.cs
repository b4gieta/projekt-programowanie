using CameraEntity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicalBodyEntity
{
    public class PhysicalBody
    {
        public bool IsStatic { get; set; }
        public bool IsGrounded { get; set; }
        public Vector2 Velocity { get; set; }

        public void AddVelocityX(float gain)
        {
            Velocity = new Vector2(Velocity.X + gain, Velocity.Y);
            if (Velocity.X > 6) Velocity = new Vector2(6, Velocity.Y);
            else if (Velocity.X < -6) Velocity = new Vector2(-6, Velocity.Y);
        }

        public void AddVelocityY(float gain)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + gain);
        }

        public void SetVelocityX(float vel)
        {
            Velocity = new Vector2(vel, Velocity.Y);
        }

        public void SetVelocityY(float vel)
        {
            Velocity = new Vector2(Velocity.X, vel);
        }

        public void DampVelocityX(float dampening)
        {
            if (Velocity.X > 0)
            {
                Velocity = new Vector2(Velocity.X - dampening, Velocity.Y);
                if (Velocity.X < 0) Velocity = new Vector2(0, Velocity.Y);
            }
            else if (Velocity.X < 0)
            {
                Velocity = new Vector2(Velocity.X + dampening, Velocity.Y);
                if (Velocity.X > 0) Velocity = new Vector2(0, Velocity.Y);
            }
        }

        public Vector2 KeepInScreenBounds(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, Camera cam)
        {
            Vector2 newPosition = position;
            Vector2 camOffset = cam.Position - cam.Origin;

            if (position.X - camOffset.X > graphics.PreferredBackBufferWidth - texture.Width / 2)
            {
                newPosition.X = graphics.PreferredBackBufferWidth - texture.Width / 2;
            }
            else if (position.X - camOffset.X < texture.Width / 2)
            {
                newPosition.X = texture.Width / 2;
            }

            if (position.Y - camOffset.Y > graphics.PreferredBackBufferHeight - texture.Height / 2)
            {
                newPosition.Y = graphics.PreferredBackBufferHeight - texture.Height / 2;
                IsGrounded = true;
                SetVelocityY(0);
            }
            else if (position.Y - camOffset.Y < texture.Height / 2)
            {
                newPosition.Y = texture.Height / 2;
                SetVelocityY(0);
            }

            return newPosition;
        }
    }
}
