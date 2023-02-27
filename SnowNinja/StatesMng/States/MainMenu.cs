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
    class MainMenu : State
    {
        private List<Component> _components;// contains the buttons

        /// <summary>
        /// MenuState's constructor.
        /// </summary>
        public MainMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            #region Creating the buttons
            var buttonTexture = _content.Load<Texture2D>("Controls/blueButton");

            var positionX = (Game1.ScreenWidth / 2) - (buttonTexture.Width / 2);

            var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

            var singleplayerButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(positionX, 200),
                Text = "Singleplayer",
            };

            singleplayerButton.Click += singlePlayerButton_Click;// adding a function to the event of the particular button.

            var multiplayerButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(positionX, 300),
                Text = "Multiplayer",
            };

            multiplayerButton.Click += multiplayerButton_Click;// adding a function to the event of the particular button.

            var guideButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(positionX, 400),
                Text = "How to Play?",
            };

            guideButton.Click += GuideButton_Click;// adding a function to the event of the particular button.

            var exitButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(positionX, 500),
                Text = "Exit"
            };
            exitButton.Click += ExitButton_Click;// adding a function to the event of the particular button.

            _components = new List<Component>()// filling the button list
            {
                singleplayerButton,
                multiplayerButton,
                guideButton,
                exitButton,

            };
            #endregion
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

        private void ExitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();// closing the game.
        }

        private void GuideButton_Click(object sender, EventArgs e)
        {
            guideActivation = true;// opening the guide in Game1.
        }

        /// <summary>
        /// changing the state to multiplayer.
        /// </summary>

        private void multiplayerButton_Click(object sender, EventArgs e)
        {
            MediaPlayer.Stop();
            _game.ChangeState(new MultiPlayer(_game, _graphicsDevice, _content));
        }

        /// <summary>
        /// audio and boolean managment in Game1. 
        /// </summary>

        private void singlePlayerButton_Click(object sender, EventArgs e)
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(Game1._duelSong);
            gameActivation = true;
            Game1.activeGame = true;
            Game1.escPress = false;
        }
    }
}
