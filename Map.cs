using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TileMap {
    class Tile {
        public Texture2D Texture{ get; set; }
        public string Name { get; set; }

        public Tile(Texture2D texture, string name) {
            Texture = texture;
            Name = name;
        }// end constructor

        public Tile(string name, Game game) {
            try {
                Texture = game.Content.Load<Texture2D>(name);
            } catch {
                Console.WriteLine("Tile not found");
            }
        }// end constructor

    }
    public class Background {
        private Texture2D[,] map;                   // Map of all tiles
        public int Rows { get; set; }               // Number of rows in the map 
        public int Columns { get; set; }            // Number of columns in the map
        public Texture2D BaseTile { get; set; }     // Reference texture

        // Constructor to build the map from scratch
        public Background(Texture2D baseTile, int rows, int columns) {
            // Initialize variables
            BaseTile = baseTile;
            Rows = rows;
            Columns = columns;
            map = new Texture2D[rows, columns];
            Console.WriteLine("Rows: {0}\nColomns: {0}", rows, columns);
            // Create background
            generateMap();
        }// end Constructor

        private void generateMap() {
            for (int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    map[i,j] = BaseTile;
                }
            }
        }// end generateMap

        // Constructor to build a map from a file
        public Background(string fileName, Game game) {
            try {
                BinaryReader binReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
                Rows = binReader.ReadInt32();
                Columns = binReader.ReadInt32();
                // Load in the baseTile
                BaseTile = game.Content.Load<Texture2D>(binReader.ReadString());
                // Load in Map
                map = new Texture2D[Rows, Columns];
                for(int i = 0; i < Rows; i++) {
                    for(int j = 0; j < Columns; j++) { 
                        map[i,j] = game.Content.Load<Texture2D>(binReader.ReadString());
                    }
                }
            } catch {
                    Console.WriteLine("Failed to load map");
            }
        }
        public void Draw(SpriteBatch spriteBatch) {
            Vector2 location = new Vector2(0,0);

            for(int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    // Draw the sprite
                    DrawTile(spriteBatch, map[i,j], location);
                    // Change location
                    location.X += BaseTile.Width;
                }
                location.Y += BaseTile.Height;
                location.X = 0;
            }
        }// end Draw()

        private void DrawTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 location) {
            try {
                Rectangle sourceRectangle = new Rectangle((int)location.X, (int)location.Y, BaseTile.Width, BaseTile.Height);
                spriteBatch.Draw(texture, sourceRectangle, null, Color.White);
            } catch {
                Console.WriteLine("Tile Exeception source");
            }
        }// end DrawTile()

        public void updateTile(Texture2D tile, Vector2 location) {
            try {
                // Find the tile location 
                int col = (int)(location.X/BaseTile.Height);
                int row = (int)(location.Y/BaseTile.Width);
                // Replace tile
                map[row,col] = tile;
            }  catch {
                
            }
        }// end updateTile()

        public void exportToBinary(string fileName) {
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
                            binWriter.Write(map[i,j].Name);
                            // If the texture is not on the list of textures add it
                            if (textureList.Contains(map[i,j].Name))
                                textureList.Append(map[i,j].Name);
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