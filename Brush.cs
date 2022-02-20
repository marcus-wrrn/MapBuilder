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
        public Assets.GameAsset PrimaryTexture{ get; set; }
        public Assets.GameAsset SecondaryTexture{ get; set; }
        private int brushSize;

        // Constructors
        public Brush(Texture2D prim, Texture2D second, int size) {
            if(size <= 0 && size <= 10)
                throw new BrushException("Size of brush must be between 1 and 10, got: " + size);
            PrimaryTexture = new Assets.GameAsset(prim);
            SecondaryTexture = new Assets.GameAsset(second);
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

        public void UpdateBrush(Background background, Vector2 mouseLocation) {
            int col = (int)(mouseLocation.X/background.BaseTile.Height);
            int row = (int)(mouseLocation.Y/background.BaseTile.Width);
            // If brush is out of bounds of the map don't update
            if(row >= background.Rows || row < 0 || col >= background.Columns || col < 0)
                return;
            // Get Location
            Vector2 brushLocation = background.GetTile(row,col).Location;
            if(brushLocation != null) {
                PrimaryTexture.Location = brushLocation;
                SecondaryTexture.Location = brushLocation;
            }
        }

        public void DrawBrush(SpriteBatch spriteBatch) {
            try {
                Rectangle sourceRectangle = new Rectangle((int)PrimaryTexture.Location.X, (int)PrimaryTexture.Location.Y, PrimaryTexture.Texture.Width, PrimaryTexture.Texture.Height);
                spriteBatch.Draw(PrimaryTexture.Texture, sourceRectangle, null, Color.White);
            } catch {
                Console.WriteLine("Tile Exeception source");
            }
        }
    }
}