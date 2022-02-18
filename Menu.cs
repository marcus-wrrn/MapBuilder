using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets;

namespace MenuSystem {

    public class MenuException: Exception {
        public MenuException(string message): base(message) {}
    }

    public class TilePickerMenu {
        public bool MenuClicked{ get; set; }
        private float menuSize;                     // size of the menu
        private GameAsset menuAsset;
        private GameAsset[] tileData;
        private const int tileSpacing = 3;
        private const int tileRowSpacing = 4;
        private const int tileLimit = 8;
        private bool visible;
        

        public TilePickerMenu(Texture2D[] tileSets, float scale, Vector2 loc, Texture2D menu) {
            try {
                menuAsset = new GameAsset(menu, loc, 1000.0f);
                menuSize = scale;
                MenuClicked = false;
                visible = true;
                setButtonData(tileSets);
            } catch(MenuException e) {
                Console.WriteLine(e);
            }
        }

        public void toggleVisibility() {
            if(visible)
                visible = false;
            else
                visible = true;
        }

        public bool ifMenuClicked(Vector2 mouse) {
            if(!visible)
                return false;
            if (mouse.X >= menuAsset.Location.X && mouse.X <= menuAsset.Location.X + menuAsset.Texture.Width)
                if (mouse.Y >= menuAsset.Location.Y && mouse.Y <= menuAsset.Location.Y + menuAsset.Texture.Height)
                    return true;
            return false;
        }

        
        public void MoveUp(GameTime gameTime) {
            menuAsset.MoveUp(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveUp(gameTime);
            }
        }// end MoveUp()

        public void MoveDown(GameTime gameTime) {
            menuAsset.MoveDown(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveDown(gameTime);
            }
        }// end MoveDown()

        public void MoveLeft(GameTime gameTime) {
            menuAsset.MoveLeft(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveLeft(gameTime);
            }
        }// end MoveLeft()

        public void MoveRight(GameTime gameTime) {
            menuAsset.MoveRight(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveRight(gameTime);
            }
        }// end MoveRight()

        // public void DragAndDrop(Vector2 mouse, Vector2 mouseStart, GameTime gameTime) {
        //     Vector2 location = Location;
        //     Console.WriteLine("Mouse: {0}, {0}", Location.X, Location.Y);
        //     location.X += 100f * (float)(mouse.X - mouseStart.X) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //     location.Y += 100f * (float)(mouse.Y - mouseStart.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //     Location = location;
        //     setButtonData();
        //     Console.WriteLine("Mouse Start: {0}, {0}", Location.X, Location.Y);
        // }

        private void moveButtons(Vector2 mouse, Vector2 mouseStart, GameTime gameTime) {
            Vector2 tempLocation = new Vector2(0,0);
            foreach(GameAsset butt in tileData) {
                tempLocation = butt.Location;
                tempLocation.X += 100f * (float)(mouse.X - mouseStart.X) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                tempLocation.Y += 100f * (float)(mouse.Y - mouseStart.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                butt.Location = tempLocation;
            }
        }

        //------------------------- Work In Progress -------------
        private void setButtonData(Texture2D[] tileSets) {
            // Check that the size of the tileSet does not exceed the menu
            if(tileSets.Length > 8 || tileSets.Length == 0) 
                throw new MenuException("Menu can only support up to " + tileLimit + " tiles");
            tileData = new GameAsset[tileSets.Length];
            Vector2 location = menuAsset.Location;
            Vector2 tile = new Vector2(tileSets[0].Width, tileSets[0].Height);
            for (int i = 0; i < tileData.Length; i++) {
                if ( i == 0 ) {
                    location.X += tile.X * (tileSpacing - 1);
                    location.Y += tile.Y * (tileSpacing - 1);
                }
                else if ( i % 2 == 1 ) {
                    location.X += tile.X * tileRowSpacing;
                }
                else {
                    location.X -= tile.X * tileRowSpacing;
                    location.Y += tile.Y * tileSpacing;
                }
                tileData[i] = new GameAsset(tileSets[i], location, menuAsset.Speed);
            }
        }

        // Draws the menu and all of its tiles
        public void Draw(SpriteBatch spriteBatch) {
            try {
                if (visible) {
                    // Draws the rectangle base
                    Rectangle baseRectangle = new Rectangle((int)menuAsset.Location.X, (int)menuAsset.Location.Y, menuAsset.Texture.Width, menuAsset.Texture.Height);
                    spriteBatch.Draw(menuAsset.Texture, baseRectangle, null, Color.White);
                    // Draws the tiles
                    drawTiles(spriteBatch);
                }
            } catch {
                Console.WriteLine("Console Did Not Load");
            }        
        }

        // Draws all the tiles into their place in menu
        private void drawTiles(SpriteBatch spriteBatch) {
            for (int i = 0; i < tileData.Length; i++) {
                spriteBatch.Draw(tileData[i].Texture, tileData[i].Location, null, Color.White);
            }
        }

        // Gets the texture data of the menu item when clicked returns null if not found
        public Microsoft.Xna.Framework.Graphics.Texture2D getTileData(Vector2 mouseLocation) {
            if(!visible)
                return null;
            foreach(GameAsset tile in tileData) {
                if(mouseLocation.X <= tile.Location.X + tile.Texture.Width && mouseLocation.X >= tile.Location.X)
                    if(mouseLocation.Y <= tile.Location.Y + tile.Texture.Height && mouseLocation.Y >= tile.Location.Y)
                        return tile.Texture;
            }
            return null;
        }

        
    }
}