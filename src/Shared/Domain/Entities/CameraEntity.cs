using GameObjectEntity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace CameraEntity
{
    public class Camera
    {
        public GameObject Target { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Position { get; set; }
        public int leftBorder { get; set; } = 0;
        public int rightBorder { get; set; } = 2048;
        public int topBorder { get; set; } = 0;
        public int bottomBorder { get; set; } = 2048;

        public Camera(GameObject target, Vector2 origin)
        {
            Target = target;
            Origin = origin;
            Position = Origin;
        }

        private Vector2 GetClampedPosition(GraphicsDeviceManager graphics)
        {
            Vector2 targetPos = Target.Position;
            targetPos.X = Math.Clamp(targetPos.X, leftBorder + graphics.GraphicsDevice.Viewport.Width / 2, rightBorder - graphics.GraphicsDevice.Viewport.Width / 2);
            targetPos.Y = Math.Clamp(targetPos.Y, topBorder + graphics.GraphicsDevice.Viewport.Height / 2, bottomBorder - graphics.GraphicsDevice.Viewport.Height / 2);
            return targetPos;
        }

        public void UpdatePosition(GraphicsDeviceManager graphics)
        {
            Vector2 targetPos = GetClampedPosition(graphics);
            Position = Vector2.Lerp(Position, targetPos, 0.2f);
        }

        public void ForceFocusOnTarget(GraphicsDeviceManager graphics)
        {
            Position = GetClampedPosition(graphics);
            UpdatePosition(graphics);
        }
    }
}
