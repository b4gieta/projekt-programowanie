using Microsoft.Xna.Framework.Input;
namespace ControllerEntity
{
    public class Controller
    {
        public int MoveX { get; set; }
        public bool IsJumping { get; set; }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MoveX = 0;
            if (keyboardState.IsKeyDown(Keys.Left)) MoveX--;
            if (keyboardState.IsKeyDown(Keys.Right)) MoveX++;

            IsJumping = keyboardState.IsKeyDown(Keys.Up);
        }
    }
}
