using GameObjectEntity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PhysicalBodyEntity;

namespace LevelEntity
{
    public class Level
    {
        public string Name { get; set; } = "";
        public List<GameObject> GameObjects = new List<GameObject>();
        public Vector2 PlayerSpawn { get; set; }

        public void LoadGameObjectTextures(ContentManager content)
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Texture = content.Load<Texture2D>(gameObject.TextureName);
                gameObject.SetSize();
            }
        }

        public void DrawGameObjects(SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (GameObject gameObject in GameObjects)
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

        public Level(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Content/Levels/" + fileName);
            string[] fileContent = File.ReadAllLines(filePath);

            for (int i = 0; i < fileContent.Length; i++)
            {
                for (int j = 0; j < fileContent[i].Length; j++)
                {
                    if (fileContent[i][j] == 'D')
                    {
                        GameObject grass = new GameObject("Grass", new Vector2(32 + 64 * j, 32 + 64 * i), "Sprites/grass");
                        grass.PhysicalBody = new PhysicalBody();
                        grass.PhysicalBody.IsStatic = true;
                        GameObjects.Add(grass);
                    }

                    else if (fileContent[i][j] == 'P') PlayerSpawn = new Vector2(32 + 64 * j, 32 + 64 * i);
                }
            }
        }
    }
}
