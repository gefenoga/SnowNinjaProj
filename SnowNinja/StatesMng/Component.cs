using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.StatesMng
{
    public abstract class Component
    {
        /// <summary>
        /// basic update function.
        /// </summary>
        public abstract void Update(GameTime gameTime);
        
        /// <summary>
        /// basic draw function.
        /// </summary>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        
    }
}
