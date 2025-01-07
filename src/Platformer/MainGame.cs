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

        SpriteFont DefaultFont;
        Vector2 FontPosition;

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Vector2 screenCenter = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
            FontPosition = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, 100);

            CurrentLevel = new Level("lvl1.txt");

            GameObject person = new GameObject("Player", CurrentLevel.PlayerSpawn, "Sprites/player anim");
            person.PhysicalBody = new PhysicalBody();
            person.Controller = new Controller();
            person.Animation = new Animation();
            person.Layer = 0.5f;
            person.Width = 32;
            person.Height = 64;
            CurrentLevel.GameObjects.Add(person);

            MainCamera = new Camera(person, screenCenter);
            MainCamera.rightBorder = Convert.ToInt32(CurrentLevel.GridSize.X);
            MainCamera.topBorder = -512;
            MainCamera.bottomBorder = Convert.ToInt32(CurrentLevel.GridSize.Y);

            Score = SaveSystem.Load().Score;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            CurrentLevel.LoadGameObjectTextures(Content);
            DefaultFont = Content.Load<SpriteFont>("Arial");
        }

        protected override void Update(GameTime gameTime)
        {
            //Exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                SaveSystem.Save(Score);
                Debug.WriteLine(SaveSystem.GetSavePath());
                Exit();
            }            

            //GameObjects
            List<GameObject> gameObjectsToRemove = new List<GameObject>();
            foreach (GameObject gameObject in CurrentLevel.GameObjects)
            {
                //Input
                if (gameObject.Controller != null)
                {
                    gameObject.Controller.GetInput();
                    if (gameObject.PhysicalBody.IsGrounded && gameObject.Controller.IsJumping && gameObject.PhysicalBody.Velocity.Y >= 0) gameObject.PhysicalBody.AddVelocityY(-20f);
                    gameObject.PhysicalBody.AddVelocityX(gameObject.Controller.MoveX);
                    if (gameObject.Controller.MoveX > 0) gameObject.Flip = false;
                    else if (gameObject.Controller.MoveX < 0) gameObject.Flip = true;
                }

                //Enemy
                if (gameObject.Enemy != null)
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

                //Physics
                if (gameObject.PhysicalBody != null && !gameObject.PhysicalBody.IsStatic)
                {
                    //X axis
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
                                    gameObjectsToRemove.Add(other);
                                    Debug.WriteLine(other.PhysicalBody.Velocity.Y.ToString());
                                    continue;
                                }
                            }

                            //Rest of physics
                            int steps = 10;
                            for(int i = 0; i < steps; i++)
                            {
                                gameObject.SetPositionX(oldPosition.X + gameObject.PhysicalBody.Velocity.X * (1f / steps) * (steps - i));
                                if (!gameObject.GetBoundingBox().Intersects(other.GetBoundingBox())) break;
                                gameObject.Position = oldPosition;
                            }
                            gameObject.PhysicalBody.SetVelocityX(0);
                            break;
                        }
                    }

                    //Y axis
                    oldPosition = gameObject.Position;
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

                    //Keep player in screen bounds horizontally
                    if (gameObject.Controller != null)
                    {
                        Rectangle hitbox = new Rectangle(0, 0, gameObject.Width, gameObject.Height);
                        gameObject.Position = gameObject.PhysicalBody.KeepInScreenBoundsX(Graphics, hitbox, gameObject.Position, MainCamera);
                    }

                    //Set animations
                    if (!gameObject.PhysicalBody.IsGrounded && gameObject.Animation != null)
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

                //Animation
                if (gameObject.Animation != null) gameObject.Animation.EvaluateState(2f/60f);

                //Get dead enemnies
                if (gameObject.Enemy != null && gameObject.Enemy.IsDead)
                {
                    Score += 1000;
                    gameObjectsToRemove.Add(gameObject);
                }
            }

            //Remove dead enemies
            foreach (GameObject gameObject in gameObjectsToRemove)
            {
                if (CurrentLevel.GameObjects.Contains(gameObject)) CurrentLevel.GameObjects.Remove(gameObject);
            }

            //Camera
            MainCamera.UpdatePosition(Graphics);

            //Score
            TimeToPoint += (1f / 60f);
            if (TimeToPoint > 0.5f)
            {
                TimeToPoint = 0;
                Score += 1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(SpriteSortMode.BackToFront);

            CurrentLevel.DrawGameObjects(SpriteBatch, MainCamera.Origin - MainCamera.Position);

            string output = "Score: " + Score.ToString();
            Vector2 FontOrigin = DefaultFont.MeasureString(output) / 2;
            SpriteBatch.DrawString(DefaultFont, output, FontPosition, Color.Black, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.1f);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
