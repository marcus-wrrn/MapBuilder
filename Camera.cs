using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
// Talk to Stacy Shane
// Talk to Dr. Chen
// 

namespace Visualization {

    public class Camera {
        private int _screenHeight;
        private int _screenWidth;
        private SnakeObjects.Snake _snake;
        private SnakeObjects.Fruit _fruit;
        private TileMap.Background _backgroundMap;
        public TileMap.TileRange CameraRange{ get; private set; }
        private Vector2 _mapOffset;

        public Camera(MapBuilder.Game1 game, Containers.SnakeContainer snakeContainer) {
            _screenHeight = game.GetPrefferedBufferHeight();
            _screenWidth = game.GetPrefferedBufferWidth();
            _snake = snakeContainer.PlayerSnake;
            _fruit = snakeContainer.SnakeFruit;

            _backgroundMap = snakeContainer.Map;
            _mapOffset = new Vector2(0,0);
            InitializeCamera();
            
        }// end Camera Constructor

        // Find CameraRange
        private void InitializeCamera() {
            // Calculate max number of tiles for height
            int maxTilesHeight = _screenHeight/_backgroundMap.BaseTile.Height + 1; // Adds 1 to make sure that no blank space is being shown
            int maxTilesWidth = _screenWidth/_backgroundMap.BaseTile.Width + 1;
            // Console.WriteLine(maxTilesHeight);
            // Console.WriteLine(maxTilesWidth);
            // Get SnakeHeadLocation
            var snakeLoc = _snake.GetSnakeHeadLocation();
            int snakeColumn = _backgroundMap.GetColumnNumber(snakeLoc);
            int snakeRow = _backgroundMap.GetRowNumber(snakeLoc);
            // Finds Camera Range
            CameraRange = new TileMap.TileRange(GetMinColumnOrRow(snakeRow, maxTilesHeight), GetMaxColumnOrRow(snakeRow, maxTilesHeight, _backgroundMap.Rows), 
                                                GetMinColumnOrRow(snakeColumn, maxTilesWidth), GetMaxColumnOrRow(snakeColumn, maxTilesWidth, _backgroundMap.Columns));
            InitializeOffset(snakeLoc);
        }// end InitializeCamera()

        private int GetMinColumnOrRow(int snakeTile, int maxTiles) {
            int minTile = snakeTile - maxTiles/2;
            if(minTile < 0)
                minTile = 0;
            return minTile;
        }// end getMinColumnOrRow()

        private int GetMaxColumnOrRow(int snakeTile, int maxTiles, int maxMap) {
            int maxTile = snakeTile + maxTiles/2;
            if(maxTile >= maxMap)
                maxTile = maxMap;
            return maxTile;
        }// end getMaxColumnOrRow()

        private void InitializeOffset(Vector2 snakeLoc) {
            var centreLoc = new Vector2(_screenWidth/2, _screenHeight/2);
            //_mapOffset = _backgroundMap.GetTile(CameraRange.StartX, CameraRange.StartY).Location;
            _mapOffset = centreLoc - snakeLoc;
        }


        // Camera functionality
        public void Update() {
            // Updates the camera
            InitializeCamera();
            TestCamera();
        }// end Update()

        private void TestCamera() {
            var snakeLoc = _snake.GetSnakeHeadLocation();
            int snakeColumn = _backgroundMap.GetColumnNumber(snakeLoc);
            int snakeRow = _backgroundMap.GetRowNumber(snakeLoc);

            Console.WriteLine("+++++++++ SNAKE TEST ++++++++++++");
            Console.Write("Snake Row test: ");
            if(snakeRow >= CameraRange.StartX || snakeRow <= CameraRange.EndX)
                Console.WriteLine("Passed");
            else
                Console.WriteLine("Failed");
            Console.WriteLine("StartX: " + CameraRange.StartX);
            Console.WriteLine("EndX: " + CameraRange.EndX);
            Console.WriteLine("Snake Row: " + snakeRow);
            Console.WriteLine("StartY: " + CameraRange.StartY);
            Console.WriteLine("EndY: " + CameraRange.EndY);
            Console.WriteLine("Snake Column: " + snakeColumn);

        }

        // Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            _backgroundMap.Draw(CameraRange, _mapOffset, spriteBatch);
            _snake.Draw(spriteBatch, _mapOffset);
            _fruit.Draw(spriteBatch, _mapOffset);
        }
        // Needs to draw tiles at offset locations
        // Offset is based off of player location
    }
}