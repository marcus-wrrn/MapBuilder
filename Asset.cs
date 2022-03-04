using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assets {

    // Base class used for all Assets
    public class GameAsset {
        public Texture2D Texture{ get; set; }   // texture of the asset
        public Vector2 Location{ get; set; }    // location of the asset
        public float Speed{ get; set; }

        public GameAsset(Texture2D text, Vector2 loc, float speed) {
            Texture = text;
            Location = loc;
            Speed = speed;
        }// end constructor()

        // Additional Constructors
        public GameAsset(Texture2D text, GraphicsDeviceManager graphics, float speed) : this(text, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), speed) {}
        public GameAsset(Texture2D text, GraphicsDeviceManager graphics) : this(text, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), 0.0f) {}
        public GameAsset(Texture2D text) : this(text, new Vector2(0, 0), 0.0f) {}
        public GameAsset(Texture2D text, Vector2 loc) : this(text, loc, 0.0f) {}
        public GameAsset(Texture2D text, float speed) : this(text, new Vector2(0,0), speed) {}

        public void MoveUp(GameTime gameTime) {
            // I hate the fact I have to do this and there must be an easier way than creating a new variable
            Vector2 location = Location;
            location.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Location = location;
        }// end MoveUp()

        public void MoveDown(GameTime gameTime) {
            // I hate the fact I have to do this and there must be an easier way than creating a new variable
            Vector2 location = Location;
            location.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Location = location;
        }// end MoveDown()

        public void MoveRight(GameTime gameTime) {
            // I hate the fact I have to do this and there must be an easier way than creating a new variable
            Vector2 location = Location;
            location.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Location = location;
        }// end MoveRight()

        public void MoveLeft(GameTime gameTime) {
            // I hate the fact I have to do this and there must be an easier way than creating a new variable
            Vector2 location = Location;
            location.X -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Location = location;
        }// end MoveLeft()

        public void Draw(SpriteBatch spriteBatch) {
            Rectangle sourceRectangle = new Rectangle((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
            spriteBatch.Draw(Texture, sourceRectangle, null, Color.White);
        }// Draws the texture
    }
}