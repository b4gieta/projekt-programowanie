using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameObjectEntity;
using LevelEntity;
using CameraEntity;
using SaveEntity;
using System.Collections.Generic;
using UIElementEntity;
using System.Linq;
using PhysicsEngineEntity;

namespace Platformer
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        private Level CurrentLevel;
        private Camera MainCamera;
        private PhysicsEngine Physics;

        private int Lives;
        private int Score;
        private float TimeToPoint;

        private SpriteFont DefaultFont;
        private Vector2 ScorePosition;

        private KeyboardState PreviousKeyboardState;
        private bool IsPaused;
        private bool GameOver;

        private List<UIElement> PauseMenu;
        private List<UIElement> GameOverMenu;
        private Image ScoreBackground;

        public MainGame()
        {
            IsPaused = false;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void InitializeUI(Vector2 screenCenter)
        {
            Score = SaveSystem.Load().Score;
            ScorePosition = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, 30);
            ScoreBackground = new Image(ScorePosition, "UI/score background", Content);
            PauseMenu = UIElement.CreatePauseMenu(screenCenter, Content);
            GameOverMenu = UIElement.CreateGameOverMenu(screenCenter, Content);
        }

        private void LockPlayerOnScreen()
        {
            GameObject player = CurrentLevel.GameObjects.Where(go => go.Controller != null).First();
            Rectangle hitbox = new Rectangle(0, 0, player.Width, player.Height);
            player.Position = player.PhysicalBody.KeepInScreenBoundsX(Graphics, hitbox, player.Position, MainCamera);
        }

        private void CheckGameOver()
        {
            GameObject player = CurrentLevel.GameObjects.Where(go => go.Controller != null).First();
            GameOver = player.Controller.IsDead;
        }

        private void UpdateMenu(List<UIElement> menu, MouseState mouseState)
        {
            foreach (UIElement e in menu)
            {
                if (e is not Button) continue;
                Button button = e as Button;
                button.CheckHover(mouseState.Position);
                if (!button.Hover || mouseState.LeftButton != ButtonState.Pressed) continue;
                switch (button.Action)
                {
                    case Button.ButtonAction.Resume:
                        IsPaused = false;
                        break;
                    case Button.ButtonAction.Exit:
                        SaveSystem.Save(Score, Lives);
                        Exit();
                        break;
                    case Button.ButtonAction.Restart:
                        LoadLevel("lvl1.txt");
                        break;
                }
            }
        }

        private void LoadLevel(string levelName)
        {
            Lives--;
            if (Lives == 0)
            {
                Score = 0;
                Lives = 3;
            }

            SaveSystem.Save(Score, Lives);
            CurrentLevel = new Level(levelName);
            GameObject player = GameObject.GetPlayer(CurrentLevel.PlayerSpawn);
            CurrentLevel.GameObjects.Add(player);
            CurrentLevel.LoadGameObjectTextures(Content);
            MainCamera.Target = player;
            MainCamera.ForceFocusOnTarget(Graphics);
            IsPaused = false;
            GameOver = false;
        }

        protected override void Initialize()
        {
            Vector2 screenCenter = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
            Lives = 3;
            CurrentLevel = new Level("lvl1.txt");
            GameObject player = GameObject.GetPlayer(CurrentLevel.PlayerSpawn);
            CurrentLevel.GameObjects.Add(player);
            Physics = new PhysicsEngine();
            MainCamera = new Camera(player, screenCenter, CurrentLevel);
            InitializeUI(screenCenter);            

            PreviousKeyboardState = Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            CurrentLevel.LoadGameObjectTextures(Content);
            DefaultFont = Content.Load<SpriteFont>("Calibri");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) && keyboardState.IsKeyDown(Keys.Escape) != PreviousKeyboardState.IsKeyDown(Keys.Escape)) IsPaused = !IsPaused;

            if (GameOver) UpdateMenu(GameOverMenu, mouseState);
            else if (IsPaused) UpdateMenu(PauseMenu, mouseState);
            else
            {
                foreach (GameObject gameObject in CurrentLevel.GameObjects)
                {
                    if (gameObject.Controller != null && !GameOver) GameOver = gameObject.Controller.Update(gameObject, CurrentLevel);
                    if (gameObject.Enemy != null) gameObject.Enemy.Update(gameObject, CurrentLevel);
                    if (gameObject.PhysicalBody != null && !gameObject.PhysicalBody.IsStatic) Physics.UpdatePhysics(gameObject, CurrentLevel);
                    if (gameObject.Animation != null) gameObject.Animation.EvaluateState(2f / 60f);
                }

                Score += CurrentLevel.GetPointsForEnemies();
                if (!GameOver) CheckGameOver();
                LockPlayerOnScreen();
                MainCamera.UpdatePosition(Graphics);

                TimeToPoint += (1f / 60f);
                if (TimeToPoint > 0.5f && !GameOver)
                {
                    TimeToPoint = 0;
                    Score += 1;
                }
            }

            PreviousKeyboardState = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(SpriteSortMode.BackToFront);

            CurrentLevel.DrawGameObjects(SpriteBatch, MainCamera.Origin - MainCamera.Position);

            if (GameOver)
            {
                foreach (UIElement e in GameOverMenu)
                {
                    e.Draw(SpriteBatch);
                    if (e is Button) (e as Button).DrawText(SpriteBatch, DefaultFont);
                }
            }
            else if (IsPaused)
            {
                foreach (UIElement e in PauseMenu)
                {
                    e.Draw(SpriteBatch);
                    if (e is Button) (e as Button).DrawText(SpriteBatch, DefaultFont);
                }
            }
            else
            {
                string output = $"Score: {Score}";
                Vector2 fontOrigin = DefaultFont.MeasureString(output) / 2;
                ScoreBackground.Draw(SpriteBatch);
                SpriteBatch.DrawString(DefaultFont, output, ScorePosition, Color.White, 0, fontOrigin, 1.0f, SpriteEffects.None, 0f);
                Image.DrawLives(Lives, new Vector2(24, 24), SpriteBatch, Content);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
