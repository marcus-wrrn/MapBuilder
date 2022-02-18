using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileMap;
using MenuSystem;

namespace MapBuilder
{
    public class Game1 : Game
    {
        private Texture2D ball;
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Background background;
        private Texture2D currentTile;
        private Texture2D secondTile;
        private TilePickerMenu menu;
        private string fileName = "test";

        private Controller.GameControl controller;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 3840;
            _graphics.PreferredBackBufferHeight = 2160;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        { 
            base.Initialize();
            controller = new Controller.GameControl();
        }

        protected override void LoadContent()
        {
            Texture2D[] test = new Texture2D[8];
            for(int i = 0; i < test.Length; i++) {
                test[i] = Content.Load<Texture2D>("tile2Test");
            }
            test[0] = Content.Load<Texture2D>("tile");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Texture2D>("Ball");
            currentTile = Content.Load<Texture2D>("tile");
            secondTile = Content.Load<Texture2D>("tile2Test");
            Texture2D menuTexture = Content.Load<Texture2D>("Menu");
            background = new Background(fileName, this);
            // background = new Background(currentTile, (int)(_graphics.PreferredBackBufferHeight / currentTile.Height), (int)(_graphics.PreferredBackBufferWidth / currentTile.Width));
            menu = new TilePickerMenu(test, 0, new Vector2(_graphics.PreferredBackBufferWidth - menuTexture.Width, 0), menuTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            var kstate = Keyboard.GetState();

            // Command Logic

            // If Shift + V is pressed toggle map visibility

            // if(kstate.IsKeyDown(Keys.V) && kstate.IsKeyDown(Keys.LeftShift)) {
            //     if(keyRelease)
            //         menu.toggleVisibility();
            //     keyRelease = false;
            // }

            // if(kstate.IsKeyUp(Keys.V) && !keyRelease)
            //     keyRelease = true;
                
            controller.MenuEffects(this, menu, gameTime);
            // Mouse Logic
            // Finds the position of the mouse
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 mouseStart = new Vector2(0,0);
            // If left mouse button pressed
            if(Mouse.GetState().LeftButton == ButtonState.Pressed) {
                // Check to see if it clicked on a menu icon
                Texture2D tempTile = menu.GetTileTexture(mouse);
                if(tempTile != null)
                    currentTile = tempTile;
                else if(menu.IfMenuClicked(mouse)) {
                    if(!menu.MenuClicked) {
                        System.Console.WriteLine("WTF");
                        menu.MenuClicked = true;
                        mouseStart.X = mouse.X;
                        mouseStart.Y = mouse.Y;
                    }
                    else  {
                        System.Console.WriteLine("It got here!");
                        //menu.DragAndDrop(mouse, mouseStart, gameTime);
                    }
                }    
                // If the menu icon wasn't clicked update the tile below it
                else {
                    background.updateTile(currentTile, mouse);
                    menu.MenuClicked = false;
                }
                 
            }
            // If e was pressed export the map to a binary file
            if(Keyboard.GetState().IsKeyDown(Keys.E))
                background.exportToBinary(fileName);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            background.Draw(_spriteBatch);
            menu.Draw(_spriteBatch);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        
    }
}
