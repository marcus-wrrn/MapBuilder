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
        private enum Direction { UP, DOWN, LEFT, RIGHT }                        // Used for the bucket fill command helps make the algorithm more efficient
        private int brushSize;
        private Assets.GameAsset primaryTexture;
        private Assets.GameAsset secondaryTexture;
        public bool ShowSecondary;

        // Constructors
        public Brush(Texture2D prim, Texture2D second, int size) {
            if(size <= 0 && size <= 10)
                throw new BrushException("Size of brush must be between 1 and 10, got: " + size);
            primaryTexture = new Assets.GameAsset(prim);
            secondaryTexture = new Assets.GameAsset(second);
            brushSize = size;
            ShowSecondary = false;
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
                primaryTexture.Location = brushLocation;
                secondaryTexture.Location = brushLocation;
            }
        }


        public void ChangeTexture(Texture2D texture) {
            if(ShowSecondary)
                secondaryTexture.Texture = texture;
            else
                primaryTexture.Texture = texture;
        }
        
        public Assets.GameAsset GetCurrentTile() {
            if(ShowSecondary)
                return secondaryTexture;
            return primaryTexture;
        }


        public void BucketFill(Background map, Vector2 location, Texture2D texture) {
            // Find Row and Column
            int row = map.GetRowNumber(location);
            int col = map.GetColumnNumber(location);
            FillInBucket(map, row, col, texture);
        }
        private void FillInBucket(Background map, int row, int col, Texture2D texture) {
            FillBucketUp(map, row, col, texture);
            FillBucketRight(map, row, col, texture);
            FillBucketDown(map, row, col, texture);
            FillBucketLeft(map, row, col, texture);
        }// end FillInBucket

        private void FillBucketLeft(Background map, int row, int col, Texture2D texture) {
            col--;
            if(col >= map.Columns || col < 0)
                return;
            var tile = map.GetTile(row, col);
            if(tile.Texture == texture)
                return;
            tile.Texture = texture;
            FillInBucket(map, row, col, texture);
        }// end FillBucketLeft()

        private void FillBucketRight(Background map, int row, int col, Texture2D texture) {
            col++;
            if(col >= map.Columns || col < 0)
                return;
            var tile = map.GetTile(row, col);
            if(tile.Texture == texture)
                return;
            tile.Texture = texture;
            FillInBucket(map, row, col, texture);
        }// end FillBucketRight()

        private void FillBucketUp(Background map, int row, int col, Texture2D texture) {
            row--;
            if(row >= map.Rows || row < 0)
                return;
            var tile = map.GetTile(row, col);
            if(tile.Texture == texture)
                return;
            tile.Texture = texture;
            FillInBucket(map, row, col, texture);
        }// end FillBucketUp()

        private void FillBucketDown(Background map, int row, int col, Texture2D texture) {
            row++;
            if(row >= map.Rows || row < 0)
                return;
            var tile = map.GetTile(row, col);
            if(tile.Texture == texture)
                return;
            tile.Texture = texture;
            FillInBucket(map, row, col, texture);
        }// end FillBucketDown()


        public void DrawBrush(SpriteBatch spriteBatch) {
            try {
                if(!ShowSecondary) {
                    Rectangle sourceRectangle = new Rectangle((int)primaryTexture.Location.X, (int)primaryTexture.Location.Y, primaryTexture.Texture.Width, primaryTexture.Texture.Height);
                    spriteBatch.Draw(primaryTexture.Texture, sourceRectangle, null, Color.White);
                }
                else {
                    Rectangle sourceRectangle = new Rectangle((int)secondaryTexture.Location.X, (int)secondaryTexture.Location.Y, secondaryTexture.Texture.Width, secondaryTexture.Texture.Height);
                    spriteBatch.Draw(secondaryTexture.Texture, sourceRectangle, null, Color.White);
                }
            } catch {
                Console.WriteLine("Tile Exeception source");
            }
        }
    }
}