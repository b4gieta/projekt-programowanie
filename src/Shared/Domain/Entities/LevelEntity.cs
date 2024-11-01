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
                gameObject.texture = content.Load<Texture2D>(gameObject.textureName);
            }
        }

        public void DrawGameObjects(SpriteBatch spriteBatch)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                float layer = 1.0f;
                if (gameObject.player) layer = 0.0f;

                if (gameObject.texture != null)
                {
                    spriteBatch.Draw(
                    gameObject.texture,
                    gameObject.position,
                    null,
                    Color.White,
                    0f,
                    new Vector2(gameObject.texture.Width / 2, gameObject.texture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    layer
                );
                }
            }
        }

        public GameObject? GetPlayer()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].player) return gameObjects[i];
            }

            return null;
        }
    }
}
