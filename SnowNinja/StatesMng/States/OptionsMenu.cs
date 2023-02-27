using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SnowNinja.StatesMng.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.StatesMng.States
{
    class OptionsMenu : State
    {
        private List<Component> _components;// contains the buttons

        /// <summary>
        /// OptionsMenu's constructor.
        /// </summary>

        public OptionsMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/wooden button");

            var positionX = (Game1.ScreenWidth / 2) - (buttonTexture.Width / 2);

            var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

            var ContinueButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(positionX, 200),
                Text = "Continue",
            };
            ContinueButton.Click += ContinueButton_Click;// return back to game

            var BackToMainButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(positionX, 300),
                Text = "Main Menu",
            };
            BackToMainButton.Click += BackToMainButton_Click;// returns to main menu.

            _components = new List<Component>()//filling up the list.
            {
                ContinueButton,
                BackToMainButton,
            };

        }
        
        /// <summary>
        /// updating the buttons.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        /// <summary>
        /// drawing the buttons.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }
        
        /// <summary>
        /// managing Game1 booleans and playing again the opening music.
        /// </summary>
        private void BackToMainButton_Click(object sender, EventArgs e)
        {
            returnToMenu = true;
            Game1.activeGame = true;
            MediaPlayer.Play(Game1._menuSong);
        }

        /// <summary>
        /// managing Game1 booleans and playing again the duel music.
        /// </summary>
        private void ContinueButton_Click(object sender, EventArgs e)
        {
            Game1.escPress = false;
            Game1.activeGame = true;
            MediaPlayer.Play(Game1._duelSong);
        }


    }
}
