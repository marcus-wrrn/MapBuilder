using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Assets;

namespace TileMap {

    // Provides a range of tiles
    public class SquareRange {
        public int StartX{ get; set; }
        public int StartY{ get; set; }
        public int EndX{ get; set; }
        public int EndY{ get; set; }

        public SquareRange(int startX, int endX, int startY, int endY) {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
        }
    }// end TileRange

    public class BackgroundException : Exception {
        public BackgroundException(string message): base(message) {}
    }// end BackgroundException

    public class Background {
        private GameAsset[,] map;                   // Map of all tiles
        public int Rows { get; set; }               // Number of rows in the map 
        public int Columns { get; set; }            // Number of columns in the map
        public Texture2D BaseTile { get; set; }     // Reference texture
        public SquareRange Boundries { get; private set; }

        private Vector2 OffSet;

        // Constructor to build the map from scratch
        public Background(Texture2D baseTile, Vector2 offSet, int rows, int columns) {
            OffSet = offSet;
            // Initialize variables
            BaseTile = baseTile;
            Rows = rows;
            Columns = columns;
            map = new GameAsset[rows, columns];
            // Create background
            GenerateMap();
        }// end Constructor

        public Background(Texture2D baseTile, int rows, int columns) : this(baseTile, new Vector2(0f, 0f), rows, columns){}

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

        // Constructor to build a map from a file
        public Background(string fileName, Game game) {
            try {
                // Read the binary file
                BinaryReader binReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
                Rows = binReader.ReadInt32();
                Columns = binReader.ReadInt32();
                // Get the Offset
                OffSet = new Vector2();
                OffSet.X = (float)binReader.ReadDouble();
                OffSet.Y = (float)binReader.ReadDouble();
                // Load in the baseTile
                BaseTile = game.Content.Load<Texture2D>(binReader.ReadString());
                // Load in Map
                map = new GameAsset[Rows, Columns];
                GenerateMapFromFile(binReader, game);
            } catch {
                    Console.WriteLine("Failed to load map");
            }
        }// end constructor from file

        private void GenerateMapFromFile(BinaryReader binaryReader, Game game) {
            Vector2 location = new Vector2(OffSet.X,OffSet.Y);
            for (int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    map[i,j] = new GameAsset(game.Content.Load<Texture2D>(binaryReader.ReadString()), location);
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

        public void UpdateTile(Texture2D tile, Vector2 location) {
            try {
                // Find the tile location 
                int col = GetColumnNumber(location);
                int row = GetRowNumber(location);
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
                    // Record the OffSet of the map
                    binWriter.Write((double)OffSet.X);
                    binWriter.Write((double)OffSet.Y);
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
        }//  end ExportToBinary()


        public void Draw(SpriteBatch spriteBatch) {
            for(int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    // Draw the sprite
                    DrawTile(spriteBatch, map[i,j]);
                }
            }
        }// end Draw()

        public void Draw(SquareRange range, Vector2 offset, SpriteBatch spriteBatch) {
            try {
                for(int i = range.StartX; i < range.EndX; i++) {
                    for(int j = range.StartY; j < range.EndY; j++) {
                        DrawTile(spriteBatch, map[i,j], offset);
                    }
                }
            } catch {
                Console.WriteLine("Tile out of range");
                Console.WriteLine("StartX: " + range.StartX);
                Console.WriteLine("EndX: " + range.EndX);
                Console.WriteLine("StartY: " + range.StartY);
                Console.WriteLine("EndY: " + range.EndY);
                Console.WriteLine("Rows: " + Rows);
                Console.WriteLine("Columns: " + Columns);
            }
        }


        private void DrawTile(SpriteBatch spriteBatch, GameAsset tile) {
            DrawTile(spriteBatch, tile, new Vector2(0,0));
        }// end DrawTile()

        // Draws a tile with an offset
        private void DrawTile(SpriteBatch spriteBatch, GameAsset tile, Vector2 offset) {
            try {
                tile.Draw(spriteBatch, offset);
            } catch {
                Console.WriteLine("Tile Exeception source");
            }
        }// end DrawTile()
        
    }// end Background
}// end namespace TileMap