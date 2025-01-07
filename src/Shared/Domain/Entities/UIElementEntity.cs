﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace UIElementEntity
{
    public abstract class UIElement
    {
        public Vector2 Position { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }

    public class Button : UIElement
    {
        public bool Hover { get; set; }
        public bool Click { get; set; }
        public Texture2D TextureNormal { get; set; }
        public Texture2D TextureHover { get; set; }
        public Texture2D TextureClick { get; set; }
        public enum ButtonAction {Resume, Exit};
        public ButtonAction Action { get; set; }

        public Button(Vector2 position, ContentManager content)
        {
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
            spriteBatch.Draw(texToDraw, Position, null, Color.White, 0f, new Vector2(texToDraw.Width / 2, texToDraw.Height / 2), Vector2.One, SpriteEffects.None, 0.1f);
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
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One, SpriteEffects.None, 0.2f);
        }
    }
}
