using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UIElementEntity
{
    public abstract class UIElement
    {
        public Vector2 Position { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public static List<UIElement> CreatePauseMenu(Vector2 screenCenter, ContentManager content)
        {
            List<UIElement> pauseMenu = new List<UIElement>();

            Image background = new Image(screenCenter, "UI/pause background", content);
            pauseMenu.Add(background);

            Button resumeButton = new Button(new Vector2(screenCenter.X - 200, screenCenter.Y - 37), content, Button.ButtonAction.Resume);
            pauseMenu.Add(resumeButton);

            Button exitButton = new Button(new Vector2(screenCenter.X - 200, screenCenter.Y + 37), content, Button.ButtonAction.Exit);
            pauseMenu.Add(exitButton);

            return pauseMenu;
        }

        public static List<UIElement> CreateGameOverMenu(Vector2 screenCenter, ContentManager content)
        {
            List<UIElement> gameOverMenu = new List<UIElement>();

            Image background = new Image(screenCenter, "UI/gameover background", content);
            gameOverMenu.Add(background);

            Button restartButton = new Button(new Vector2(screenCenter.X, screenCenter.Y + 32), content, Button.ButtonAction.Restart);
            gameOverMenu.Add(restartButton);

            Button exitButton = new Button(new Vector2(screenCenter.X, screenCenter.Y + 106), content, Button.ButtonAction.Exit);
            gameOverMenu.Add(exitButton);

            return gameOverMenu;
        }
    }

    public class Button : UIElement
    {
        public bool Hover { get; set; }
        public bool Click { get; set; }
        public Texture2D TextureNormal { get; set; }
        public Texture2D TextureHover { get; set; }
        public Texture2D TextureClick { get; set; }
        public enum ButtonAction {Resume, Exit, Restart};
        public ButtonAction Action { get; set; }

        public Button(Vector2 position, ContentManager content, ButtonAction action)
        {
            Action = action;
            Position = position;
            TextureNormal = content.Load<Texture2D>("UI/button normal");
            TextureHover = content.Load<Texture2D>("UI/button hover");
            TextureClick = content.Load<Texture2D>("UI/button clicked");
        }

        public void CheckHover(Point mousePosition)
        {
            Rectangle bounds = new Rectangle((int)Position.X - TextureNormal.Width / 2, (int)Position.Y - TextureNormal.Height / 2, TextureNormal.Width, TextureNormal.Height);
            Hover = bounds.Contains(mousePosition);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texToDraw = TextureNormal;
            if (Click) texToDraw = TextureClick;
            else if (Hover) texToDraw = TextureHover;
            spriteBatch.Draw(texToDraw, Position, null, Color.White, 0f, new Vector2(texToDraw.Width / 2, texToDraw.Height / 2), Vector2.One, SpriteEffects.None, 0.001f);
        }

        public void DrawText(SpriteBatch spriteBatch, SpriteFont font)
        {
            Vector2 fontOrigin = font.MeasureString(Action.ToString()) / 2;
            spriteBatch.DrawString(font, Action.ToString(), Position, Color.Black, 0, fontOrigin, 1.0f, SpriteEffects.None, 0f);
        }
    }

    public class Image : UIElement
    {
        public Texture2D Texture { get; set; }

        public Image(Vector2 position, string spriteDirectory, ContentManager content)
        {
            Position = position;
            Texture = content.Load<Texture2D>(spriteDirectory);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One, SpriteEffects.None, 0.002f);
        }

        public static void DrawLives(int lives, Vector2 origin, SpriteBatch spriteBatch, ContentManager content)
        {
            for (int i = 0; i < lives; i++)
            {
                Vector2 offset = new Vector2(i * 32, 0);
                Image head = new Image(origin + offset, "Sprites/head", content);
                head.Draw(spriteBatch);
            }
        }
    }
}
