using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameObjectEntity;
using LevelEntity;

namespace Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Level currentLevel;
        private GameObject player;
        private KeyboardState previousKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Vector2 screenCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            GameObject person = new GameObject("Player", screenCenter, "person");
            person.isPlayer = true;
            GameObject tree = new GameObject("Tree", screenCenter + new Vector2(200, graphics.PreferredBackBufferHeight / 2 - 96), "tree");
            tree.isStatic = true;

            Level testLevel = new Level();
            testLevel.gameObjects.Add(person);
            testLevel.gameObjects.Add(tree);
            currentLevel = testLevel;

            player = currentLevel.GetPlayer();

            previousKeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            currentLevel.LoadGameObjectTextures(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (player != null)
            {
                var keyboardState = Keyboard.GetState();
                Vector2 moveDirection = new Vector2();
                if (keyboardState.IsKeyDown(Keys.Right)) moveDirection.X++;
                if (keyboardState.IsKeyDown(Keys.Left)) moveDirection.X--;
                if (keyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyDown(Keys.Up) != previousKeyboardState.IsKeyDown(Keys.Up) && (player.isGrounded || player.isOnBottomEdge)) 
                    player.velocity = new Vector2(player.velocity.X, -10);

                player.isGrounded = false;
                foreach (GameObject gameObject in currentLevel.gameObjects)
                {
                    if (gameObject != player && player.GetBoundingBox().Intersects(gameObject.GetBoundingBox()))
                    {
                        if (moveDirection.X > 0 && player.position.X < gameObject.position.X) moveDirection.X = 0;
                        if (moveDirection.X < 0 && player.position.X > gameObject.position.X) moveDirection.X = 0;
                    }
                    if (gameObject != player && player.GetGroundCheck().Intersects(gameObject.GetBoundingBox())) player.isGrounded = true;
                }

                player.position += moveDirection * 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                previousKeyboardState = keyboardState;
            }

            foreach (GameObject gameObject in currentLevel.gameObjects)
            {
                if (!gameObject.isStatic)
                {                    
                    if (!gameObject.isGrounded) gameObject.velocity = new Vector2(gameObject.velocity.X, gameObject.velocity.Y + 0.2f);
                    else if (gameObject.velocity.Y > 0.5f) gameObject.velocity = new Vector2(gameObject.velocity.X, 0);
                    gameObject.position += gameObject.velocity;
                    gameObject.KeepInScreenBounds(graphics);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.BackToFront);
            currentLevel.DrawGameObjects(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
