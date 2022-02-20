using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileMap;
using MenuSystem;

namespace MapBuilder
{
    public class Game1 : Game
    {   
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public Background background;
        public TilePickerMenu TileMenu;
        public Drawing.Brush brush;
        private Controller.GameContainer gameContainer;
        private string fileName = "test";
        private Controller.GameControl controller;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 3840;
            _graphics.PreferredBackBufferHeight = 2160;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        { 
            base.Initialize();
            controller = new Controller.GameControl();
            Texture2D[] test = new Texture2D[8];
            for(int i = 0; i < test.Length; i++)
                test[i] = Content.Load<Texture2D>("tile2Test");
            test[0] = Content.Load<Texture2D>("tile");
            Texture2D menuTexture = Content.Load<Texture2D>("Menu");
            Background background = new Background(fileName, this);
            // background = new Background(currentTile, (int)(_graphics.PreferredBackBufferHeight / currentTile.Height), (int)(_graphics.PreferredBackBufferWidth / currentTile.Width));
            MenuSystem.TilePickerMenu menu = new TilePickerMenu(test, 0, new Vector2(_graphics.PreferredBackBufferWidth - menuTexture.Width, 0), menuTexture);
            Drawing.Brush brush = new Drawing.Brush(Content.Load<Texture2D>("tile"), Content.Load<Texture2D>("tile2Test"));
            gameContainer = new Controller.GameContainer(menu, background, brush);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Update Containers
            // Update Controllers
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // Command Logic                
            controller.CommandInputs(this, gameTime);
            // Mouse Logic
            // Finds the position of the mouse
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (mouse.X <= _graphics.PreferredBackBufferWidth && mouse.Y <= _graphics.PreferredBackBufferHeight)
                gameContainer.Brush.UpdateBrush(gameContainer.Map, mouse);
            
            var inputContainer = new Controller.InputContainer(Mouse.GetState(), Keyboard.GetState(), gameTime);
            controller.Update(this, gameTime, gameContainer, inputContainer);
            // If e was pressed export the map to a binary file
            if(Keyboard.GetState().IsKeyDown(Keys.E))
                gameContainer.Map.ExportToBinary(fileName);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            gameContainer.Map.Draw(_spriteBatch);
            gameContainer.Brush.DrawBrush(_spriteBatch);
            gameContainer.TileMenu.Draw(_spriteBatch);

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        
    }
}
