using AnimationEntity;
using ControllerEntity;
using EnemyEntity;
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
        public Vector2 TextureOffset { get; set; }
        public Vector2 Parallax { get; set; }
        public float Layer { get; set; } = 0.7f;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Flip { get; set; }
        public PhysicalBody? PhysicalBody { get; set; }
        public Controller? Controller { get; set; }
        public Animation? Animation { get; set; }
        public Enemy? Enemy { get; set; }
        
        public GameObject(string name, Vector2 position, string textureName)
        {
            Name = name;
            Position = position;
            TextureName = textureName;
            Parallax = new Vector2(1, 1);
        }

        public void SetPositionX(float posX)
        {
            Position = new Vector2(posX, Position.Y);
        }

        public void SetPositionY(float posY)
        {
            Position = new Vector2(Position.X, posY);
        }

        public void SetSize()
        {
            if (Texture != null)
            {
                if (Width == 0) Width = Texture.Width;
                if (Height == 0) Height = Texture.Height;
            }
        }

        public Rectangle GetBoundingBox()
        {
            if (Texture != null) return new Rectangle((int)Position.X - Width / 2, (int)Position.Y - Height / 2, Width, Height);
            else return new Rectangle((int)Position.X, (int)Position.Y, 0, 0);
        }

        public int[] GetTile()
        {
            int[] result = [(int)Math.Round((Position.X / 64)), (int)Math.Round((Position.Y / 64))];
            return result;
        }

        public static GameObject GetPlayer(Vector2 spawn)
        {
            GameObject person = new GameObject("Player", spawn, "Sprites/player anim");
            person.PhysicalBody = new PhysicalBody();
            person.Controller = new Controller();
            person.Animation = new Animation();
            person.Layer = 0.1f;
            person.Width = 32;
            person.Height = 64;
            return person;
        }
    }
}
