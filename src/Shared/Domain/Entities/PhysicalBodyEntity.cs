using CameraEntity;
using Microsoft.Xna.Framework;

namespace PhysicalBodyEntity
{
    public class PhysicalBody
    {
        public bool IsStatic { get; set; }
        public bool IsGrounded { get; set; }
        public Vector2 Velocity { get; set; }
        public float MaxVelocityX { get; set; } = 6f;

        public void AddVelocityX(float gain)
        {
            Velocity = new Vector2(Velocity.X + gain, Velocity.Y);
            if (Velocity.X > MaxVelocityX) Velocity = new Vector2(MaxVelocityX, Velocity.Y);
            else if (Velocity.X < -MaxVelocityX) Velocity = new Vector2(-MaxVelocityX, Velocity.Y);
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

        public Vector2 KeepInScreenBoundsX(GraphicsDeviceManager graphics, Rectangle hitbox, Vector2 position, Camera cam)
        {
            Vector2 newPosition = position;
            Vector2 camOffset = cam.Position - cam.Origin;

            if (position.X - camOffset.X > graphics.GraphicsDevice.Viewport.Width - hitbox.Width / 2)
            {
                newPosition.X = graphics.GraphicsDevice.Viewport.Width / 2 + cam.Position.X - hitbox.Width / 2;
                SetVelocityX(0);
            }
            else if (position.X - camOffset.X < hitbox.Width / 2)
            {
                newPosition.X = hitbox.Width / 2;
                SetVelocityX(0);
            }

            return newPosition;
        }

        public Vector2 KeepInScreenBoundsY(GraphicsDeviceManager graphics, Rectangle hitbox, Vector2 position, Camera cam)
        {
            Vector2 newPosition = position;
            Vector2 camOffset = cam.Position - cam.Origin;

            if (position.Y - camOffset.Y > graphics.GraphicsDevice.Viewport.Height - hitbox.Height / 2)
            {
                newPosition.Y = graphics.GraphicsDevice.Viewport.Height / 2 + cam.Position.Y - hitbox.Height / 2;
                SetVelocityY(0);
            }
            else if (position.Y - camOffset.Y < hitbox.Height / 2)
            {
                newPosition.Y = hitbox.Height / 2;
                SetVelocityY(0);
            }

            return newPosition;
        }
    }
}
