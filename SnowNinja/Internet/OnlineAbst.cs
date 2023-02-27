using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnowNinja.Animations.Models;
using SnowNinja.Animations.Sprites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnowNinja.Internet
{
    public delegate void OnConnectionHandler(); // creates event.

    abstract class OnlineAbst
    {
        #region properties
        public event OnConnectionHandler connectHandler; // raises on connection.

        protected BinaryReader reader;// translates the binary code into useable information.
        protected BinaryWriter writer; // transforms the information to binary code.

        protected int port;

        protected Thread thread; // secondary thread to manage the communications.

        protected TcpClient client; // gets the bit stream. 
        
        public Sprite serverPlayer, clientPlayer; // the two players in the game. 

        protected Texture2D healthTex; // for the sprite constructor.

        
        #endregion

        #region Methods

        /// <summary>
        /// raises when the func is called 
        /// </summary>
        protected void ConnectionEventTrigger()
        {
            connectHandler?.Invoke();
        }

        /// <summary>
        /// called when the connection starts.
        /// </summary>
        public void Initialize(Dictionary<string, Animation> animations, ContentManager content)
        {
            InitPlayers(animations, content);
            StartCom();
        }
        
        /// <summary>
        /// gets the bit stream and sets the character by it.
        /// </summary>
        protected void ReadAndUpdateCharacter(Sprite character, Sprite enemy)
        {
            character.Velocity.X = reader.ReadSingle();
            character.Velocity.Y = reader.ReadSingle();
            character.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            character.hasJumped = reader.ReadBoolean();
            character.toThrow = reader.ReadBoolean();
            MinorUpdates(character);
            character.dir = reader.ReadString();
            character.keyboardString = reader.ReadString();
            enemy.health.rectangle.Width = reader.ReadInt32();
        }

        /// <summary>
        /// updates the knives.
        /// </summary>
        private void MinorUpdates(Sprite character)
        {
            if (character.toThrow)
            {
                character.addKnife();
            }
            foreach (var knife in character.knivesSet)
            {
                knife.UpdateKnife();
            }
        }

        /// <summary>
        /// sends a bit stream of the variables needed in connection.
        /// </summary>
        protected void WriteCharacterData(Sprite character, Sprite enemy)
        {
            writer.Write(character.Velocity.X);
            writer.Write(character.Velocity.Y);
            writer.Write(character.Position.X);
            writer.Write(character.Position.Y);
            writer.Write(character.hasJumped);
            writer.Write(character.toThrow);
            writer.Write(character.dir);
            writer.Write(character.keyboardString);
            writer.Write(enemy.health.rectangle.Width);
        }

        /// <summary>
        /// connection managments in derived classes.
        /// </summary>
        protected abstract void SocketThread();

        /// <summary>
        /// creates the players in derived classes.
        /// </summary>
        protected abstract void InitPlayers(Dictionary<string, Animation> animations, ContentManager content);

        /// <summary>
        /// creates the thread.
        /// </summary>
        public void StartCom()
        {
            thread = new Thread(new ThreadStart(SocketThread));
            thread.IsBackground = true;
            thread.Start();
        }

        #endregion

    }
}
