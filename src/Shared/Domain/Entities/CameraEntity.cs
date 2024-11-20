using GameObjectEntity;
using Microsoft.Xna.Framework;
namespace CameraEntity
{
    public class Camera
    {
        public GameObject Target;
        public Vector2 Origin;
        public Vector2 Position; 

        public Camera(GameObject target, Vector2 origin)
        {
            Target = target;
            Origin = origin;
            Position = Origin;
        }

        public void UpdatePosition()
        {
            Position = Vector2.Lerp(Position, Target.Position, 0.2f);
        }
    }
}
