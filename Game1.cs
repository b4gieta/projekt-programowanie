using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace projekt_programowanie
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Texture2D testCharacter;
        Vector2 testCharacterPosition;
        float testCharacterSpeed;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            testCharacterPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            testCharacterSpeed = 250f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            testCharacter = Content.Load<Texture2D>("test character");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            float scaledSpeed = testCharacterSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Up)) testCharacterPosition.Y -= scaledSpeed;
            if (keyboardState.IsKeyDown(Keys.Down)) testCharacterPosition.Y += scaledSpeed;
            if (keyboardState.IsKeyDown(Keys.Left)) testCharacterPosition.X -= scaledSpeed;
            if (keyboardState.IsKeyDown(Keys.Right)) testCharacterPosition.X += scaledSpeed;


            if (testCharacterPosition.X > graphics.PreferredBackBufferWidth - testCharacter.Width / 2) 
                testCharacterPosition.X = graphics.PreferredBackBufferWidth - testCharacter.Width / 2;
            else if (testCharacterPosition.X < testCharacter.Width / 2)
                testCharacterPosition.X = testCharacter.Width / 2;

            if (testCharacterPosition.Y > graphics.PreferredBackBufferHeight - testCharacter.Height / 2)
                testCharacterPosition.Y = graphics.PreferredBackBufferHeight - testCharacter.Height / 2;
            else if (testCharacterPosition.Y < testCharacter.Height / 2)
                testCharacterPosition.Y = testCharacter.Height / 2;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.Draw(
                testCharacter,
                testCharacterPosition,
                null,
                Color.White,
                0f,
                new Vector2(testCharacter.Width / 2, testCharacter.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
