﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MenuSystem;

namespace MapBuilder
{
    public class Game1 : Game
    {   
        // Constants for the file names
        // TODO: Implement a file save + load system
        private string _snakeFile = "Testing";
        private string _mapBuilderFile = "test2";
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Containers.GameEditorContainer MapEditorContainer;
        public Containers.SnakeContainer SnakeContainer;
        //private string fileName = "test";
        private Controllers.MasterController _controller;
        public MenuSystem.StartMenu _startMenu;
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
            _controller = new Controllers.MasterController(this);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Command Logic                
            _controller.Update(gameTime);                
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _controller.Draw(gameTime, _spriteBatch);     
            //_startMenu.Draw(_spriteBatch);       
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void LoadMap() {
            MapEditorContainer = new Containers.GameEditorContainer(this, _mapBuilderFile);
        }

        public void LoadNewMap(string fileName, int rows, int columns) {
            _mapBuilderFile = fileName;
            MapEditorContainer = new Containers.GameEditorContainer(this, _mapBuilderFile, rows, columns);
        }

        public void LoadSnake() {
            SnakeContainer = new Containers.SnakeContainer(this, _snakeFile);
        }

        public void LoadStartMenu() {
            var startMenuAsset = new Assets.GameAsset(Content.Load<Texture2D>("TestBox"), new Vector2(_graphics.PreferredBackBufferWidth/3, _graphics.PreferredBackBufferHeight/3));
            _startMenu = new StartMenu(startMenuAsset, Content.Load<SpriteFont>("CustomFont"));
        }

        public int GetPrefferedBufferWidth() {
            return _graphics.PreferredBackBufferWidth;
        }

        public int GetPrefferedBufferHeight() {
            return _graphics.PreferredBackBufferHeight;
        }

    }// end Game1
}
