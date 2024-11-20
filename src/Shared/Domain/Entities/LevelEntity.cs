using GameObjectEntity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEntity
{
    public class Level
    {
        public string name { get; set; } = "";
        public List<GameObject> gameObjects = new List<GameObject>();

        public void LoadGameObjectTextures(ContentManager content)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Texture = content.Load<Texture2D>(gameObject.TextureName);
            }
        }

        public void DrawGameObjects(SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.Texture != null)
                {
                    spriteBatch.Draw(
                    gameObject.Texture,
                    gameObject.Position + offset,
                    null,
                    Color.White,
                    0f,
                    new Vector2(gameObject.Texture.Width / 2, gameObject.Texture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    gameObject.Layer
                );
                }
            }
        }
    }
}
