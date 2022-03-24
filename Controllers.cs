using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace Controllers {
    
    // +++++++++++++++++++++++++++++++++++++++++++ Initialization for Controllers +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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

    // TODO implement a proper interface into classes
    public interface ControllerInterface {
        void Update(GameTime gameTime);
        void CommandInputs(GameTime gameTime);
        void Draw(GameTime gameTime);
    }// end ContorlollerInterface

    // Base Controller for all classes
    public class BaseController {
        // to be used later to swap between different modes for the map editor
        protected Button[] commandKeys;               // All Key commands with the shift 
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

    // +++++++++++++++++++++++++++++++++++++++++++ Map Controller +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // Map Controller for controller the MapBuilder functionality of the code
    public class MapController : BaseController {
        private enum MenuCommands { MENU_VISIBILITY, MOVE_MENU_MODE_SWITCH, DRAW_MODE_SWITCH, 
        BUCKET_MODE_SWITCH, MOVE_UP, MOVE_DOWN, MOVE_LEFT, 
        MOVE_RIGHT, UPDATE_TILE_PRIMARY, UPDATE_TILE_SECONDARY,
        SAVE_MAP }

        protected enum ControllerMode { MOVE_MENU, DRAW, BUCKET_FILL }

        private Containers.GameEditorContainer _container;
        private ControllerMode mode;                // The mode that the controller is currently in

        public MapController(Containers.GameEditorContainer container) {
            _container = container;
            // TODO add save/load file path

            commandKeys = new Button[] {
                // Toggles Menu Visibility
                new Button(Keys.V, Keys.LeftShift, MenuCommands.MENU_VISIBILITY),
                // Toggles to Move Menu Mode
                new Button(Keys.M, Keys.LeftShift, MenuCommands.MOVE_MENU_MODE_SWITCH),
                // Toggles to Bucket Fill Mode
                new Button(Keys.G, MenuCommands.BUCKET_MODE_SWITCH),
                new Button(Keys.S, Keys.LeftShift, MenuCommands.SAVE_MAP),
                // Movement commands
                new Button(Keys.Up, MenuCommands.MOVE_UP),
                new Button(Keys.Down, MenuCommands.MOVE_DOWN),
                new Button(Keys.Left, MenuCommands.MOVE_LEFT),
                new Button(Keys.Right, MenuCommands.MOVE_RIGHT)
            };
            mode = ControllerMode.DRAW;
        }// end Contructor

        public MapController(MapBuilder.Game1 game) : this(game.MapEditorContainer) {}

        // Checks keys to check if any MenuEffects should be triggered
        private void CommandInputs(GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case MenuCommands.MENU_VISIBILITY:
                        // Toggles menu visibility
                        UseCommand(kState, butt, () => _container.TileMenu.ToggleVisibility());
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
                    case MenuCommands.SAVE_MAP:
                        UseCommand(kState, butt, () => _container.SaveMap());
                        break;
                    case MenuCommands.MOVE_UP:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            _container.TileMenu.MoveUp(gameTime);
                        break;
                    case MenuCommands.MOVE_DOWN:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            _container.TileMenu.MoveDown(gameTime);
                        break;
                    case MenuCommands.MOVE_RIGHT:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            _container.TileMenu.MoveRight(gameTime);
                        break;
                    case MenuCommands.MOVE_LEFT:
                        if (mode == ControllerMode.MOVE_MENU && kState.IsKeyDown(butt.Key))
                            _container.TileMenu.MoveLeft(gameTime);
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
            _container.BrushTool.UpdateBrush(_container.Map, mouseLoc);
            if (mouseState.LeftButton == ButtonState.Pressed) {
                // Set Show Secondary to False
                _container.BrushTool.ShowSecondary = false;
                BrushInputHelper(mouseLoc);
            }
            else if (mouseState.RightButton == ButtonState.Pressed) {
                // Set Brush to display secondary Texture in its view
                _container.BrushTool.ShowSecondary = true;
                BrushInputHelper(mouseLoc);
            }
            else
                _container.BrushTool.ShowSecondary = false;
            
        }// end MouseEffects()

        // Helps BrushInputManager by Making sure that the map is being updated Correctly
        private void BrushInputHelper(Vector2 mouseLoc) {
            // Gets the current texture
            Texture2D texture = _container.BrushTool.GetCurrentTile().Texture;
            // Check to see if it clicked on a menu icon
            Texture2D tempTile = _container.TileMenu.GetTileTexture(mouseLoc);
            if (tempTile != null) {
                _container.BrushTool.ChangeTexture(tempTile);
            }
            else if (_container.TileMenu.IfMenuClicked(mouseLoc)) {
                if(!_container.TileMenu.MenuClicked)
                    _container.TileMenu.MenuClicked = true;
            }
            // If the menu icon wasn't clicked update the tile below it
            else {
                if (mode == ControllerMode.DRAW)
                    _container.Map.UpdateTile(texture, mouseLoc);
                else if(mode == ControllerMode.BUCKET_FILL)
                    _container.BrushTool.BucketFill(_container.Map, mouseLoc, texture);
                _container.TileMenu.MenuClicked = false;
            }
        }
        public void Update(GameTime gameTime) {
            CommandInputs(gameTime);
            BrushInputManager();
        }

        public void Draw(SpriteBatch spriteBatch) {
            _container.Map.Draw(spriteBatch);
            _container.BrushTool.DrawBrush(spriteBatch);
            _container.TileMenu.Draw(spriteBatch);
        }
            
    }// end MapControl

    // +++++++++++++++++++++++++++++++++++++++++++ Snake Controller +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // Snake Controller responsible for controlling the Snake game
    public class SnakeController : BaseController {
        protected enum ControllerMode { PLAY_GAME, GAME_OVER }
        private enum SnakeCommands { MOVE_UP, MOVE_DOWN, MOVE_LEFT, MOVE_RIGHT, RESET_SNAKE }
        private ControllerMode Mode;
        private Visualization.Camera _playerCamera;
        private SnakeObjects.Snake PlayerSnake;
        private SnakeObjects.Fruit SnakeFruit;
        private TileMap.Background Map;
        private SpriteFont Font;

        public SnakeController(MapBuilder.Game1 game) {
            // Get all values from the container
            UnpackContainer(game.SnakeContainer);
            _playerCamera = new Visualization.Camera(game, game.SnakeContainer);
            Mode = ControllerMode.PLAY_GAME;
            // Initiallize the key Commands
            commandKeys = new Button[] {
                // Toggles Menu Visibility
                new Button(Keys.Up, SnakeCommands.MOVE_UP),
                new Button(Keys.Down, SnakeCommands.MOVE_DOWN),
                new Button(Keys.Right, SnakeCommands.MOVE_RIGHT),
                new Button(Keys.Left, SnakeCommands.MOVE_LEFT),
                new Button(Keys.R, Keys.LeftShift, SnakeCommands.RESET_SNAKE)
            };
        }// end Constructor

        // Not strictly neccessary as you could just store everything in a container
        // but I think it makes the code more readable
        private void UnpackContainer(Containers.SnakeContainer container) {
            PlayerSnake = container.PlayerSnake;
            SnakeFruit = container.SnakeFruit;
            Map = container.Map;
            Font = container.Font;
        }// end UnpackContainer()

        // Updates the Snake
        public void Update(GameTime gameTime) {
            // Call the input commands
            CommandInputs();
            _playerCamera.Update();
            // Update game
            if(Mode == ControllerMode.PLAY_GAME) {
                if(PlayerSnake.PlaySnake(gameTime))
                    SnakeFruit.Update();
                else
                    Mode = ControllerMode.GAME_OVER; 
            }
        }// end Update

        private void CommandInputs() {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case SnakeCommands.MOVE_UP:
                        if (kState.IsKeyDown(butt.Key))
                            PlayerSnake.MoveUp();
                        break;
                    case SnakeCommands.MOVE_DOWN:
                        if (kState.IsKeyDown(butt.Key))
                            PlayerSnake.MoveDown();
                        break;
                    case SnakeCommands.MOVE_LEFT:
                        if (kState.IsKeyDown(butt.Key))
                            PlayerSnake.MoveLeft();
                        break;
                    case SnakeCommands.MOVE_RIGHT:
                        if (kState.IsKeyDown(butt.Key))
                            PlayerSnake.MoveRight();
                        break;
                    case SnakeCommands.RESET_SNAKE:
                        if (kState.IsKeyDown(butt.Key)) {
                            PlayerSnake.ResetSnake();
                            Mode = ControllerMode.PLAY_GAME;
                        }
                        break;
                    default:
                        break;
                }
            }// end foreach loop
        }// end CommandInputs()

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            // Map.Draw(spriteBatch);
            // PlayerSnake.Draw(spriteBatch);
            // SnakeFruit.Draw(spriteBatch);
            _playerCamera.Draw(gameTime, spriteBatch);
            if(Mode == ControllerMode.GAME_OVER)
                spriteBatch.DrawString(Font, "GAMEOVER", new Vector2(2000, 800), Color.Red);
            spriteBatch.DrawString(Font, "Score \n" + (PlayerSnake.GetLength() - 1), new Vector2(3000, 500), Color.WhiteSmoke);
        }
    }// end SnakeController

    // +++++++++++++++++++++++++++++++++++++++++++ StartMenu Controller +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class StartMenuController : BaseController {
        private MenuSystem.StartMenu _startMenu;
        enum KeyCommands { MOVE_UP, MOVE_DOWN, CHOSE_OPTION };

        public bool SnakeSelected{ get; private set; }
        public bool MenuSelected{ get; private set; }

        public StartMenuController(MapBuilder.Game1 game) {
            _startMenu = game._startMenu;
            commandKeys = new Button[] {
                new Button(Keys.Down, KeyCommands.MOVE_DOWN),
                new Button(Keys.Up, KeyCommands.MOVE_UP),
                new Button(Keys.Enter, KeyCommands.CHOSE_OPTION)
            };
        }// end Constructor

        public void Update(GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            foreach(Button butt in commandKeys) {
                // Switch case which goes through every available menu command 
                switch (butt.effectName) {
                    case KeyCommands.MOVE_UP:
                        UseCommand(kState, butt, () => _startMenu.MoveSelected());
                        break;
                    case KeyCommands.MOVE_DOWN:
                        UseCommand(kState, butt, () => _startMenu.MoveSelected());
                        break;
                    case KeyCommands.CHOSE_OPTION:
                        if(kState.IsKeyDown(butt.Key)) {
                            if(_startMenu.IsSnakeSelected())
                                SnakeSelected = true;
                            else
                                MenuSelected = true;
                        }
                        break;
                    default:
                        break;
                }
            }// end foreach loop
        }// end Update()

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            _startMenu.Draw(spriteBatch);
        }// end Draw()
    }

    // +++++++++++++++++++++++++++++++++++++++++++ Master Controller +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // Master Controller class which will be used to control the flow of the game
    public class MasterController : BaseController {
        private enum MainControllerMode{ START_SCREEN, MAP_EDITOR, SNAKE_GAME }
        private enum SecondaryControllerMode{ OPTIONS_MENU, OFF }
        private enum MasterCommands{ EXIT_GAME, OPTIONS_MENU, SWITCH_TO_SNAKE, SWITCH_TO_MAPBUILDER}
        private MapBuilder.Game1 gamePointer;
        private SnakeController snakeController;
        private MapController mapController;
        private StartMenuController _startMenuController;
        private MainControllerMode primaryMode;
        //private SecondaryControllerMode secondaryMode;

        public MasterController(MapBuilder.Game1 game) {
            // Set all necessary controller 
            gamePointer = game;
            // Loads Start Screen
            LoadStartScreen();
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
                    _startMenuController.Update(gameTime);
                    // If the player selected an option then load into
                    LoadStartSelection();
                    break;
                case MainControllerMode.SNAKE_GAME:
                    snakeController.Update(gameTime);
                    break;
                case MainControllerMode.MAP_EDITOR:
                    mapController.Update(gameTime);
                    break;
            }
        }// end Update()

        private void LoadStartSelection() {
            if(_startMenuController.SnakeSelected)
                LoadSnake();
            else if(_startMenuController.MenuSelected)
                LoadMap();
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
            //gamePointer.LoadMap();
            gamePointer.LoadNewMap("Testing", 50, 50);
            // Create the map controller
            mapController = new MapController(gamePointer);
            // Set the mode to null
            primaryMode = MainControllerMode.MAP_EDITOR;
        }// end LoadMap()

        private void LoadStartScreen() {
            gamePointer.LoadStartMenu();
            _startMenuController = new StartMenuController(gamePointer);
            primaryMode = MainControllerMode.START_SCREEN;
        }// end LoadStartScreen()

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            switch(primaryMode) {
                case MainControllerMode.START_SCREEN:
                    _startMenuController.Draw(gameTime, spriteBatch);
                    break;
                case MainControllerMode.SNAKE_GAME:
                    snakeController.Draw(gameTime, spriteBatch);
                    break;
                case MainControllerMode.MAP_EDITOR:
                    mapController.Draw(spriteBatch);
                    break;
            }
        }// end Draw()
    }// end MasterController
}