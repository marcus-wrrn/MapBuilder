using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace Controller {
    
    enum Commands { MENU_VISIBILITY, MOVE_MENU_MODE_SWITCH, DRAW_MODE_SWITCH, BUCKET_MODE_SWITCH, MOVE_UP, MOVE_DOWN, MOVE_LEFT, MOVE_RIGHT, UPDATE_TILE_PRIMARY, UPDATE_TILE_SECONDARY }

    // to be used later to swap between different modes for the map editor
    enum ControllerMode { MOVE_MENU, DRAW, BUCKET_FILL }


    public class GameControl {

        // Button Class used to keep track of whether a button is being held down
        private class Button {
            public Keys Key{ get; set; }
            public Keys SecondaryKey{ get; set; }
            public bool IsPressed{ get; set; }
            public Commands effectName{get;}

            public Button(Keys key, Keys key2, Commands name) {
                Key = key;
                SecondaryKey = key2;
                IsPressed = false;
                effectName = name;
            }

            public Button(Keys key, Commands name) {
                Key = key;
                SecondaryKey = Keys.None;
                IsPressed = false;
                effectName = name;
            }
        }// end Button

        private Button[] commandKeys;               // All Key commands with the shift 
        private ControllerMode mode;                // The mode that the controller is currently in

        public GameControl() {
            commandKeys = new Button[] {
                // Toggles Menu Visibility
                new Button(Keys.V, Keys.LeftShift, Commands.MENU_VISIBILITY),
                // Toggles to Move Menu Mode
                new Button(Keys.M, Keys.LeftShift, Commands.MOVE_MENU_MODE_SWITCH),
                // Toggles to Bucket Fill Mode
                new Button(Keys.G, Commands.BUCKET_MODE_SWITCH),
                // Movement commands
                new Button(Keys.Up, Commands.MOVE_UP),
                new Button(Keys.Down, Commands.MOVE_DOWN),
                new Button(Keys.Left, Commands.MOVE_LEFT),
                new Button(Keys.Right, Commands.MOVE_RIGHT)
            };
            

            mode = ControllerMode.DRAW;
        }// end GameControl

        public void Update(MapBuilder.Game1 game, GameTime gameTime) {
            BrushInputManager(game);
            CommandInputs(game, gameTime);
        }


        // Checks keys to check if any MenuEffects should be triggered
        public void CommandInputs(MapBuilder.Game1 game, GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case Commands.MENU_VISIBILITY:
                        // Toggles menu visibility
                        UseCommand(kState, butt, () => game.TileMenu.ToggleVisibility());
                        break;
                    // Swap to move menu mode
                    case Commands.MOVE_MENU_MODE_SWITCH:
                        // Toggles to move menu mode
                        if(mode == ControllerMode.DRAW)
                            UseCommand(kState, butt, () => mode = ControllerMode.MOVE_MENU);
                        else if(mode == ControllerMode.MOVE_MENU)
                            UseCommand(kState, butt, () => mode = ControllerMode.DRAW);
                        break;
                    case Commands.BUCKET_MODE_SWITCH:
                        if(mode == ControllerMode.DRAW)
                            UseCommand(kState, butt, () => mode = ControllerMode.BUCKET_FILL);
                        else if(mode == ControllerMode.BUCKET_FILL)
                            UseCommand(kState, butt, () => mode = ControllerMode.DRAW);
                        break;
                    case Commands.MOVE_UP:
                        if(mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            game.TileMenu.MoveUp(gameTime);
                        break;
                    case Commands.MOVE_DOWN:
                        if(mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            game.TileMenu.MoveDown(gameTime);
                        break;
                    case Commands.MOVE_RIGHT:
                        if(mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            game.TileMenu.MoveRight(gameTime);
                        break;
                    case Commands.MOVE_LEFT:
                        if(mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            game.TileMenu.MoveLeft(gameTime);
                        break;
                    default:
                        break;
                }
            }// end foreach loop
        }// end MenuEffects()

        // Controls how the mouse inputs effect the Brushes Drawing ability
        private void BrushInputManager(MapBuilder.Game1 game) {
            var mouseState = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(mouseState.X, mouseState.Y);
            if(mouseState.LeftButton == ButtonState.Pressed) {
                // Set Show Secondary to False
                game.Brush.ShowSecondary = false;
                BrushInputHelper(game, mouseLoc);
            }
            else if(mouseState.RightButton == ButtonState.Pressed) {
                // Set Brush to display secondary Texture in its view
                game.Brush.ShowSecondary = true;
                BrushInputHelper(game, mouseLoc);
            }
            else
                game.Brush.ShowSecondary = false;
        }// end MouseEffects()

        // Helps BrushInputManager by Making sure that the map is being updated Correctly
        private void BrushInputHelper(MapBuilder.Game1 game, Vector2 mouseLoc) {
            // Gets the current texture
            Texture2D texture = game.Brush.GetCurrentTile().Texture;
            // Check to see if it clicked on a menu icon
            Texture2D tempTile = game.TileMenu.GetTileTexture(mouseLoc);
            if(tempTile != null) {
                game.Brush.ChangeTexture(tempTile);
            }
            else if(game.TileMenu.IfMenuClicked(mouseLoc)) {
                if(!game.TileMenu.MenuClicked)
                    game.TileMenu.MenuClicked = true;
            }    
            // If the menu icon wasn't clicked update the tile below it
            else {
                if(mode == ControllerMode.DRAW)
                    game.Map.UpdateTile(texture, mouseLoc);
                else if(mode == ControllerMode.BUCKET_FILL)
                    game.Brush.BucketFill(game.Map, mouseLoc, texture);
                game.TileMenu.MenuClicked = false;
            }
        }

        private void UseCommand(KeyboardState kState, Button butt, Action action) {
            // If the secondary key does not exist or the secondary button is pressed
            if(butt.SecondaryKey == Keys.None || kState.IsKeyDown(butt.SecondaryKey)) {
                if(kState.IsKeyDown(butt.Key))
                    if(!butt.IsPressed) {
                        action();
                        butt.IsPressed = true;
                    }
                if(kState.IsKeyUp(butt.Key))
                    butt.IsPressed = false;
            }
        }// end UseCommand()

    }// end GameControl
}