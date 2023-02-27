using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
    class Bot : Sprite
    {
        Player _player;

        public string decision = ""; //what the bot is going to do.
        string dirToGo = "";// what direction the bot needs to would go to.

        float timer;

        /// <summary>
        /// Bot's constructor.
        /// </summary>
        public Bot(Dictionary<string, Animation> animations, Player player) : base(animations)
        {
            _player = player;
            PlayerType = "enemy";
            health = new HealthBar(Game1.healthTex, new Vector2(1019, 101),
                    new Rectangle(0, 0, Game1.healthTex.Width - 20, Game1.healthTex.Height - 4));
            Position = new Vector2(1200, 700);
            knivesSet = new List<Knife>();
            scale = 0.35f;
            Speed = 3f;
            knifeScale = 0.15f;
            origin = new Vector2(Game1.anyPlayerTex.Width / 20, Game1.anyPlayerTex.Height / 2);
            texHeight = Game1.anyPlayerTex.Height * 0.35f;
            color = new Color(64, 13, 13);
        }

        /// <summary>
        /// returns the distance between the two players.
        /// </summary>
        private float distance()
        {
            return Math.Abs(_position.X - _player._position.X);
        }

        /// <summary>
        /// decides what direction the bot in facing.
        /// </summary>
        private void GetCloser()
        {
            if (_position.X > _player._position.X)
            {
                dirToGo = "left";
            }
            else
            {
                dirToGo = "right";
            }

        }

        /// <summary>
        /// determines what the bot should do based on the player's position.
        /// </summary>
        private void BotDecisions()
        {
            if((_position.Y >= _player._position.Y - 72) && (_position.Y <= _player._position.Y + 72)){//in front of player
                decision = "throw";

                if (_position.X > _player._position.X)// to face the player
                {
                    dir = "left";
                }
                else
                {
                    dir = "right";
                }
            }
            
            else if(_position.Y > _player._position.Y) // beneath the player
            {
                decision = "jump";
            }
            else // above the players.
            {
                decision = "down";
            }

            if(distance() >= 350)// the gap between bot & player.
            {
                GetCloser();
            }
            else// stops the bot's movement.
            {
                dirToGo = "";
            }
            
        }

        /// <summary>
        /// overrides the Move func in Sprite class.
        /// </summary>
        protected override void Move()
        {
            BotDecisions();
            if (dirToGo.Equals("right"))
            {
                Velocity.X = Speed;// moves right.
                dir = "right";
                keyboardString = "right";
            }
            else if (dirToGo.Equals("left"))
            {
                Velocity.X = -Speed;// moves left.
                dir = "left";
                keyboardString = "left";
            }
            else Velocity.X = 0f;//stands still.

            #region Jumping

            if (decision.Equals("jump") && hasJumped == false)//jumping scenario.
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

            if (decision.Equals("throw"))// throwing scenario
            {
                toThrow = true;
                keyboardString = "throw";
            }

            if (decision.Equals("down"))
            {
                keyboardString = "down";
            }
        }

        /// <summary>
        /// overrides the SetAnimations in Sprite class.
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
            
            if (dirToGo.Equals("right"))
                _animationManager.Play(_animations["runningR"]);

            else if (dirToGo.Equals("left"))
                _animationManager.Play(_animations["runningL"]);


            else if (hasJumped)// jumping animation
            {
                if (Velocity.X > 0)
                    _animationManager.Play(_animations["jumpingR"]);
                else if (Velocity.X < 0) _animationManager.Play(_animations["jumpingL"]);
            }
        }

        /// <summary>
        /// overrides the KnifeStates in Sprite class.
        /// </summary>
        protected override void KnifeStates(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(timer >= 0.85)// sets difficulty of game.
            {
                if (decision.Equals("throw"))
                {
                    addKnife();
                    toThrow = false;
                    Game1._throwing.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                }
                decision = "";
                timer = 0;
            }
            foreach (var knife in knivesSet)
            {
                knife.UpdateKnife();
            }
        }

        public override void Update(GameTime gameTime, Sprite enemy)
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
    }
}
