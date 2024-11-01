using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GameObjectEntity
{
    public class GameObject
    {
        public string name { get; set; } = "";
        public Vector2 position { get; set; }
        public string textureName { get; set; } = "";
        public Texture2D? texture { get; set; }
        public bool player { get; set; }
        public bool active { get; set; } = true;

        public GameObject(string name, Vector2 position, string textureName)
        {
            this.name = name;
            this.position = position;
            this.textureName = textureName;
        }

        public Rectangle GetBoundingBox()
        {
            if (texture != null) return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            return new Rectangle((int)position.X, (int)position.Y,0,0);
        }
    }
}
