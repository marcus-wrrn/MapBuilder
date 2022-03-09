using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace Controllers {
    public class Button {
            public Keys Key{ get; set; }
            public Keys SecondaryKey{ get; set; }
            public bool IsPressed{ get; set; }
            public Enum effectName{get;}

            public Button(Keys key, Keys key2, Enum name) {
                Key = key;
                SecondaryKey = key2;
                IsPressed = false;
                effectName = name;
            }

            public Button(Keys key, Enum name) {
                Key = key;
                SecondaryKey = Keys.None;
                IsPressed = false;
                effectName = name;
            }
    }// end Button

    public interface ControllerInterface {
        void Update(GameTime gameTime);
        void CommandInputs(GameTime gameTime);
        void Draw(GameTime gameTime);
    }

    public class BaseController {
        // to be used later to swap between different modes for the map editor
        protected Button[] commandKeys;               // All Key commands with the shift 
 

        // 
        protected void UseCommand(KeyboardState kState, Button butt, Action action) {
            // If the secondary key does not exist or the secondary button is pressed
            if (butt.SecondaryKey == Keys.None || kState.IsKeyDown(butt.SecondaryKey)) {
                if (kState.IsKeyDown(butt.Key))
                    if (!butt.IsPressed) {
                        action();
                        butt.IsPressed = true;
                    }
                if (kState.IsKeyUp(butt.Key))
                    butt.IsPressed = false;
            }
        }// end UseCommand()
    }

    public class MapControl : BaseController {
        private enum MenuCommands { MENU_VISIBILITY, MOVE_MENU_MODE_SWITCH, DRAW_MODE_SWITCH, 
        BUCKET_MODE_SWITCH, MOVE_UP, MOVE_DOWN, MOVE_LEFT, 
        MOVE_RIGHT, UPDATE_TILE_PRIMARY, UPDATE_TILE_SECONDARY,
        RESET_SNAKE }

        protected enum ControllerMode { MOVE_MENU, DRAW, BUCKET_FILL }

        private MenuSystem.TilePickerMenu TileMenu;
        private Drawing.Brush Brush;
        private TileMap.Background Map;
        private ControllerMode mode;                // The mode that the controller is currently in

        public MapControl(MenuSystem.TilePickerMenu tMenu, Drawing.Brush brsh) {
            TileMenu = tMenu;
            Brush  = brsh;
            commandKeys = new Button[] {
                // Toggles Menu Visibility
                new Button(Keys.V, Keys.LeftShift, MenuCommands.MENU_VISIBILITY),
                // Toggles to Move Menu Mode
                new Button(Keys.M, Keys.LeftShift, MenuCommands.MOVE_MENU_MODE_SWITCH),
                // Toggles to Bucket Fill Mode
                new Button(Keys.G, MenuCommands.BUCKET_MODE_SWITCH),
                new Button(Keys.R, Keys.LeftShift, MenuCommands.RESET_SNAKE),
                // Movement commands
                new Button(Keys.Up, MenuCommands.MOVE_UP),
                new Button(Keys.Down, MenuCommands.MOVE_DOWN),
                new Button(Keys.Left, MenuCommands.MOVE_LEFT),
                new Button(Keys.Right, MenuCommands.MOVE_RIGHT)
            };
            

            mode = ControllerMode.DRAW;
        }// end GameControl

        // Checks keys to check if any MenuEffects should be triggered
        private void CommandInputs(GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case MenuCommands.MENU_VISIBILITY:
                        // Toggles menu visibility
                        UseCommand(kState, butt, () => TileMenu.ToggleVisibility());
                        break;
                    // Swap to move menu mode
                    case MenuCommands.MOVE_MENU_MODE_SWITCH:
                        // Toggles to move menu mode
                        if (mode == ControllerMode.DRAW)
                            UseCommand(kState, butt, () => mode = ControllerMode.MOVE_MENU);
                        else if(mode == ControllerMode.MOVE_MENU)
                            UseCommand(kState, butt, () => mode = ControllerMode.DRAW);
                        break;
                    case MenuCommands.BUCKET_MODE_SWITCH:
                        if (mode == ControllerMode.DRAW)
                            UseCommand(kState, butt, () => mode = ControllerMode.BUCKET_FILL);
                        else if(mode == ControllerMode.BUCKET_FILL)
                            UseCommand(kState, butt, () => mode = ControllerMode.DRAW);
                        break;
                    case MenuCommands.MOVE_UP:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            TileMenu.MoveUp(gameTime);
                        break;
                    case MenuCommands.MOVE_DOWN:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            TileMenu.MoveDown(gameTime);
                        break;
                    case MenuCommands.MOVE_RIGHT:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            TileMenu.MoveRight(gameTime);
                        break;
                    case MenuCommands.MOVE_LEFT:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            TileMenu.MoveLeft(gameTime);
                        break;
                    default:
                        break;
                }
            }// end foreach loop
        }// end MenuEffects()

        // Controls how the mouse inputs effect the Brushes Drawing ability
        private void BrushInputManager() {
            var mouseState = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton == ButtonState.Pressed) {
                // Set Show Secondary to False
                Brush.ShowSecondary = false;
                BrushInputHelper(mouseLoc);
            }
            else if (mouseState.RightButton == ButtonState.Pressed) {
                // Set Brush to display secondary Texture in its view
                Brush.ShowSecondary = true;
                BrushInputHelper(mouseLoc);
            }
            else
                Brush.ShowSecondary = false;
        }// end MouseEffects()

        // Helps BrushInputManager by Making sure that the map is being updated Correctly
        private void BrushInputHelper(Vector2 mouseLoc) {
            // Gets the current texture
            Texture2D texture = Brush.GetCurrentTile().Texture;
            // Check to see if it clicked on a menu icon
            Texture2D tempTile = TileMenu.GetTileTexture(mouseLoc);
            if (tempTile != null) {
                Brush.ChangeTexture(tempTile);
            }
            else if (TileMenu.IfMenuClicked(mouseLoc)) {
                if(!TileMenu.MenuClicked)
                    TileMenu.MenuClicked = true;
            }

            // If the menu icon wasn't clicked update the tile below it
            else {
                if (mode == ControllerMode.DRAW)
                    Map.UpdateTile(texture, mouseLoc);
                else if(mode == ControllerMode.BUCKET_FILL)
                    Brush.BucketFill(Map, mouseLoc, texture);
                TileMenu.MenuClicked = false;
            }
        }
        public void Update(GameTime gameTime) {
            CommandInputs(gameTime);
            BrushInputManager();
        }

        public void Draw(SpriteBatch spriteBatch) {
            Map.Draw(spriteBatch);
            TileMenu.Draw(spriteBatch);
            Brush.DrawBrush(spriteBatch);
        }
            
    }// end MapControl

    public class SnakeController : BaseController {
        protected enum ControllerMode { PLAY_GAME, GAME_OVER }
        private enum SnakeCommands { MOVE_UP, MOVE_DOWN, MOVE_LEFT, MOVE_RIGHT, RESET_SNAKE }
        private SnakeObjects.Snake Snake;
        private SnakeObjects.Fruit SnakeFruit;
        private SpriteFont ScoreFont;
        private ControllerMode Mode;

        SnakeController(SnakeObjects.Snake snek, SnakeObjects.Fruit snekFruit, SpriteFont scoreFont) {
            Snake = snek;
            Mode = ControllerMode.PLAY_GAME;
            ScoreFont = scoreFont;
            // Initiallize the key Commands
            commandKeys = new Button[] {
                // Toggles Menu Visibility
                new Button(Keys.Up, SnakeCommands.MOVE_UP),
                new Button(Keys.Down, SnakeCommands.MOVE_DOWN),
                new Button(Keys.Right, SnakeCommands.MOVE_RIGHT),
                new Button(Keys.Left, SnakeCommands.MOVE_LEFT),
                new Button(Keys.R, Keys.LeftShift, SnakeCommands.RESET_SNAKE)
            };
        }
        private void CommandInputs() {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case SnakeCommands.MOVE_UP:
                        if (kState.IsKeyDown(butt.Key))
                            Snake.MoveUp();
                        break;
                    case SnakeCommands.MOVE_DOWN:
                        if (kState.IsKeyDown(butt.Key))
                            Snake.MoveDown();
                        break;
                    case SnakeCommands.MOVE_LEFT:
                        if (kState.IsKeyDown(butt.Key))
                            Snake.MoveLeft();
                        break;
                    case SnakeCommands.MOVE_RIGHT:
                        if (kState.IsKeyDown(butt.Key))
                            Snake.MoveRight();
                        break;
                    case SnakeCommands.RESET_SNAKE:
                        if (kState.IsKeyDown(butt.Key)) {
                            Snake.ResetSnake();
                            Mode = ControllerMode.PLAY_GAME;
                        }
                        break;
                    default:
                        break;
                }
            }// end foreach loop
        }// end CommandInputs()

        public void Update(GameTime gameTime) {
            // Call the input commands
            CommandInputs();
            // Update game
            if(Mode == ControllerMode.PLAY_GAME) {
                if(Snake.PlaySnake(gameTime))
                    SnakeFruit.Update();
                else
                    Mode = ControllerMode.GAME_OVER; 
            }
        }// end Update

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            Snake.Draw(spriteBatch);
            SnakeFruit.Draw(spriteBatch);
            spriteBatch.DrawString(ScoreFont, "Score " + (Snake.GetLength() - 1), new Vector2(3000, 500), Color.Black);
        }
    }// end SnakeController

    public class MasterController : BaseController {
        
    }
}