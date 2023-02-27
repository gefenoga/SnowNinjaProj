using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SnowNinja.Animations.Models;
using SnowNinja.Animations.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja
{
    class Player : Sprite
    {
        /// <summary>
        /// Player's construcrtor.
        /// </summary>
        public Player(Dictionary<string, Animation> animations) : base(animations)
        {
            PlayerType = "player";
            health = new HealthBar(Game1.healthTex, new Vector2(119, 101),
                  new Rectangle(0, 0, Game1.healthTex.Width - 20, Game1.healthTex.Height - 4));

            knivesSet = new List<Knife>();
            Position = new Vector2(50, 700);
            scale = 0.35f;
            knifeScale = 0.15f;
            Speed = 5f;
            origin = new Vector2(Game1.anyPlayerTex.Width / 20, Game1.anyPlayerTex.Height / 2);
            texHeight = Game1.anyPlayerTex.Height * 0.35f;
            Input = new Input()
            {
                Down = Keys.S,
                Left = Keys.A,
                Right = Keys.D,
                Throw = Keys.E,
                Jump = Keys.Space,
            };
            color = Color.White;
        }
        /// <summary>
        /// oerrides the Move func in Sprite class
        /// </summary>
        protected override void Move()
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

            if (Keyboard.GetState().IsKeyDown(Input.Throw))// throwing scenario
            {
                toThrow = true;
                keyboardString = "throw";
            }

            if (Keyboard.GetState().IsKeyDown(Input.Down))
            {
                keyboardString = "down";
            }
        }

        /// <summary>
        /// overrides the SetAnimations func in Sprite class. 
        /// </summary>
        public override void SetAnimations()
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

            if (!Keyboard.GetState().IsKeyUp(Input.Throw))// preventing spam clicking.
            {
                toThrow = false;
                keyboardString = "";
            }

            if (!hasJumped && Velocity.X > 0 && keyboardString.Equals("right"))
                _animationManager.Play(_animations["runningR"]);

            else if (!hasJumped && Velocity.X < 0 && keyboardString.Equals("left"))
                _animationManager.Play(_animations["runningL"]);


            else if (hasJumped)// jumping animation
            {
                if (Velocity.X > 0)
                    _animationManager.Play(_animations["jumpingR"]);
                else if (Velocity.X < 0) _animationManager.Play(_animations["jumpingL"]);
            }
        }

        /// <summary>
        /// overrides the KnifeStates func in Sprite Class.
        /// </summary>
        protected override void KnifeStates(GameTime gameTime)
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
    }
}
