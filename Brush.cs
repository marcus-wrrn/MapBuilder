using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TileMap;

namespace Drawing {
    public class BrushException: Exception {
        public BrushException(string message): base(message) {}
    }// end BrushException

    public class Brush {
        public Texture2D PrimaryTexture{ get; set; }
        public Texture2D SecondaryTexture{ get; set; }
        private int brushSize;

        // Constructors
        public Brush(Texture2D prim, Texture2D second, int size) {
            if(size <= 0 && size <= 10)
                throw new BrushException("Size of brush must be between 1 and 10, got: " + size);
            PrimaryTexture = prim;
            SecondaryTexture = second;
            brushSize = size;
        }// end main constructor

        public Brush(Texture2D prim, Texture2D second) : this(prim, second, 1) {}
        public Brush(Texture2D prim) : this(prim, prim, 1) {}
        public Brush(Texture2D prim, int size) : this(prim, prim, size) {}

        public void IncreaseBrushSize() {
            if (brushSize < 10)
                brushSize++;
        }// end IncreaseBrushSize()

        public void DecreaseBrushSize() {
            if (brushSize > 1)
                brushSize--;
        }// end DecreaseBrushSize()

        public void DrawBrush(SpriteBatch spriteBatch, Background background, Vector2 mouseLocation) {
            // Find the tile location 
            int col = (int)(mouseLocation.X/background.BaseTile.Height);
            int row = (int)(mouseLocation.Y/background.BaseTile.Width);
            // Get Location
            Vector2 brushLocation = background.GetTile(row,col).Location;
            try {
                Rectangle sourceRectangle = new Rectangle((int)brushLocation.X, (int)brushLocation.Y, PrimaryTexture.Width, PrimaryTexture.Height);
                spriteBatch.Draw(PrimaryTexture, sourceRectangle, null, Color.White);
            } catch {
                Console.WriteLine("Tile Exeception source");
            }
        }
    }
}