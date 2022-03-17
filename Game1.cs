using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileMap;
using MenuSystem;

namespace MapBuilder
{
    public class Game1 : Game
    {   
        // Constants for the file names
        // TODO: Implement a file save + load system
        private string SNAKE_FILE_NAME = "test2";
        private string MAPBUILDER_FILE_NAME = "test2";
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public Containers.GameEditorContainer MapEditorContainer;
        public Containers.SnakeContainer SnakeContainer;
        //private string fileName = "test";
        private Controllers.MasterController controller;
        public bool IsNotGameOver = true;
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
            controller = new Controllers.MasterController(this);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Command Logic                
            controller.Update(gameTime);                
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);
            _spriteBatch.Begin();
            controller.Draw(gameTime, _spriteBatch);            
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void LoadMap() {
            MapEditorContainer = new Containers.GameEditorContainer(this, MAPBUILDER_FILE_NAME);
        }

        public void LoadSnake() {
            SnakeContainer = new Containers.SnakeContainer(this, SNAKE_FILE_NAME);
        }

        public int GetPrefferedBufferWidth() {
            return _graphics.PreferredBackBufferWidth;
        }

        public int GetPrefferedBufferHeight() {
            return _graphics.PreferredBackBufferHeight;
        }

    }// end Game1
}
