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
            person.player = true;
            GameObject tree = new GameObject("Player", screenCenter + new Vector2(200, 0), "tree");

            Level testLevel = new Level();
            testLevel.gameObjects.Add(person);
            testLevel.gameObjects.Add(tree);
            currentLevel = testLevel;

            player = currentLevel.GetPlayer();

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
                if (keyboardState.IsKeyDown(Keys.Up)) moveDirection.Y--;
                if (keyboardState.IsKeyDown(Keys.Down)) moveDirection.Y++;
                if (keyboardState.IsKeyDown(Keys.Right)) moveDirection.X++;
                if (keyboardState.IsKeyDown(Keys.Left)) moveDirection.X--;

                player.position += moveDirection * 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
