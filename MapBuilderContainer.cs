/* This File serves the purpose of containerizing all the important objects/variables to be used in the Game Editor */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Containers {
    public class GameEditorContainer {
        public TileMap.Background Map;
        public MenuSystem.TilePickerMenu TileMenu;
        public Drawing.Brush BrushTool;
        private string MapFileName;

        public GameEditorContainer(MapBuilder.Game1 game, string fileName) {
            MapFileName = fileName;
            Map = new TileMap.Background(fileName, game);
            BrushTool = new Drawing.Brush(game.Content.Load<Texture2D>("tile"), game.Content.Load<Texture2D>("tile2Test"));
            LoadMenu(game);
        }

        private void LoadMenu(MapBuilder.Game1 game) {
            // Initializing all textures to be loaded into the map
            Texture2D[] test = new Texture2D[8];
            for(int i = 0; i < test.Length; i++)
                test[i] = game.Content.Load<Texture2D>("tile2Test");
            test[0] = game.Content.Load<Texture2D>("tile");

            Texture2D menuTexture = game.Content.Load<Texture2D>("Menu");

            TileMenu = new MenuSystem.TilePickerMenu(test, 0, new Vector2(game.GetPrefferedBufferWidth() - menuTexture.Width, 0), menuTexture);
        }
        // Methods used to save the map
        public void SaveMap() {
            Map.ExportToBinary(MapFileName);
        }

        public void SaveMap(string fileName) {
            Map.ExportToBinary(fileName);
        }

        public void LoadNewMap(string fileName, MapBuilder.Game1 game) {
            try
            {
                Map = new TileMap.Background(fileName, game);
            }
            catch (System.Exception)
            {
                Console.WriteLine("File not found");
                throw;
            }
        }


    }// end GameEditorContainer

    public class SnakeContainer {
        public TileMap.Background Map;
        public SnakeObjects.Snake PlayerSnake;
        public SnakeObjects.Fruit SnakeFruit;
        public SpriteFont Font;
        private string SnakeFileName;
        public SnakeContainer(MapBuilder.Game1 game, string fileName) {
            SnakeFileName = fileName;
            Map = new TileMap.Background(fileName, game);
            PlayerSnake = new SnakeObjects.Snake(new SnakeObjects.SnakeTextures(game), Map);
            SnakeFruit = new SnakeObjects.Fruit(game.Content.Load<Texture2D>("Apple"), Map, PlayerSnake);
            Font = game.Content.Load<SpriteFont>("CustomFont");
        }
    }
}