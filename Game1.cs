﻿using Microsoft.Xna.Framework;
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
        public Background Map;
        public TilePickerMenu TileMenu;
        public Drawing.Brush Brush;
        public SnakeObjects.Fruit SnakeFruit;
        public SnakeObjects.Snake Snake;
        private SpriteFont Font;
        private string fileName = "test";
        private Controllers.MapControl controller;
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
            // Initializes controller
            controller = new Controllers.MapControl();
            // Initializing all textures to be loaded into the map
            Texture2D[] test = new Texture2D[8];
            for(int i = 0; i < test.Length; i++)
                test[i] = Content.Load<Texture2D>("tile2Test");
            test[0] = Content.Load<Texture2D>("tile");
            Texture2D menuTexture = Content.Load<Texture2D>("Menu");
            // Create Map
            Map = new Background(fileName, this);
            //Map = new Background(Content.Load<Texture2D>("tile"), new Vector2(50, 50), 30, 40);
            // background = new Background(currentTile, (int)(_graphics.PreferredBackBufferHeight / currentTile.Height), (int)(_graphics.PreferredBackBufferWidth / currentTile.Width));
            TileMenu = new TilePickerMenu(test, 0, new Vector2(_graphics.PreferredBackBufferWidth - menuTexture.Width, 0), menuTexture);
            Brush = new Drawing.Brush(Content.Load<Texture2D>("tile"), Content.Load<Texture2D>("tile2Test"));
            Snake = new SnakeObjects.Snake(Content.Load<Texture2D>("tile2Test"), Map);
            SnakeFruit = new SnakeObjects.Fruit(Content.Load<Texture2D>("tile2Test"), Map, Snake);
            Font = Content.Load<SpriteFont>("CustomFont");
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
                Brush.UpdateBrush(Map, mouse);
            
            controller.Update(this, gameTime);
            // If e was pressed export the map to a binary file
            if(Keyboard.GetState().IsKeyDown(Keys.E))
                Map.ExportToBinary(fileName);
            if(IsNotGameOver) {
                IsNotGameOver = Snake.PlaySnake(gameTime);
                SnakeFruit.Update();
            }
                
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);
            _spriteBatch.Begin();

            Map.Draw(_spriteBatch);
            //Brush.DrawBrush(_spriteBatch);
            TileMenu.Draw(_spriteBatch);
            Snake.Draw(_spriteBatch);
            SnakeFruit.Draw(_spriteBatch);

            // Draw Font
            _spriteBatch.DrawString(Font, "Score: " + (Snake.GetLength() - 1), new Vector2(3000, 500), Color.Black);
            
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void ResetGame() {
            Snake.ResetSnake();
            SnakeFruit.GenerateLocation();
            IsNotGameOver = true;
        }
    }
}
