using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Snow
{
    class SnowGenerator
    {
        Texture2D texture;

        float spawnWidth;
        float density;

        List<Snowfall> snowFall = new List<Snowfall>();

        float timer;

        Random rand1, rand2;

        /// <summary>
        /// SnowGenerator's constructor.
        /// </summary>
        public SnowGenerator(Texture2D newTexture, float newSpawnWidth, float newDensity)
        {
            texture = newTexture;
            spawnWidth = newSpawnWidth;
            density = newDensity;
            rand1 = new Random();
            rand2 = new Random();
        }

        /// <summary>
        /// adds to the list a snow particle.
        /// </summary>
        public void CreateSnowParticle()
        {
            snowFall.Add(new Snowfall(texture, new Vector2(
                -50 + (float)rand1.NextDouble() * spawnWidth, 0),
                new Vector2(1, rand2.Next(5, 8))));
        }

        /// <summary>
        /// updates the positions of the snowfall and 
        /// delete the ones who reach the bottom of screen
        /// </summary>
        public void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (timer > 0)
            {
                timer -= 1f / density;
                CreateSnowParticle();
            }
            for (int i = 0; i < snowFall.Count; i++)
            {
                snowFall[i].Update();

                if (snowFall[i].Position.Y > graphics.Viewport.Height)
                {
                    snowFall.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// draws the snowfall.
        /// </summary>
        public void Draw(SpriteBatch sb)
        {
            foreach (Snowfall snow in snowFall)
                snow.Draw(sb);
        }
    }
}
