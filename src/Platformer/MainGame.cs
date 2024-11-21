using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameObjectEntity;
using LevelEntity;
using PhysicalBodyEntity;
using ControllerEntity;
using CameraEntity;

namespace Platformer
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        private Level CurrentLevel;
        private Camera MainCamera;

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Vector2 screenCenter = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);

            CurrentLevel = new Level("lvl1.txt");

            GameObject person = new GameObject("Player", screenCenter, "Sprites/person");
            person.PhysicalBody = new PhysicalBody();
            person.Controller = new Controller();
            person.Layer = 0.5f;
            person.Position = CurrentLevel.PlayerSpawn;
            CurrentLevel.GameObjects.Add(person);

            MainCamera = new Camera(person, screenCenter);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            CurrentLevel.LoadGameObjectTextures(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            //Exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            //GameObjects
            foreach (GameObject gameObject in CurrentLevel.GameObjects)
            {
                //Input
                if (gameObject.Controller != null)
                {
                    gameObject.Controller.GetInput();
                    if (gameObject.PhysicalBody.IsGrounded && gameObject.Controller.IsJumping && gameObject.PhysicalBody.Velocity.Y >= 0) gameObject.PhysicalBody.AddVelocityY(-20f);
                    gameObject.PhysicalBody.AddVelocityX(gameObject.Controller.MoveX);
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
                        if (gameObject != other && other.PhysicalBody != null && gameObject.GetBoundingBox().Intersects(other.GetBoundingBox()))
                        {
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
                        if (gameObject != other && other.PhysicalBody != null && gameObject.GetBoundingBox().Intersects(other.GetBoundingBox()))
                        {
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

                if (gameObject.Controller != null) gameObject.Position = gameObject.PhysicalBody.KeepInScreenBounds(Graphics, gameObject.Texture, gameObject.Position, MainCamera);
            }

            //Camera
            MainCamera.UpdatePosition();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(SpriteSortMode.BackToFront);
            CurrentLevel.DrawGameObjects(SpriteBatch, MainCamera.Origin - MainCamera.Position);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
