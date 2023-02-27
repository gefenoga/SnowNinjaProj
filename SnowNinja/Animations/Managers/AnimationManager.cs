using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnowNinja.Animations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Animations.Managers
{
    public class AnimationManager
    {
        private Animation _animation;// the current animation that is running.

        private float _timer;// helps to manage the animation speed with the variable FrameSpeed

        public float scale = 0.35f;// the scale for the Draw function.

        public Vector2 Position { get; set; }// position of the animation.

        /// <summary>
        /// The constructor of the AnimationManager class.
        /// </summary>

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        /// <summary>
        /// gets an animation and creates the animation.
        /// </summary>

        public void Play(Animation animation)
        {
            if (_animation == animation)
                return;

            _animation = animation;

            _animation.CurrentFrame = 0;

            _timer = 0;
        }


        /// <summary>
        /// a function that stops the animation by reseting the frame index and the timer. 
        /// </summary>

        public void Stop()
        {
            _timer = 0;

            _animation.CurrentFrame = 0;
        }


        /// <summary>
        /// creates the frame speed and continues to the next frame
        /// every time the timer has reached the max. 
        /// </summary>

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0;

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0;
            }
        }

        /// <summary>
        /// draws the current frame using SpriteBatch- 
        /// cuts from the image the exact frame in order to create the animation.
        /// </summary>

        public void Draw(SpriteBatch sb, Color PlayerColor)
        {
            sb.Draw(_animation.Texture, Position,
                new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                0, _animation.FrameWidth, _animation.FrameHeight), PlayerColor, 0,
                new Vector2(_animation.FrameWidth / 2, _animation.FrameHeight / 2), scale, SpriteEffects.None, 1);

            new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                0, _animation.FrameWidth, _animation.FrameHeight);

        }
    }
}
