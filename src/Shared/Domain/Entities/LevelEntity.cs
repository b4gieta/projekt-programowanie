using GameObjectEntity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PhysicalBodyEntity;
using EnemyEntity;

namespace LevelEntity
{
    public class Level
    {
        public string Name { get; set; } = "";
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        public Vector2 PlayerSpawn { get; set; }
        public string[] Tilemap { get; set; }
        public Vector2 GridSize { get; set; }

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
                SpriteEffects flip = SpriteEffects.None;
                if (gameObject.Flip) flip = SpriteEffects.FlipHorizontally;

                if (gameObject.Texture != null)
                {
                    Rectangle? spriteSheetPart = null;
                    if (gameObject.Animation != null)
                    {
                        spriteSheetPart = gameObject.Animation.GetAnimationFrame(gameObject.Width, gameObject.Height);
                    }

                    spriteBatch.Draw(
                    gameObject.Texture,
                    gameObject.Position + offset,
                    spriteSheetPart,
                    Color.White,
                    0f,
                    new Vector2(gameObject.Width / 2, gameObject.Height / 2),
                    Vector2.One,
                    flip,
                    gameObject.Layer
                    );
                }
            }
        }

        public Level(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Content/Levels/" + fileName);
            string[] fileContent = File.ReadAllLines(filePath);
            Tilemap = fileContent;

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

                    else if (fileContent[i][j] == 'G')
                    {
                        GameObject enemy = Enemy.GetGoomba(new Vector2(32 + 64 * j, 32 + 64 * i), "Sprites/enemy anim");
                        GameObjects.Add(enemy);
                    }
                }
            }

            GridSize = new Vector2(fileContent[0].Length * 64, fileContent.Length * 64);
        }
    }
}
