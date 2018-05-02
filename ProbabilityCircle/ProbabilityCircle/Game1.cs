using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace ProbabilityCircle
{
    public class Game1 : Game
    {
        MouseState mouseState;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteFont fontLarge;

        Image totalAreaSquare, lightBlueCircle, redCircle, yellowCircle, whiteCircle;
        List<Image> xMark = new List<Image>();

        double totalHitProbability, lightBlueHitProbability, redHitProbability, yellowHitProbability, whiteHitProbability = 0;

        Random hitPositionX = new Random(Guid.NewGuid().GetHashCode());
        Random hitPositionY = new Random(Guid.NewGuid().GetHashCode());

        int tempX = 0;
        int tempY = 0;
        float hitTicker = 0.25f;
        int totalScore = 0;

        // I failed this question on the test. After hearing back from James, I watched a quick video and saw the simple formula P(x) = success / total, 
        // and realised it meant success area is the area of the circle, where total is the area of the total square area. Very Simple!

        // I built this simple MonoGame solution to apply the skills from that question in a possible real world example.
        // Just wanted to use it as a learning point as well.

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            fontLarge = Content.Load<SpriteFont>("fontLarge");

            // The total area that is hittable. (800 px X 800 px)
            totalAreaSquare = new Image("800x800sq");
            totalAreaSquare.LoadContent(Content, GraphicsDevice, spriteBatch);
            totalAreaSquare.Position = new Vector2(100, 100);

            lightBlueCircle = new Image("700x700circle");
            lightBlueCircle.LoadContent(Content, GraphicsDevice, spriteBatch);
            lightBlueCircle.Position = totalAreaSquare.Position + totalAreaSquare.origin - lightBlueCircle.origin;

            redCircle = new Image("500x500circle");
            redCircle.LoadContent(Content, GraphicsDevice, spriteBatch);
            redCircle.Position = totalAreaSquare.Position + totalAreaSquare.origin - redCircle.origin;

            yellowCircle = new Image("300x300circle");
            yellowCircle.LoadContent(Content, GraphicsDevice, spriteBatch);
            yellowCircle.Position = totalAreaSquare.Position + totalAreaSquare.origin - yellowCircle.origin;

            whiteCircle = new Image("100x100circle");
            whiteCircle.LoadContent(Content, GraphicsDevice, spriteBatch);
            whiteCircle.Position = totalAreaSquare.Position + totalAreaSquare.origin - whiteCircle.origin;


            // area of circle divided by total area
            totalHitProbability = (lightBlueCircle.Radius * lightBlueCircle.Radius * Math.PI) / (totalAreaSquare.SourceRect.Width * totalAreaSquare.SourceRect.Height);

            // smallest circle works the same way.
            whiteHitProbability = (whiteCircle.Radius * whiteCircle.Radius * Math.PI) / (totalAreaSquare.SourceRect.Width * totalAreaSquare.SourceRect.Height);
            // additional circles work the same minus the probability of the inner circles.
            yellowHitProbability = (yellowCircle.Radius * yellowCircle.Radius * Math.PI) / (totalAreaSquare.SourceRect.Width * totalAreaSquare.SourceRect.Height) - whiteHitProbability;
            redHitProbability = (redCircle.Radius * redCircle.Radius * Math.PI) / (totalAreaSquare.SourceRect.Width * totalAreaSquare.SourceRect.Height) - yellowHitProbability - whiteHitProbability;
            lightBlueHitProbability = (lightBlueCircle.Radius * lightBlueCircle.Radius * Math.PI) / (totalAreaSquare.SourceRect.Width * totalAreaSquare.SourceRect.Height) - redHitProbability - yellowHitProbability - whiteHitProbability;
        }


        protected override void UnloadContent()
        {
            Content.Unload();
            totalAreaSquare.UnloadContent();
            lightBlueCircle.UnloadContent();
            redCircle.UnloadContent();
            yellowCircle.UnloadContent();
            whiteCircle.UnloadContent();

            if (xMark != null)
                for (int i = 0; i < xMark.Count; i++)
                    xMark[i].UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            if (hitTicker >= 0)
                hitTicker -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // probabilty of any hit is area of lightBlueCircle (largest circle) / area of totalAreaSquare.

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) )
                Exit();

            if ((Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Enter) || mouseState.LeftButton == ButtonState.Pressed) && hitTicker <= 0)
            {
                hitTicker = 0.25f;
                tempX = hitPositionX.Next((int)totalAreaSquare.Position.X, (int)totalAreaSquare.Position.X + totalAreaSquare.SourceRect.Width);
                tempY = hitPositionY.Next((int)totalAreaSquare.Position.Y, (int)totalAreaSquare.Position.Y + totalAreaSquare.SourceRect.Height);
                xMark.Add(new Image("xMark"));
                xMark[xMark.Count - 1].LoadContent(Content, GraphicsDevice, spriteBatch);
                xMark[xMark.Count - 1].Position = new Vector2(tempX, tempY) - xMark[xMark.Count - 1].origin;
                CalculatePoint(xMark[xMark.Count - 1].Position);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, null);
            
            totalAreaSquare.Draw(spriteBatch);
            lightBlueCircle.Draw(spriteBatch);
            redCircle.Draw(spriteBatch);
            yellowCircle.Draw(spriteBatch);
            whiteCircle.Draw(spriteBatch);

            spriteBatch.DrawString(fontLarge, "Probabilty Test. Press or Hold Space/Enter/LMB to set next marker.", new Vector2(10, 10), Color.Black);

            spriteBatch.DrawString(font, "Hittable Area: 800px X 800px", totalAreaSquare.Position + new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(font, "Probability of Hitting any: " + (totalHitProbability * 100).ToString("0.##") + "%", totalAreaSquare.Position + new Vector2(10, 40), Color.White);

            spriteBatch.DrawString(fontLarge, "10", new Vector2(lightBlueCircle.Position.X + lightBlueCircle.origin.X - 25, lightBlueCircle.Position.Y), Color.Black);
            // radius taken from image width.
            spriteBatch.DrawString(font, "R= " + lightBlueCircle.Radius + "px", new Vector2(lightBlueCircle.Position.X + lightBlueCircle.origin.X - 25, lightBlueCircle.Position.Y + 30), Color.Black);
            spriteBatch.DrawString(font, "Prob. Only Light Blue: " + (lightBlueHitProbability * 100).ToString("0.##") + "%", new Vector2(lightBlueCircle.Position.X + lightBlueCircle.origin.X - 150, lightBlueCircle.Position.Y + 50), Color.Black);

            spriteBatch.DrawString(fontLarge, "25", new Vector2(redCircle.Position.X + redCircle.origin.X - 25, redCircle.Position.Y), Color.Black);
            spriteBatch.DrawString(font, "R= " + redCircle.Radius + "px", new Vector2(redCircle.Position.X + redCircle.origin.X - 25, redCircle.Position.Y + 30), Color.Black);
            spriteBatch.DrawString(font, "Prob. Only Red: " + (redHitProbability * 100).ToString("0.##") + "%", new Vector2(redCircle.Position.X + redCircle.origin.X - 150, redCircle.Position.Y + 50), Color.Black);

            spriteBatch.DrawString(fontLarge, "50", new Vector2(yellowCircle.Position.X + yellowCircle.origin.X - 25, yellowCircle.Position.Y), Color.Black);
            spriteBatch.DrawString(font, "R= " + yellowCircle.Radius + "px", new Vector2(yellowCircle.Position.X + yellowCircle.origin.X - 25, yellowCircle.Position.Y + 30), Color.Black);
            spriteBatch.DrawString(font, "Prob. Only Yellow: " + (yellowHitProbability * 100).ToString("0.##") + "%", new Vector2(yellowCircle.Position.X + yellowCircle.origin.X - 150, yellowCircle.Position.Y + 50), Color.Black);

            spriteBatch.DrawString(fontLarge, "100", new Vector2(whiteCircle.Position.X + whiteCircle.origin.X - 30, whiteCircle.Position.Y + 10), Color.Black);
            spriteBatch.DrawString(font, "R= " + whiteCircle.Radius + "px", new Vector2(whiteCircle.Position.X + whiteCircle.origin.X - 25, whiteCircle.Position.Y + 40), Color.Black);
            spriteBatch.DrawString(font, "Prob. Only White: " + (whiteHitProbability * 100).ToString("0.##") + "%", new Vector2(whiteCircle.Position.X + whiteCircle.origin.X - 150, whiteCircle.Position.Y + 60), Color.Black);

            spriteBatch.DrawString(fontLarge, "Total Score: " + totalScore.ToString(), new Vector2(110, 910), Color.Black);
            

            if (xMark != null)
                for (int i = 0; i < xMark.Count; i++)
                    xMark[i].Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
            
        }

        public void CalculatePoint(Vector2 vectorPoint)
        {
            // all circles have same midPoint so can use just one.
            Vector2 midPoint = new Vector2(lightBlueCircle.Position.X + lightBlueCircle.origin.X, lightBlueCircle.Position.Y + lightBlueCircle.origin.Y); 

            // the formula that was needed for the test question.
            //(x-center_x)^2 + (y - center_y)^2 < radius^2
            double hitCalculate = ((vectorPoint.X - midPoint.X) * (vectorPoint.X - midPoint.X)) + ((vectorPoint.Y - midPoint.Y) * (vectorPoint.Y - midPoint.Y));
            if (hitCalculate < (whiteCircle.Radius * whiteCircle.Radius))
                totalScore += 100;
            else if (hitCalculate < (yellowCircle.Radius * yellowCircle.Radius))
                totalScore += 50;
            else if (hitCalculate < (redCircle.Radius * redCircle.Radius))
                totalScore += 25;
            else if (hitCalculate < (lightBlueCircle.Radius * lightBlueCircle.Radius))
                totalScore += 10;
        }
    }
}
