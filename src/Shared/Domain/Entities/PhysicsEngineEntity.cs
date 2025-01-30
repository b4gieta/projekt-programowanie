using GameObjectEntity;
using AnimationEntity;
using LevelEntity;
using Microsoft.Xna.Framework;

namespace PhysicsEngineEntity
{
    public class PhysicsEngine
    {
        public void UpdatePhysics(GameObject gameObject, Level currentLevel)
        {
            CheckXAxis(gameObject, currentLevel);
            CheckYAxis(gameObject, currentLevel);
            if (!gameObject.PhysicalBody.IsGrounded && gameObject.Animation != null) UpdateAnimations(gameObject, currentLevel);
        }

        private void CheckXAxis(GameObject gameObject, Level currentLevel)
        {
            gameObject.PhysicalBody.DampVelocityX(0.33f);
            Vector2 oldPosition = gameObject.Position;
            gameObject.SetPositionX(oldPosition.X + gameObject.PhysicalBody.Velocity.X);

            foreach (GameObject other in currentLevel.GameObjects)
            {
                if (gameObject.Enemy != null && other.Enemy != null) continue;

                if (gameObject != other && other.PhysicalBody != null && gameObject.GetBoundingBox().Intersects(other.GetBoundingBox()))
                {
                    //Walk on player to kill him
                    if (gameObject.Enemy != null && other.Controller != null && !gameObject.Enemy.IsDead)
                    {
                        bool playerGoingDownwards = Math.Abs(other.PhysicalBody.Velocity.Y) > Math.Abs(other.PhysicalBody.Velocity.X);
                        bool playerFalling = other.PhysicalBody.Velocity.Y > 1;

                        if (!playerFalling || !playerGoingDownwards)
                        {
                            other.Controller.IsDead = true;
                            continue;
                        }
                    }

                    //Rest of physics
                    int steps = 10;
                    for (int i = 0; i < steps; i++)
                    {
                        gameObject.SetPositionX(oldPosition.X + gameObject.PhysicalBody.Velocity.X * (1f / steps) * (steps - i));
                        if (!gameObject.GetBoundingBox().Intersects(other.GetBoundingBox())) break;
                        gameObject.Position = oldPosition;
                    }
                    gameObject.PhysicalBody.SetVelocityX(0);
                    break;
                }
            }
        }

        private void CheckYAxis(GameObject gameObject, Level currentLevel)
        {
            Vector2 oldPosition = gameObject.Position;
            if (!gameObject.PhysicalBody.IsGrounded) gameObject.PhysicalBody.AddVelocityY(0.9f);
            gameObject.SetPositionY(oldPosition.Y + gameObject.PhysicalBody.Velocity.Y);
            gameObject.PhysicalBody.IsGrounded = false;

            foreach (GameObject other in currentLevel.GameObjects)
            {
                if (gameObject.Enemy != null && other.Enemy != null) continue;

                if (gameObject != other && other.PhysicalBody != null && gameObject.GetBoundingBox().Intersects(other.GetBoundingBox()))
                {
                    //Jump on enemy to kill him
                    if (gameObject.Controller != null && other.Enemy != null)
                    {
                        bool playerGoingDownwards = Math.Abs(gameObject.PhysicalBody.Velocity.Y) > Math.Abs(gameObject.PhysicalBody.Velocity.X);
                        bool playerFalling = gameObject.PhysicalBody.Velocity.Y > 1;

                        if (playerFalling && playerGoingDownwards && !gameObject.PhysicalBody.IsGrounded)
                        {
                            other.Enemy.IsDead = true;
                            gameObject.PhysicalBody.SetVelocityY(-20);
                            continue;
                        }
                    }

                    //Rest of physics
                    int steps = 10;
                    for (int i = 0; i < steps; i++)
                    {
                        gameObject.SetPositionY(oldPosition.Y + gameObject.PhysicalBody.Velocity.Y * (1f / steps) * (steps - i));
                        if (!gameObject.GetBoundingBox().Intersects(other.GetBoundingBox())) break;
                        gameObject.Position = oldPosition;
                    }
                    if (gameObject.PhysicalBody.Velocity.Y >= 0) gameObject.PhysicalBody.IsGrounded = true;
                    gameObject.PhysicalBody.SetVelocityY(0);

                    break;
                }
            }
        }

        private void UpdateAnimations(GameObject gameObject, Level currentLevel)
        {
            if (gameObject.PhysicalBody.Velocity.Y < 0) gameObject.Animation.CurrentState = Animation.State.Jumping;
            else if (gameObject.PhysicalBody.Velocity.Y > 0) gameObject.Animation.CurrentState = Animation.State.Falling;
            else
            {
                if (gameObject.PhysicalBody.Velocity.X < 0.1f && gameObject.PhysicalBody.Velocity.X > -0.1f) gameObject.Animation.CurrentState = Animation.State.Idle;
                else gameObject.Animation.CurrentState = Animation.State.Walking;
            }
        }
    }
}
