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

    public class MapController : BaseController {
        private enum MenuCommands { MENU_VISIBILITY, MOVE_MENU_MODE_SWITCH, DRAW_MODE_SWITCH, 
        BUCKET_MODE_SWITCH, MOVE_UP, MOVE_DOWN, MOVE_LEFT, 
        MOVE_RIGHT, UPDATE_TILE_PRIMARY, UPDATE_TILE_SECONDARY,
        RESET_SNAKE }

        protected enum ControllerMode { MOVE_MENU, DRAW, BUCKET_FILL }

        private MenuSystem.TilePickerMenu TileMenu;
        private Drawing.Brush Brush;
        private TileMap.Background Map;
        private ControllerMode mode;                // The mode that the controller is currently in

        public MapController(MenuSystem.TilePickerMenu tMenu, Drawing.Brush brsh, TileMap.Background map) {
            TileMenu = tMenu;
            Brush  = brsh;
            Map = map;
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
        }// end Contructor

        public MapController(MapBuilder.Game1 game) : this(game.TileMenu, game.Brush, game.Map) {}



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
        private TileMap.Background Map;

        public SnakeController(SnakeObjects.Snake snek, SnakeObjects.Fruit snekFruit, SpriteFont scoreFont, TileMap.Background map) {
            Snake = snek;
            SnakeFruit = snekFruit;
            Mode = ControllerMode.PLAY_GAME;
            ScoreFont = scoreFont;
            Map = map;
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

        public SnakeController(MapBuilder.Game1 game) : this(game.Snake, game.SnakeFruit, game.Font, game.Map) {}
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
            Map.Draw(spriteBatch);
            Snake.Draw(spriteBatch);
            SnakeFruit.Draw(spriteBatch);
            spriteBatch.DrawString(ScoreFont, "Score " + (Snake.GetLength() - 1), new Vector2(3000, 500), Color.Black);
        }
    }// end SnakeController

    // Master Controller class which will be used to control the flow of the game
    public class MasterController : BaseController {
        private enum MainControllerMode{ START_SCREEN, MAP_EDITOR, SNAKE_GAME }
        private enum SecondaryControllerMode{ OPTIONS_MENU, OFF }
        private enum MasterCommands{ EXIT_GAME, OPTIONS_MENU, SWITCH_TO_SNAKE, SWITCH_TO_MAPBUILDER}
        private MapBuilder.Game1 gamePointer;
        private SnakeController snakeController;
        private MapController mapController;
        private MainControllerMode primaryMode;
        //private SecondaryControllerMode secondaryMode;
        public MasterController(MapBuilder.Game1 game) {
            // Set all necessary controller 
            gamePointer = game;
            // Sets all modes to their starting states
            primaryMode = MainControllerMode.START_SCREEN;
            //secondaryMode = SecondaryControllerMode.OFF;
            // Sets all key commands
            commandKeys = new Button[] {
                new Button(Keys.Escape, MasterCommands.OPTIONS_MENU),
                new Button(Keys.Escape, Keys.LeftShift, MasterCommands.EXIT_GAME),
                new Button(Keys.S, Keys.LeftShift, MasterCommands.SWITCH_TO_SNAKE),
                new Button(Keys.M, Keys.LeftShift, MasterCommands.SWITCH_TO_MAPBUILDER)
            }; 
        }// end MasterController constructor

        public void Update(GameTime gameTime) {
            switch (primaryMode) {
                case MainControllerMode.START_SCREEN:
                    UpdateStartScreen();
                    break;
                case MainControllerMode.SNAKE_GAME:
                    snakeController.Update(gameTime);
                    break;
                case MainControllerMode.MAP_EDITOR:
                    mapController.Update(gameTime);
                    break;
            }
        }

        private void UpdateStartScreen() {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case MasterCommands.EXIT_GAME:
                        if(kState.IsKeyDown(butt.Key))
                            gamePointer.Exit();
                        break;
                    case MasterCommands.SWITCH_TO_MAPBUILDER:
                        if(primaryMode == MainControllerMode.START_SCREEN)
                            UseCommand(kState, butt, () => LoadMap());
                        break;
                    case MasterCommands.SWITCH_TO_SNAKE:
                        if(primaryMode == MainControllerMode.START_SCREEN)
                            UseCommand(kState, butt, () => LoadSnake());
                        break;
                    default:
                        break;
                }
            }// end foreach loop
        }//end UpdateStartScreen()

        private void LoadSnake() {
            // Load all of the game logic for the snake game
            gamePointer.LoadSnake();
            // Initialize the controller
            snakeController = new SnakeController(gamePointer);
            // Change mode to Snake
            primaryMode = MainControllerMode.SNAKE_GAME;
        }// end LoadSnake()

        private void LoadMap() {
            // Load all game logic for the map
            gamePointer.LoadMap();
            // Create the map controller
            mapController = new MapController(gamePointer);
            // Set the mode to null
            primaryMode = MainControllerMode.MAP_EDITOR;
        }// end LoadMap()

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            switch(primaryMode) {
                case MainControllerMode.START_SCREEN:
                    break;
                case MainControllerMode.SNAKE_GAME:
                    snakeController.Draw(gameTime, spriteBatch);
                    break;
                case MainControllerMode.MAP_EDITOR:
                    mapController.Draw(spriteBatch);
                    break;
            }
        }

    }// end MasterController
}