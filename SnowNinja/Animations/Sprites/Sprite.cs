using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SnowNinja.Animations.Managers;
using SnowNinja.Animations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Animations.Sprites
{
    public class Sprite
    {
        #region Fields

        protected AnimationManager _animationManager; //managment of animation of the Sprite.

        public Dictionary<string, Animation> _animations;// contains the sets of animation of the Sprite.

        public Vector2 _position;

        protected Texture2D _texture;//in case of using the Sprite class without animations. 

        #endregion

        #region Properties

        public Input Input;// objects that contains types of Keys.

        public KeyboardState _currentKey;//Keyboard managment
        public KeyboardState _previousKey;//Keyboard managment

        public string keyboardString = "";// used in the Internet part.

        public float scale;

        public HealthBar health;

        public float rotation = 0f;

        public bool hasWon = false;// boolean that changes if the sprite has won or not.

        public Vector2 origin;

        public Vector2 Position // a field that sets the position to the animation if there is an animation.
        {
            get { return _position; }
            set
            {
                _position = value;

                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }

        public Rectangle aniRectangle
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y,
                    (int)(Game1.anyPlayerTex.Width * (10 / scale)), (int)(Game1.anyPlayerTex.Height * scale));
            }
        }

        public Rectangle rec
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            }
        }

        public float Speed;

        public Vector2 Velocity;// the speed & direction of the sprite

        public float knifeScale; // modifying the scale of the knife.

        public float texHeight;

        public bool hasJumped;

        public string dir = "none";

        public List<Knife> knivesSet = new List<Knife>();// the knives the sprite is throwing.

        public Color color;

        public string PlayerType;// whether "Player" or "Enemy". 

        #endregion

        #region Methods

        /// <summary>
        /// Sprite's animations contsructor.
        /// </summary>

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);

            hasJumped = false;

        }

        /// <summary>
        /// Sprite's regular constructor.
        /// </summary>
        public Sprite(Texture2D texture)
        {
            _texture = texture;
            hasJumped = false;

        }

        public bool toThrow;// throwing knives managment

        /// <summary>
        ///manages the movement & jumping & throwing knives of the sprite. 
        /// </summary>

        protected virtual void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Velocity.X = Speed;// moves right.
                dir = "right";
                keyboardString = "right";
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Velocity.X = -Speed;// moves left.
                dir = "left";
                keyboardString = "left";
            }
            else Velocity.X = 0f;//stands still.

            #region Jumping

            if (Keyboard.GetState().IsKeyDown(Input.Jump) && hasJumped == false)//jumping scenario.
            {
                _position.Y -= 20f;
                Velocity.Y = -10f;
                hasJumped = true;
                keyboardString = "jump";
            }

            float i = 1;
            Velocity.Y += 0.15f * i;// makes the sprite fall down.


            if (_position.Y + texHeight >= Game1.ScreenHeight)// stop falling when hits ground.
            {
                hasJumped = false;
            }

            if (hasJumped == false)// stop falling when hits ground.
            {
                Velocity.Y = 0f;
            }

            #endregion

            if (_currentKey.IsKeyUp(Input.Throw) && _previousKey.IsKeyDown(Input.Throw))// throwing scenario
            {
                toThrow = true;
                keyboardString = "throw";
            }
            else
            {
                toThrow = false;
                keyboardString = "";
            }
            if (Keyboard.GetState().IsKeyDown(Input.Down))
            {
                keyboardString = "down";
            }
        }

        /// <summary>
        /// sets the animations of the sprite based on Velocity, toThrow and hasJumped.
        /// </summary>

        public virtual void SetAnimations()
        {
            if (Velocity.X == 0f && Velocity.Y == 0f)
            { //displaying the direction of the standing sprite.
                if (dir.Equals("none"))
                    _animationManager.Play(_animations["standingR"]);
                else if (dir.Equals("right"))
                    _animationManager.Play(_animations["standingR"]);
                else if (dir.Equals("left"))
                    _animationManager.Play(_animations["standingL"]);

                if (toThrow)// throwing animation
                {
                    if (dir.Equals("right"))
                        _animationManager.Play(_animations["throwingR"]);
                    else if (dir.Equals("left"))
                        _animationManager.Play(_animations["throwingL"]);
                }
            }

            if (toThrow)// throwing animation
            {
                if (dir.Equals("right"))
                    _animationManager.Play(_animations["throwingR"]);
                else if (dir.Equals("left"))
                    _animationManager.Play(_animations["throwingL"]);
            }
            
            if (!hasJumped && Velocity.X > 0)
                _animationManager.Play(_animations["runningR"]);

            else if (!hasJumped && Velocity.X < 0)
                _animationManager.Play(_animations["runningL"]);


            else if (hasJumped)// jumping animation
            {
                if (Velocity.X > 0)
                    _animationManager.Play(_animations["jumpingR"]);
                else if (Velocity.X < 0) _animationManager.Play(_animations["jumpingL"]);
            }
        }

        /// <summary>
        /// updates the position so it stays in screen borders.
        /// </summary>

        protected void keepInScreen()
        {
            _position.X = MathHelper.Clamp(_position.X,
                _animations["runningR"].FrameWidth / 10 + 23f, Game1.ScreenWidth - 50f);
            _position.Y = MathHelper.Min(_position.Y, Game1.ScreenHeight - _animations["runningR"].FrameHeight * scale);
        }

        /// <summary>
        /// checks hit by enemy's knives. 
        /// </summary>

        public void CheckHealth(Sprite enemy)
        {
            var halfWidth = 40 * scale;
            var halfHeight = 144 * scale;
            foreach (var enemyKnife in enemy.knivesSet)
            {
                if ((enemyKnife._position.X >= _position.X - 20) && (enemyKnife._position.X <= _position.X + 20) //sprite's body
                    && (enemyKnife._position.Y >= _position.Y - 72) && enemyKnife._position.Y <= _position.Y + 72)//sprite's body
                {
                    health.hit();
                    enemyKnife.IsRemoved = true;
                }
            }

            if (health.rectangle.Width <= 0)// win scenario
                enemy.hasWon = true;
        }

        /// <summary>
        /// adds a new knife to knivesSet
        /// </summary>
        public void addKnife()
        {
            Knife temp = new Knife(Game1.knifeTexture);

            temp.origin = new Vector2(Game1.knifeTexture.Width / 2, Game1.knifeTexture.Height / 2);

            temp.scale = 0.2f;

            temp.knifeDir = dir;

            temp.Position = new Vector2(_position.X, _position.Y);

            knivesSet.Add(temp);

        }

        /// <summary>
        /// updates the knifes based on keyboard
        /// </summary>
        protected virtual void KnifeStates(GameTime gameTime)
        {
            if (_previousKey.IsKeyDown(Input.Throw) &&
                    _currentKey.IsKeyUp(Input.Throw))
            {
                addKnife();
                Game1._throwing.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
            }

            foreach (var knife in knivesSet)
            {
                knife.UpdateKnife();
            }
        }
        
        /// <summary>
        /// updates the Sprite's animations, movement and more.
        /// </summary>
        public virtual void Update(GameTime gameTime, Sprite enemy)
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
            
            KnifeStates(gameTime);

            Move();

            SetAnimations();

            _animationManager.Update(gameTime);

            Position += Velocity;

            keepInScreen();

            CheckHealth(enemy);

        }

        /// <summary>
        /// part 1: regular Sprite's Draw.
        /// part 2: animations's Draw.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                spriteBatch.Draw(_texture, Position, null, color, rotation,
                    origin, scale, SpriteEffects.None, 1);
                //health.Draw(spriteBatch);
                //DrawKnives(spriteBatch);
            }
            else if (_animationManager != null)
            {
                _animationManager.Draw(spriteBatch, color);
                health.Draw(spriteBatch);
                DrawKnives(spriteBatch);
            }
        }

        /// <summary>
        /// draws the knives.
        /// </summary>
        public virtual void DrawKnives(SpriteBatch sb)
        {
            foreach (var knife in knivesSet)
            {
                sb.Draw(Game1.knifeTexture, knife.Position, null, Color.White, knife.rotation,
                    knife.origin, knifeScale, SpriteEffects.None, 1);
            }

        }

        #endregion
    }
}
