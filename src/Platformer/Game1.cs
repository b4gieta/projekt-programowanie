using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameObjectEntity;
using LevelEntity;
using PhysicalBodyEntity;
using ControllerEntity;

namespace Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        private Level CurrentLevel;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Vector2 screenCenter = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);

            GameObject person = new GameObject("Player", screenCenter, "person");
            person.PhysicalBody = new PhysicalBody();
            person.Controller = new Controller();
            person.Layer = 0.5f;

            GameObject tree = new GameObject("Tree", screenCenter + new Vector2(200, Graphics.PreferredBackBufferHeight / 2 - 96), "tree");
            tree.PhysicalBody = new PhysicalBody();
            tree.PhysicalBody.IsStatic = true;

            Level testLevel = new Level();
            testLevel.gameObjects.Add(person);
            testLevel.gameObjects.Add(tree);
            CurrentLevel = testLevel;

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

            foreach (GameObject gameObject in CurrentLevel.gameObjects)
            {
                //Input
                if (gameObject.Controller != null)
                {
                    gameObject.Controller.GetInput();
                    if (gameObject.PhysicalBody.IsGrounded && gameObject.Controller.IsJumping && gameObject.PhysicalBody.Velocity.Y >= 0) gameObject.PhysicalBody.AddVelocityY(-15f);
                    gameObject.PhysicalBody.AddVelocityX(gameObject.Controller.MoveX);
                }

                //Physics
                if (gameObject.PhysicalBody != null && !gameObject.PhysicalBody.IsStatic)
                {
                    //X axis
                    gameObject.PhysicalBody.DampVelocityX(0.33f);
                    Vector2 oldPosition = gameObject.Position;
                    gameObject.SetPositionX(oldPosition.X + gameObject.PhysicalBody.Velocity.X);

                    foreach (GameObject other in CurrentLevel.gameObjects)
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
                    if (!gameObject.PhysicalBody.IsGrounded) gameObject.PhysicalBody.AddVelocityY(0.5f);
                    gameObject.SetPositionY(oldPosition.Y + gameObject.PhysicalBody.Velocity.Y);
                    gameObject.PhysicalBody.IsGrounded = false;

                    foreach (GameObject other in CurrentLevel.gameObjects)
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
                            gameObject.PhysicalBody.IsGrounded = true;
                            gameObject.PhysicalBody.SetVelocityY(0);
                            break;
                        }
                    }
                }

                gameObject.Position = gameObject.PhysicalBody.KeepInScreenBounds(Graphics, gameObject.Texture, gameObject.Position);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(SpriteSortMode.BackToFront);
            CurrentLevel.DrawGameObjects(SpriteBatch);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
