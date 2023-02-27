using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.StatesMng.States
{
    public abstract class State
    {
        public bool gameActivation = false;// for Game1
        public bool returnToMenu = false;// for Game1
        public bool guideActivation = false;// for Game1

        #region fields

        protected ContentManager _content;

        protected GraphicsDevice _graphicsDevice;

        protected Game1 _game;

        #endregion

        #region Methods 
        
        /// <summary>
        /// State's Constructor
        /// </summary>

        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;

            _graphicsDevice = graphicsDevice;

            _content = content;
        }

        /// <summary>
        /// regular update function
        /// </summary>

        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// regular draw function
        /// </summary>

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        #endregion
    }
}
