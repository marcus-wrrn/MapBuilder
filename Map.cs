using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Assets;

namespace TileMap {

    public class BackgroundException : Exception {
        public BackgroundException(string message): base(message) {}
    }// end BackgroundException

    public class Background {
        private GameAsset[,] map;                   // Map of all tiles
        public int Rows { get; set; }               // Number of rows in the map 
        public int Columns { get; set; }            // Number of columns in the map
        public Texture2D BaseTile { get; set; }     // Reference texture

        private Vector2 OffSet;

        // Constructor to build the map from scratch
        public Background(Texture2D baseTile, int rows, int columns) {
            OffSet = new Vector2(200, 60);
            // Initialize variables
            BaseTile = baseTile;
            Rows = rows;
            Columns = columns;
            map = new GameAsset[rows, columns];
            // Create background
            GenerateMap();
        }// end Constructor

        // Constructor to build a map from a file
        public Background(string fileName, Game game) {
            try {
                BinaryReader binReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
                Rows = binReader.ReadInt32();
                Columns = binReader.ReadInt32();
                // Load in the baseTile
                BaseTile = game.Content.Load<Texture2D>(binReader.ReadString());
                // Load in Map
                map = new GameAsset[Rows, Columns];
                Vector2 location = new Vector2(0,0);
                for(int i = 0; i < Rows; i++) {
                    for(int j = 0; j < Columns; j++) { 
                        map[i,j] = new GameAsset(game.Content.Load<Texture2D>(binReader.ReadString()), location);
                        location.X += BaseTile.Width;
                    }
                    location.Y += BaseTile.Height;
                    location.X = 0;
                }
            } catch {
                    Console.WriteLine("Failed to load map");
            }
        }// end constructor from file

        private void GenerateMap() {
            Vector2 location = new Vector2(OffSet.X,OffSet.Y);
            for (int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    map[i,j] = new GameAsset(BaseTile, location);
                    location.X += BaseTile.Width;
                }
                location.Y += BaseTile.Height;
                location.X = OffSet.X;
            }
        }// end GenerateMap()


        public int GetColumnNumber(Vector2 location) {
            return (int)((location.X - OffSet.X)/BaseTile.Height);
        }

        public int GetRowNumber(Vector2 location) {
            return (int)((location.Y - OffSet.Y)/BaseTile.Width);
        }


        public void Draw(SpriteBatch spriteBatch) {
            for(int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    // Draw the sprite
                    DrawTile(spriteBatch, map[i,j]);
                }
            }
        }// end Draw()

        private void DrawTile(SpriteBatch spriteBatch, GameAsset tile) {
            try {
                Rectangle sourceRectangle = new Rectangle((int)tile.Location.X, (int)tile.Location.Y, BaseTile.Width, BaseTile.Height);
                spriteBatch.Draw(tile.Texture, sourceRectangle, null, Color.White);
            } catch {
                Console.WriteLine("Tile Exeception source");
            }
        }// end DrawTile()

        public void UpdateTile(Texture2D tile, Vector2 location) {
            try {
                // Find the tile location 
                int col = (int)((location.X)/BaseTile.Height);
                int row = (int)((location.Y)/BaseTile.Width + OffSet.X);
                // Replace tile
                map[row,col].Texture = tile;
            }  catch {
                Console.WriteLine("Divide by zero error");
            }
        }// end updateTile()

        

        public bool IsOnMap(int row, int col) {
            return row < map.GetLength(0) && row >= 0 && col < map.GetLength(1) && col >= 0;
        }// end IsOnMap()

        public GameAsset GetTile(int row, int col) {
            if(row >= map.GetLength(0) || row < 0 || col >= map.GetLength(1) || col < 0)
                throw new BackgroundException("Values out of bound\nRow: " + row + "\nCol: " + col);
            return map[row,col];
        }// end GetTile()

        public void ExportToBinary(string fileName) {
            // Create Binary file                 
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create))) {
                try {
                    // Records total number of elements in the map
                    binWriter.Write(Rows);
                    binWriter.Write(Columns);
                    // Creates a list of all tiles used for the map
                    string[] textureList = new string[0];
                    binWriter.Write(BaseTile.Name);
                    for(int i = 0; i < Rows; i++)
                        for(int j = 0; j < Columns; j++) {
                            binWriter.Write(map[i,j].Texture.Name);
                            // If the texture is not on the list of textures add it
                            if (textureList.Contains(map[i,j].Texture.Name))
                                textureList.Append(map[i,j].Texture.Name);
                        }
                    foreach(string name in textureList)
                        binWriter.Write(name);
                    binWriter.Close();
                } catch (IOException ioexp) {
                    Console.WriteLine("Error: {0}", ioexp.Message);
                }
            }
        }
    }// end Background
}// end namespace TileMap