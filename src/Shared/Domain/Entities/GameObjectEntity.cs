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
        public bool isPlayer { get; set; }
        public bool isActive { get; set; } = true;
        public bool isStatic { get; set; } = false;
        public Vector2 velocity { get; set; }
        public bool isGrounded { get; set; }
        public bool isOnBottomEdge { get; set; }

        public GameObject(string name, Vector2 position, string textureName)
        {
            this.name = name;
            this.position = position;
            this.textureName = textureName;
        }

        public Rectangle GetBoundingBox()
        {
            if (texture != null) return new Rectangle((int)position.X - texture.Width / 2, (int)position.Y - texture.Height / 2, texture.Width, texture.Height);
            else return new Rectangle((int)position.X, (int)position.Y,0,0);
        }

        public Rectangle GetGroundCheck()
        {
            if (texture != null) return new Rectangle((int)position.X + 2 - texture.Width / 2, (int)position.Y + texture.Height / 2, texture.Width - 4, 5);
            else return new Rectangle((int)position.X, (int)position.Y, 0, 0);
        }

        public void SetPositionX(float posX)
        {
            position = new Vector2(posX, position.Y);
        }

        public void SetPositionY(float posY)
        {
            position = new Vector2(position.X, posY);
        }

        public void KeepInScreenBounds(GraphicsDeviceManager graphics)
        {
            if (texture is not null)
            {
                if (position.X > graphics.PreferredBackBufferWidth - texture.Width / 2)
                {
                    SetPositionX(graphics.PreferredBackBufferWidth - texture.Width / 2);
                }
                else if (position.X < texture.Width / 2)
                {
                    SetPositionX(texture.Width / 2);
                }

                if (position.Y > graphics.PreferredBackBufferHeight - texture.Height / 2)
                {
                    SetPositionY(graphics.PreferredBackBufferHeight - texture.Height / 2);
                    isOnBottomEdge = true;
                }
                else if (position.Y < texture.Height / 2)
                {
                    SetPositionY(texture.Height / 2);
                    isOnBottomEdge = false;
                }
                else
                {
                    isOnBottomEdge = false;
                }
            }
            
        }
    }
}
