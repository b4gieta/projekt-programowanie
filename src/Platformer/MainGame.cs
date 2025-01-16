using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameObjectEntity;
using LevelEntity;
using PhysicalBodyEntity;
using ControllerEntity;
using CameraEntity;
using AnimationEntity;
using SaveEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIElementEntity;
using System.Linq;

namespace Platformer
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        private Level CurrentLevel;
        private Camera MainCamera;

        private int Score;
        private float TimeToPoint;

        private SpriteFont DefaultFont;
        private Vector2 ScorePosition;

        private KeyboardState PreviousKeyboardState;
        private bool IsPaused;
        private bool GameOver;

        private List<UIElement> PauseMenu;
        private Image ScoreBackground;

        public MainGame()
        {
            IsPaused = false;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void InitializeCamera(GameObject target, Vector2 screenCenter)
        {
            MainCamera = new Camera(target, screenCenter);
            MainCamera.rightBorder = Convert.ToInt32(CurrentLevel.GridSize.X);
            MainCamera.topBorder = -512;
            MainCamera.bottomBorder = Convert.ToInt32(CurrentLevel.GridSize.Y);
        }

        private void InitializeUI()
        {
            Score = SaveSystem.Load().Score;
            ScorePosition = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, 30);
            ScoreBackground = new Image(ScorePosition, "UI/score background", Content);
        }

        private void InitializePauseMenu(Vector2 screenCenter)
        {
            PauseMenu = new List<UIElement>();

            Image background = new Image(screenCenter, "UI/pause background", Content);
            PauseMenu.Add(background);

            Button resumeButton = new Button(new Vector2(screenCenter.X - 200, screenCenter.Y - 74), Content, Button.ButtonAction.Resume);
            PauseMenu.Add(resumeButton);

            Button restartButton = new Button(new Vector2(screenCenter.X - 200, screenCenter.Y), Content, Button.ButtonAction.Restart);
            PauseMenu.Add(restartButton);

            Button exitButton = new Button(new Vector2(screenCenter.X - 200, screenCenter.Y + 74), Content, Button.ButtonAction.Exit);
            PauseMenu.Add(exitButton);
        }

        private void UpdateInput(GameObject gameObject)
        {
            gameObject.Controller.GetInput();
            if (gameObject.PhysicalBody.IsGrounded && gameObject.Controller.IsJumping && gameObject.PhysicalBody.Velocity.Y >= 0) gameObject.PhysicalBody.AddVelocityY(-20f);
            gameObject.PhysicalBody.AddVelocityX(gameObject.Controller.MoveX);
            if (gameObject.Controller.MoveX > 0) gameObject.Flip = false;
            else if (gameObject.Controller.MoveX < 0) gameObject.Flip = true;
            if (gameObject.Position.Y > CurrentLevel.GridSize.Y) GameOver = true;
        }

        private void UpdateEnemy(GameObject gameObject)
        {
            int checkX = gameObject.GetTile()[0];
            int checkY = gameObject.GetTile()[1];
            if (!gameObject.Enemy.IsGoingRight) checkX -= 1;
            if (checkX < 0 || checkX >= CurrentLevel.Tilemap[0].Length ||
                checkY < 0 || checkY >= CurrentLevel.Tilemap.Length)
            {
                gameObject.Enemy.IsGoingRight = !gameObject.Enemy.IsGoingRight;
            }
            else if (CurrentLevel.Tilemap[checkY][checkX] == 'E') gameObject.Enemy.IsGoingRight = !gameObject.Enemy.IsGoingRight;

            if (gameObject.Enemy.IsGoingRight) gameObject.PhysicalBody.AddVelocityX(1);
            else gameObject.PhysicalBody.AddVelocityX(-1);

            gameObject.Flip = !gameObject.Enemy.IsGoingRight;
        }

        private void UpdatePhysics(GameObject gameObject)
        {
            CheckXAxis(gameObject);
            CheckYAxis(gameObject);
            if (!gameObject.PhysicalBody.IsGrounded && gameObject.Animation != null) UpdateAnimations(gameObject);
        }

        private void CheckXAxis(GameObject gameObject)
        {
            gameObject.PhysicalBody.DampVelocityX(0.33f);
            Vector2 oldPosition = gameObject.Position;
            gameObject.SetPositionX(oldPosition.X + gameObject.PhysicalBody.Velocity.X);

            foreach (GameObject other in CurrentLevel.GameObjects)
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
                            GameOver = true;
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

        private void CheckYAxis(GameObject gameObject)
        {
            Vector2 oldPosition = gameObject.Position;
            if (!gameObject.PhysicalBody.IsGrounded) gameObject.PhysicalBody.AddVelocityY(0.9f);
            gameObject.SetPositionY(oldPosition.Y + gameObject.PhysicalBody.Velocity.Y);
            gameObject.PhysicalBody.IsGrounded = false;

            foreach (GameObject other in CurrentLevel.GameObjects)
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

        private void UpdateAnimations(GameObject gameObject)
        {
            if (gameObject.PhysicalBody.Velocity.Y < 0) gameObject.Animation.CurrentState = Animation.State.Jumping;
            else if (gameObject.PhysicalBody.Velocity.Y > 0) gameObject.Animation.CurrentState = Animation.State.Falling;
            else
            {
                if (gameObject.PhysicalBody.Velocity.X < 0.1f && gameObject.PhysicalBody.Velocity.X > -0.1f) gameObject.Animation.CurrentState = Animation.State.Idle;
                else gameObject.Animation.CurrentState = Animation.State.Walking;
            }
        }

        private void LockPlayerOnScreen()
        {
            GameObject player = CurrentLevel.GameObjects.Where(go => go.Controller != null).First();
            Rectangle hitbox = new Rectangle(0, 0, player.Width, player.Height);
            player.Position = player.PhysicalBody.KeepInScreenBoundsX(Graphics, hitbox, player.Position, MainCamera);
        }

        private void RemoveDeadEnemies()
        {
            List<GameObject> deadEnemies = CurrentLevel.GameObjects.Where(go => go.Enemy != null && go.Enemy.IsDead).ToList();
            foreach (GameObject e in deadEnemies) CurrentLevel.GameObjects.Remove(e);
        }

        private void LoadLevel(string levelName)
        {
            CurrentLevel = new Level(levelName);
            GameObject player = GameObject.GetPlayer(CurrentLevel.PlayerSpawn);
            CurrentLevel.GameObjects.Add(player);
            CurrentLevel.LoadGameObjectTextures(Content);
            MainCamera.Target = player;
            IsPaused = false;
            GameOver = false;
        }

        protected override void Initialize()
        {
            Vector2 screenCenter = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);

            CurrentLevel = new Level("lvl1.txt");
            GameObject player = GameObject.GetPlayer(CurrentLevel.PlayerSpawn);
            CurrentLevel.GameObjects.Add(player);

            InitializeCamera(player, screenCenter);
            InitializeUI();
            InitializePauseMenu(screenCenter);

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

            //Pause
            if (keyboardState.IsKeyDown(Keys.Escape) && keyboardState.IsKeyDown(Keys.Escape) != PreviousKeyboardState.IsKeyDown(Keys.Escape))
            {
                IsPaused = !IsPaused;
                SaveSystem.Save(Score);
                Debug.WriteLine(SaveSystem.GetSavePath());
            }

            if (!IsPaused)
            {
                foreach (GameObject gameObject in CurrentLevel.GameObjects)
                {
                    if (gameObject.Controller != null && !GameOver) UpdateInput(gameObject);
                    if (gameObject.Enemy != null) UpdateEnemy(gameObject);
                    if (gameObject.PhysicalBody != null && !gameObject.PhysicalBody.IsStatic) UpdatePhysics(gameObject);
                    if (gameObject.Animation != null) gameObject.Animation.EvaluateState(2f / 60f);
                    if (gameObject.Enemy != null && gameObject.Enemy.IsDead) Score += 1000;
                }

                RemoveDeadEnemies();

                LockPlayerOnScreen();
                MainCamera.UpdatePosition(Graphics);

                TimeToPoint += (1f / 60f);
                if (TimeToPoint > 0.5f && !GameOver)
                {
                    TimeToPoint = 0;
                    Score += 1;
                }
            }
            else
            {
                foreach (UIElement e in PauseMenu)
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
                            Exit();
                            break;
                        case Button.ButtonAction.Restart:
                            LoadLevel("lvl1.txt");
                            break;
                    }
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

            if (IsPaused)
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
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
