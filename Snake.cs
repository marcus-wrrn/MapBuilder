// This class includes all objects for snake subgame that we are using for the game (idk will improve on this when I'm feeling better)
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SnakeObjects {
    public enum Movement { LEFT, RIGHT, UP, DOWN, }
    public enum PartType { HEAD, BODY, TAIL }

    // Fruit object
    public class Fruit {
        public Assets.GameAsset Object{ get; set; }
        private int row;
        private int column;
        private TileMap.Background background;
        private Snake snek;
        private Random rand;

        public Fruit(Texture2D fruitTexture, TileMap.Background tileMap, Snake snake) {
            Object = new Assets.GameAsset(fruitTexture, 0.0f);
            background = tileMap;
            rand = new Random();
            snek = snake;
            // Randomize fruit location
            GenerateLocation();
        }// end constructor

        public void GenerateLocation() {
            do  {
                row = rand.Next(0, background.Rows);
                column = rand.Next(0, background.Columns);
                Object.Location = background.GetTile(row, column).Location;
            } while(snek.IsTouchingSnake(row, column));
        }// end GenerateLocation()


        public void Update() {
            if(snek.IsTouchingSnake(row, column)) {
                GenerateLocation();
                snek.GrowBody();
            }
                
        }
        public void Draw(SpriteBatch spriteBatch) {
            Object.Draw(spriteBatch);
        }// end Draw()

    }// end Fruit
    
    public class SnakeTextures {
        public Texture2D HorizontalBody{ get; private set; }
        public Texture2D VerticalBody{ get; private set; }
        public Texture2D HeadDown{ get; private set; }
        public Texture2D HeadUp{ get; private set; }
        public Texture2D HeadLeft{ get; private set; }
        public Texture2D HeadRight{ get; private set; }
        public Texture2D TailDown{ get; private set; }
        public Texture2D TailUp{ get; private set; }
        public Texture2D TailRight{ get; private set; }
        public Texture2D TailLeft{ get; private set; }
        public Texture2D TurnLeftDown{ get; private set; }
        public Texture2D TurnLeftUp{ get; private set; }
        public Texture2D TurnRightDown{ get; private set; }
        public Texture2D TurnRightUp{ get; private set; }

        public SnakeTextures(MapBuilder.Game1 game) {
            HorizontalBody = game.Content.Load<Texture2D>("Snake_Body_Horizon");
            VerticalBody = game.Content.Load<Texture2D>("Snake_Body_Vert");
            HeadDown = game.Content.Load<Texture2D>("Snake_Head_Down");
            HeadUp = game.Content.Load<Texture2D>("Snake_Head_Up");
            HeadLeft = game.Content.Load<Texture2D>("Snake_Head_Left");
            HeadRight = game.Content.Load<Texture2D>("Snake_Head_Right");
            TailDown = game.Content.Load<Texture2D>("Snake_Tail_Down");
            TailUp = game.Content.Load<Texture2D>("Snake_Tail_Up");
            TailLeft = game.Content.Load<Texture2D>("Snake_Tail_Left");
            TailRight = game.Content.Load<Texture2D>("Snake_Tail_Right");
            TurnLeftDown = game.Content.Load<Texture2D>("Snake_Turn_Left_Down");
            TurnLeftUp = game.Content.Load<Texture2D>("Snake_Turn_Left_Up");
            TurnRightDown = game.Content.Load<Texture2D>("Snake_Turn_Right_Down");
            TurnRightUp = game.Content.Load<Texture2D>("Snake_Turn_Right_Up");
        }
    }// end SnakeTextures

    public class SnakeParts : Assets.GameAsset {
        public Movement Direction{ get; set; }
        public PartType PartType{ get; set; }
        public int Row{ get; set; }
        public int Col{ get; set; }
        private TileMap.Background Background;
        public SnakeParts(Movement direction, PartType partType, Assets.GameAsset asset, TileMap.Background bckground) : base(asset.Texture, asset.Location, asset.Speed) {
            PartType = partType;
            Direction = direction;
            Background = bckground;
            Row = bckground.GetRowNumber(asset.Location);
            Col = bckground.GetColumnNumber(asset.Location);
        }
        public SnakeParts(Movement direction, Assets.GameAsset asset, TileMap.Background background) : this(direction, PartType.BODY, asset, background) {}
        public SnakeParts GetClone() {
            return new SnakeParts(Direction, PartType, this, Background);
        }
        public void MoveUp() {
            Row--;
            Location = Background.GetTile(Row, Col).Location;
        }
        public void MoveDown() {
            Row++;
            Location = Background.GetTile(Row, Col).Location;
        }
        public void MoveRight() {
            Col++;
            Location = Background.GetTile(Row, Col).Location;
        }
        public void MoveLeft() {
            Col--;
            Location = Background.GetTile(Row, Col).Location;
        }
    }

    public class Snake {
        private const int SNAKE_SPEED = 5;
        // Direction to determine the location of the snake head for each frame, used so the player turn the snake a full 180 and end the game on a single frame
        private Movement SnakeHeadDirection;
        private SnakeTextures BodyTextures;
        private int SnakeCount = 0;
        private Assets.GameAsset BaseAsset{ get; set; }
        private List<SnakeParts> Body{ get; set; }
        private TileMap.Background background;

        public Snake(SnakeTextures textures, TileMap.Background bckground) {
            SnakeHeadDirection = Movement.DOWN;
            background = bckground;
            BodyTextures = textures;
            // Set BaseAsset 
            SetBaseAsset(textures.HeadDown);
            // Set the Bodies state to its starting position
            SetDefaultSnake();
        }// end constructor()

        private void SetBaseAsset(Texture2D texture) {
            // Set the BaseAsset
            int row = background.Rows/2;
            int col = background.Columns/2;
            var location = background.GetTile(row, col).Location;
            Assets.GameAsset baseTile = new Assets.GameAsset(texture, location, SNAKE_SPEED);
            BaseAsset = baseTile;
        }// end SetBaseAsset()

        // Sets the snake state to its starting state (i.e resets the game)
        private void SetDefaultSnake() {
            Body = new List<SnakeParts>();
            Body.Add(new SnakeParts(SnakeHeadDirection, BaseAsset, background));
        }// end SetDefaultSnake()
        
        // Both SetNewPartLoc() and MoveLocation have the same code except their values are reversed
        private void SetNewPartLoc(SnakeParts part) {
            switch (part.Direction) {
                case Movement.RIGHT:
                    part.Col++;
                    part.Location = background.GetTile(part.Row, part.Col).Location;
                    break;
                case Movement.LEFT:
                    part.Col--;
                    part.Location = background.GetTile(part.Row, part.Col).Location;
                    break;
                case Movement.DOWN:
                    part.Row++;
                    part.Location = background.GetTile(part.Row, part.Col).Location;
                    break;
                // by default it moves up
                default:
                    part.Row--;
                    part.Location = background.GetTile(part.Row, part.Col).Location;
                    break;
            }
        }// end SetNewPartLoc()

        private void MoveLocation(SnakeParts part) {
            switch (part.Direction) {
                case Movement.RIGHT:
                    part.MoveRight();
                    break;
                case Movement.LEFT:
                    part.MoveLeft();
                    break;
                case Movement.DOWN:
                    part.MoveDown();
                    break;
                // by default it moves up
                default:
                    part.MoveUp();
                    break;
            }
        }

        public bool IsTouchingSnake(int row, int col) {
            foreach(var part in Body) {
                if(part.Row == row && part.Col == col)
                    return true;
            }
            return false;
        }

        public void GrowBody() {
            // Get last body part location
            SnakeParts nextBodyPart = Body.Last().GetClone();
            // Set new parts location
            SetNewPartLoc(nextBodyPart);
            // Add to the body
            Body.Add(nextBodyPart);
        }// end GrowBody()

        // Movement functions designed to update the first snake parts direction
        public void MoveUp() {
            // Checks if the head of the snake is moving in the opposite direction
            if(SnakeHeadDirection != Movement.DOWN)
                Body[0].Direction = Movement.UP;
        }// end MoveUp()

        public void MoveDown() {
            // Checks if the head of the snake is moving in the opposite direction
            if(SnakeHeadDirection != Movement.UP)
                Body[0].Direction = Movement.DOWN;
        }// end MoveDown()

        public void MoveLeft() {
            // Checks if the head of the snake is moving in the opposite direction
            if(SnakeHeadDirection != Movement.RIGHT)
                Body[0].Direction = Movement.LEFT;
        }// end MoveLeft()
        
        public void MoveRight() {
            // Checks if the head of the snake is moving in the opposite direction
            if(SnakeHeadDirection != Movement.LEFT)
                Body[0].Direction = Movement.RIGHT;
        }// end MoveRight()

        private bool IsInBoundries(SnakeParts part) {
            var temp = part.GetClone();
            switch (temp.Direction) {
                case Movement.UP:
                    return background.IsOnMap(--temp.Row, temp.Col);
                case Movement.DOWN:
                    return background.IsOnMap(++temp.Row, temp.Col);
                case Movement.LEFT:
                    return background.IsOnMap(temp.Row, --temp.Col);
                default:
                    return background.IsOnMap(temp.Row, ++temp.Col);
            }
        }// end IsInBoundries()

        private bool WillNotTouchSnake(SnakeParts part) {
            var temp = part.GetClone();
            switch (temp.Direction) {
                case Movement.UP:
                    return IsTouchingSnake(--temp.Row, temp.Col);
                case Movement.DOWN:
                    return IsTouchingSnake(++temp.Row, temp.Col);
                case Movement.LEFT:
                    return IsTouchingSnake(temp.Row, --temp.Col);
                default:
                    return IsTouchingSnake(temp.Row, ++temp.Col);
            }
        }// end WillNotTouchSnake()

        private bool CheckIfLegalMove(SnakeParts part) {
            return !WillNotTouchSnake(part) && IsInBoundries(part);
        }//end CheckBounds()

        // Updates the Snake Body
        public bool PlaySnake(GameTime gameTime) {
            var direction = Body[0].Direction;
            // Moves all parts by their set location
            if(SnakeCount >= SNAKE_SPEED) {
                // Updates all snake parts to their updated directions
                // This means that their directions are all updated by the piece before them
                SnakeParts directionSetter;
                SnakeParts directionContainer = Body[0];
                // Sets swaps all new parts direction
                for(int i = 1; i < Body.Count; i++) {
                    directionSetter = directionContainer;
                    directionContainer = Body[i];
                    Body[i] = directionSetter.GetClone();
                }
                var Head = Body[0];
                if(CheckIfLegalMove(Head))
                    MoveLocation(Head);
                else {
                    return false;
                }
                UpdateTextures();
                // Changes the direction of the head to the new updated direction
                SnakeHeadDirection = Head.Direction;
                // Sets the snake count timer to 0
                SnakeCount = 0;
            }
            else
                SnakeCount++;
            return true;
        }// end Update()

        private void UpdateHead(SnakeParts head) {
            switch(head.Direction) {
                case Movement.UP:
                    head.Texture = BodyTextures.HeadUp;
                    break;
                case Movement.LEFT:
                    head.Texture = BodyTextures.HeadLeft;
                    break;
                case Movement.RIGHT:
                    head.Texture = BodyTextures.HeadRight;
                    break;
                default:
                    head.Texture = BodyTextures.HeadDown;
                    break;
            }
        }// end UpdateHead()


        private bool PartIsHorizontal(SnakeParts part) {
            if(part.Direction == Movement.RIGHT || part.Direction == Movement.LEFT)
                return true;
            return false;
        }// end IsPartHorizontal

        private bool PartIsVertical(SnakeParts part) {
            if(part.Direction == Movement.UP || part.Direction == Movement.DOWN)
                return true;
            return false;
        }// end IsPartVertical

        private void UpdateBodyPart(SnakeParts last, SnakeParts current, SnakeParts first) {
            if(PartIsHorizontal(current)) {
                if(PartIsHorizontal(last) && PartIsHorizontal(first)) {
                    current.Texture = BodyTextures.HorizontalBody;
                }
                else if(last.Direction == Movement.LEFT) {
                    if(first.Direction == Movement.UP)
                        current.Texture = BodyTextures.TurnLeftDown;
                    if(first.Direction == Movement.DOWN)
                        current.Texture = BodyTextures.TurnLeftUp;
                }
                else if(last.Direction == Movement.RIGHT){
                    if(first.Direction == Movement.UP)
                        current.Texture = BodyTextures.TurnRightDown;
                    if(first.Direction == Movement.DOWN)
                        current.Texture = BodyTextures.TurnRightUp;
                }
            }
            if(PartIsVertical(current)) {
                if(PartIsVertical(last) && PartIsVertical(first)) {
                    current.Texture = BodyTextures.VerticalBody;
                }
                else if(first.Direction == Movement.LEFT) {
                    if(last.Direction == Movement.UP)
                        current.Texture = BodyTextures.TurnRightUp;
                    if(last.Direction == Movement.DOWN)
                        current.Texture = BodyTextures.TurnRightDown;
                }
                else if(first.Direction == Movement.RIGHT) {
                    if(last.Direction == Movement.DOWN)
                        current.Texture = BodyTextures.TurnLeftDown;
                    if(last.Direction == Movement.UP)
                        current.Texture = BodyTextures.TurnLeftUp;
                }
            }
        }

        private void UpdateBody() {
            // Loop through the body excluding the tail and head
            for(int i = 1; i < Body.Count -1; i++) {
                UpdateBodyPart(Body[i-1], Body[i], Body[i+1]);
            }
        }

        private void UpdateTail(SnakeParts tail) {
            switch(tail.Direction) {
                //TODO change snake textures
                case Movement.UP:
                    tail.Texture = BodyTextures.TailDown;
                    break;
                case Movement.DOWN:
                    tail.Texture = BodyTextures.TailUp;
                    break;
                case Movement.LEFT:
                    tail.Texture = BodyTextures.TailLeft;
                    break;
                default:
                    tail.Texture = BodyTextures.TailRight;
                    break;
            }
        }

        private void UpdateTextures() {
            UpdateHead(Body[0]);
            UpdateBody();
            UpdateTail(Body.Last());
        }
        
        public bool IsGameOver() {
            // The head of the snake
            var head = Body[0];
            // If the head is greater than the borders the game is over
            if(head.Row > background.Rows && head.Col > background.Columns)
                return true;
            // If The snake has only one piece the game cannot be over
            if(Body.Count == 1)
                return false;
            // If the pieces of the snake touch the game is over
            
            return false;
        }// end IsGameOver()

        public void ResetSnake() {
            Body.Clear();
            Body.Add(new SnakeParts(Movement.DOWN, BaseAsset, background));
        }// end ResetSnake()

        public int GetLength() {
            return Body.Count;
        }// end GetLength()

        public void Draw(SpriteBatch spriteBatch) {
            foreach(var part in Body) {
                part.Draw(spriteBatch);
            }
        }// end Draw()
    }// end Snake
}// end SnakeObjects namespacee
