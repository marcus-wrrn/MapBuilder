// This class includes all objects for snake subgame that we are using for the game (idk will improve on this when I'm feeling better)
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Linq;



namespace SnakeObjects {
    public enum Movement { LEFT, RIGHT, UP, DOWN, }

    // Fruit object
    public class Fruit {
        public Assets.GameAsset Object{ get; set; }
        private TileMap.Background background;
        private Snake snek;
        private Random rand;

        public Fruit(Assets.GameAsset fruitObject, TileMap.Background tileMap, Snake snake) {
            Object = fruitObject;
            background = tileMap;
            rand = new Random();
            snek = snake;
            // Randomize fruit location
            GenerateLocation();
        }// end constructor

        public void GenerateLocation() {
            do  {
                int row = rand.Next(0, background.Rows);
                int col = rand.Next(0, background.Columns);
                Object.Location = background.GetTile(row, col).Location;
            } while(snek.TouchingSnake(Object.Location));
            
        }// end GenerateLocation()


        public void Update() {
            if(snek.TouchingSnake(Object.Location)) {
                GenerateLocation();
                snek.GrowBody();
            }
                
        }
        public void Draw(SpriteBatch spriteBatch) {
            Object.Draw(spriteBatch);
        }// end Draw()

    }// end Fruit

    

    public class SnakeParts : Assets.GameAsset {
        public Movement Direction{ get; set; }
        public SnakeParts(Movement direction, Texture2D text, Vector2 loc, float speed) : base(text, loc, speed) {
            Direction = direction;
        }
        public SnakeParts(Movement direction, Assets.GameAsset asset) : base(asset.Texture, asset.Location, asset.Speed) {
            Direction = direction;
        }

        public SnakeParts GetClone() {
            return new SnakeParts(Direction, Texture, Location, Speed);
        }
    }

    public class Snake {
        public Assets.GameAsset BaseAsset{ get; set; }
        public List<SnakeParts> Body{ get; set; }
        private TileMap.Background background;

        public Snake(Assets.GameAsset baseTile, TileMap.Background bckground) {
            BaseAsset = baseTile;
            Body = new List<SnakeParts>();
            Body.Add(new SnakeParts(Movement.DOWN, baseTile));
            background = bckground;
        }// end constructor()

        
        // Both SetNewPartLoc() and MoveLocation have the same code except their values are reversed
        private void SetNewPartLoc(SnakeParts part) {
            var location = part.Location;
            switch (part.Direction) {
                case Movement.RIGHT:
                    location.X -= part.Texture.Width;
                    break;
                case Movement.LEFT:
                    location.X += part.Texture.Width;
                    break;
                case Movement.DOWN:
                    location.Y += part.Texture.Height;
                    break;
                // by default it moves up
                default:
                    location.Y -= part.Texture.Height;
                    break;
            }
            part.Location = location;
        }

        private void MoveLocation(SnakeParts part, GameTime gameTime) {
            var location = part.Location;
            switch (part.Direction) {
                case Movement.RIGHT:
                    part.MoveRight(gameTime);
                    break;
                case Movement.LEFT:
                    part.MoveLeft(gameTime);
                    break;
                case Movement.DOWN:
                    part.MoveDown(gameTime);
                    break;
                // by default it moves up
                default:
                    part.MoveUp(gameTime);
                    break;
            }
        }

        public bool TouchingSnake(Vector2 location) {
            foreach(var part in Body) {
                if((int)location.Y <= (int)part.Location.Y + part.Texture.Height && (int)location.Y >= (int)part.Location.Y - part.Texture.Height)
                    if((int)location.X <= (int)part.Location.X + part.Texture.Width && (int)location.X >= (int)part.Location.X - part.Texture.Width)
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
            Body[0].Direction = Movement.UP;
        }

        public void MoveDown() {
            Body[0].Direction = Movement.DOWN;
        }

        public void MoveLeft() {
            Body[0].Direction = Movement.LEFT;
        }
        
        public void MoveRight() {
            Body[0].Direction = Movement.RIGHT;
        }

        // Updates the Snake Body
        public void Update(GameTime gameTime) {
            Console.WriteLine(gameTime.TotalGameTime);
            // Moves all parts by their set location
            for(int i = 0; i < Body.Count; i++) {
                MoveLocation(Body[i], gameTime);
            }
            // Updates all snake parts to their updated directions
            // This means that their directions are all updated by the piece before them
            Movement directionSetter;
            Movement directionContainer = Body[0].Direction;
            // Sets swaps all new parts direction
            for(int i = 1; i < Body.Count; i++) {
                directionSetter = directionContainer;
                directionContainer = Body[i].Direction;
                Body[i].Direction = directionSetter;
            }
        }// end Update()

        public void Draw(SpriteBatch spriteBatch) {
            foreach(var part in Body) {
                part.Draw(spriteBatch);
            }
        }

    }// end Snake

}// end SnakeObjects namespacee
