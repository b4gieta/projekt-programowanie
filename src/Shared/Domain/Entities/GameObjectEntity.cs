using ControllerEntity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicalBodyEntity;
namespace GameObjectEntity
{
    public class GameObject
    {
        public string Name { get; set; } = "";
        public Vector2 Position { get; set; }
        public string TextureName { get; set; } = "";
        public Texture2D? Texture { get; set; }
        public float layer { get; set; } = 1;
        public PhysicalBody? PhysicalBody { get; set; }
        public Controller? Controller { get; set; }

        public GameObject(string name, Vector2 position, string textureName)
        {
            Name = name;
            Position = position;
            TextureName = textureName;
        }

        public void SetPositionX(float posX)
        {
            Position = new Vector2(posX, Position.Y);
        }

        public void SetPositionY(float posY)
        {
            Position = new Vector2(Position.X, posY);
        }

        public Rectangle GetBoundingBox()
        {
            if (Texture != null) return new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            else return new Rectangle((int)Position.X, (int)Position.Y, 0, 0);
        }
    }
}
