using GameObjectEntity;
using LevelEntity;
using Microsoft.Xna.Framework.Input;
namespace ControllerEntity
{
    public class Controller
    {
        public int MoveX { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDead { get; set; }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MoveX = 0;
            if (keyboardState.IsKeyDown(Keys.Left)) MoveX--;
            if (keyboardState.IsKeyDown(Keys.Right)) MoveX++;

            IsJumping = keyboardState.IsKeyDown(Keys.Up);
        }

        public bool Update(GameObject gameObject, Level currentLevel)
        {
            gameObject.Controller.GetInput();
            if (gameObject.PhysicalBody.IsGrounded && gameObject.Controller.IsJumping && gameObject.PhysicalBody.Velocity.Y >= 0) gameObject.PhysicalBody.AddVelocityY(-20f);
            gameObject.PhysicalBody.AddVelocityX(gameObject.Controller.MoveX);
            if (gameObject.Controller.MoveX > 0) gameObject.Flip = false;
            else if (gameObject.Controller.MoveX < 0) gameObject.Flip = true;
            if (gameObject.Position.Y > currentLevel.GridSize.Y) return true;
            else return false;
        }
    }
}
