using Microsoft.Xna.Framework;
using GameObjectEntity;
using PhysicalBodyEntity;
using AnimationEntity;
using LevelEntity;

namespace EnemyEntity
{
    public class Enemy
    {
        public bool IsGoingRight { get; set; }
        public bool IsDead { get; set; }

        public static GameObject GetGoomba(Vector2 spawn, string spriteSheetName)
        {
            GameObject enemy = new GameObject("Goomba", spawn, spriteSheetName);
            enemy.PhysicalBody = new PhysicalBody();
            enemy.PhysicalBody.MaxVelocityX = 3f;
            enemy.Animation = new Animation();
            enemy.Enemy = new Enemy();
            enemy.Enemy.IsGoingRight = true;
            enemy.Layer = 0.11f;
            enemy.Width = 32;
            enemy.Height = 64;
            return enemy;
        }

        public void Update(GameObject gameObject, Level currentLevel)
        {
            int checkX = gameObject.GetTile()[0];
            int checkY = gameObject.GetTile()[1];
            if (!gameObject.Enemy.IsGoingRight) checkX -= 1;
            if (checkX < 0 || checkX >= currentLevel.Tilemap[0].Length ||
                checkY < 0 || checkY >= currentLevel.Tilemap.Length)
            {
                gameObject.Enemy.IsGoingRight = !gameObject.Enemy.IsGoingRight;
            }
            else if (currentLevel.Tilemap[checkY][checkX] == 'E') gameObject.Enemy.IsGoingRight = !gameObject.Enemy.IsGoingRight;

            if (gameObject.Enemy.IsGoingRight) gameObject.PhysicalBody.AddVelocityX(1);
            else gameObject.PhysicalBody.AddVelocityX(-1);

            gameObject.Flip = !gameObject.Enemy.IsGoingRight;
        }
    }
}
