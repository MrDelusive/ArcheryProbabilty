using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProbabilityCircle
{

    public class Image
    {
        public float Alpha;
        public string FontName, Path;
        public Vector2 Position, Scale;
        public Vector2 dimensions = Vector2.Zero;
        public Rectangle SourceRect;
        public bool IsActive;
        public string Effects;
        public float Rotation;
        public Color color = Color.White;
        public Texture2D Texture;

        public Vector2 origin;
        public RenderTarget2D renderTarget;
        public float Radius;

        public Image(string path)
        {
            Path = path;
            Scale = Vector2.One;

            Alpha = 1.0f;
            Rotation = 0f;
            SourceRect = Rectangle.Empty;
        }

        public void LoadContent(ContentManager Content, GraphicsDevice GraphicsDevice, SpriteBatch SpriteBatch)
        {

            if (Path != String.Empty)
                Texture = Content.Load<Texture2D>(Path);

            if (Texture != null)
            {
                dimensions.X += Texture.Width;
                dimensions.Y += Texture.Height;
            }

            if (SourceRect == Rectangle.Empty)
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            if (SourceRect == Rectangle.Empty)
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch.Begin();
            if (Texture != null)
                SpriteBatch.Draw(Texture, Vector2.Zero, Color.White);        
            SpriteBatch.End();

            origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            Radius = SourceRect.Width / 2;

        }      

        public void UnloadContent()
        {
            Texture.Dispose();
            if (renderTarget != null)
                renderTarget.Dispose();

        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);

            spriteBatch.Draw(Texture, Position + origin, SourceRect, color * Alpha, Rotation, origin, Scale, SpriteEffects.None, 0.0f);

        }

    }
}
