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

    public class MenuText {
        public bool IsSelected;
        public SpriteFont Font { get; private set; }
        public Color FontColor { get; private set; }
        public Color SelectedColor { get; private set; }
        public Vector2 Location { get; set; }
        public string Text { get; set; }

        public MenuText(SpriteFont font, Vector2 location, bool isSelected, Color color, Color selectedColor) {
            Font = font;
            IsSelected = isSelected;
            FontColor = color;
            SelectedColor = selectedColor;
            Location = location;
            Text = "";
        }
        public MenuText(SpriteFont font, Vector2 location) : this(font, location, false, Color.Black, Color.Tomato) {}

        public void Draw(SpriteBatch spriteBatch) {
            var color = IsSelected ? SelectedColor : FontColor;
            var text = IsSelected ? "> " + Text : Text;
            spriteBatch.DrawString(Font, text, Location, color);
        }
    }

    public class StartMenu {
        private enum Selected {SNAKE, MENU};
        private GameAsset _menuAsset;
        private MenuText _snakeText;
        private MenuText _mapBuildText;
        private Selected _selectedChoice;

        public StartMenu(GameAsset menuAsset, SpriteFont baseFont) {
            _menuAsset = menuAsset;
            _selectedChoice = Selected.SNAKE;
            InitializeText(baseFont);
        }// end constructor()

        private void InitializeText(SpriteFont font) {
            // Find location of the text
            var snakeTextLocation = new Vector2(_menuAsset.Location.X + _menuAsset.Texture.Width * 0.25f, _menuAsset.Location.Y + _menuAsset.Texture.Height * 0.25f);
            var menuTextLocation = new Vector2(snakeTextLocation.X, snakeTextLocation.Y + font.LineSpacing * 2f);
            // Initialize Font
            _snakeText = new MenuText(font, snakeTextLocation, true, Color.Black, Color.Crimson);
            _mapBuildText = new MenuText(font, menuTextLocation, false, Color.Black, Color.Crimson);
            // Sets text
            _snakeText.Text = "Snake";
            _mapBuildText.Text = "abcdefghijklmnopqrstuvwxyz";
        }

        public void Draw(SpriteBatch spriteBatch) {
            _menuAsset.Draw(spriteBatch);
            _snakeText.Draw(spriteBatch);
            _mapBuildText.Draw(spriteBatch);
        }
    }// end StartMenu

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
                SetTileData(tileSets);
            } catch(MenuException e) {
                Console.WriteLine(e);
            }
        }// end TilePickerMenu()

        public void ToggleVisibility() {
            if(visible)
                visible = false;
            else
                visible = true;
        }// end toggleVisibility()

        public bool IfMenuClicked(Vector2 mouse) {
            if(!visible)
                return false;
            if (mouse.X >= menuAsset.Location.X && mouse.X <= menuAsset.Location.X + menuAsset.Texture.Width)
                if (mouse.Y >= menuAsset.Location.Y && mouse.Y <= menuAsset.Location.Y + menuAsset.Texture.Height)
                    return true;
            return false;
        }// end ifMenuClicked()
        
        public void MoveUp(GameTime gameTime) {
            if(!visible)
                return;
            menuAsset.MoveUp(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveUp(gameTime);
            }
        }// end MoveUp()

        public void MoveDown(GameTime gameTime) {
            if(!visible)
                return;
            menuAsset.MoveDown(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveDown(gameTime);
            }
        }// end MoveDown()

        public void MoveLeft(GameTime gameTime) {
            if(!visible)
                return;
            menuAsset.MoveLeft(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveLeft(gameTime);
            }
        }// end MoveLeft()

        public void MoveRight(GameTime gameTime) {
            if(!visible)
                return;
            menuAsset.MoveRight(gameTime);
            foreach(GameAsset asset in tileData) {
                asset.MoveRight(gameTime);
            }
        }// end MoveRight()

        private void SetTileData(Texture2D[] tileSets) {
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
                    DrawTiles(spriteBatch);
                }
            } catch {
                Console.WriteLine("Console Did Not Load");
            }        
        }// end Draw()

        // Draws all the tiles into their place in menu
        private void DrawTiles(SpriteBatch spriteBatch) {
            for (int i = 0; i < tileData.Length; i++) {
                spriteBatch.Draw(tileData[i].Texture, tileData[i].Location, null, Color.White);
            }
        }// end DrawTiles()

        // Gets the texture data of the menu item when clicked returns null if not found
        public Microsoft.Xna.Framework.Graphics.Texture2D GetTileTexture(Vector2 mouseLocation) {
            if(!visible)
                return null;
            foreach(GameAsset tile in tileData) {
                if(mouseLocation.X <= tile.Location.X + tile.Texture.Width && mouseLocation.X >= tile.Location.X)
                    if(mouseLocation.Y <= tile.Location.Y + tile.Texture.Height && mouseLocation.Y >= tile.Location.Y)
                        return tile.Texture;
            }
            return null;
        }// end GetTileTexture()

        
    }
}