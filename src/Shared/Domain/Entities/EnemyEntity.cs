using Microsoft.Xna.Framework;
using GameObjectEntity;
using PhysicalBodyEntity;
using AnimationEntity;

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
            enemy.Animation = new Animation();
            enemy.Enemy = new Enemy();
            enemy.Layer = 0.6f;
            enemy.Width = 32;
            enemy.Height = 64;
            return enemy;
        }
    }
}
