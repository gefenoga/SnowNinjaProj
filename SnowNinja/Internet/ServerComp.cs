using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SnowNinja.Animations.Models;
using SnowNinja.Animations.Sprites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Internet
{
    class ServerComp : OnlineAbst
    {
        /// <summary>
        /// ServerComp's constructor.
        /// </summary>
        public ServerComp(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// creates the characters- player and enemy.
        /// </summary>
        protected override void InitPlayers(Dictionary<string, Animation> animations, ContentManager content)
        {
            healthTex = content.Load<Texture2D>("Objects/HealthBarInside");

            serverPlayer = new Sprite(animations)
            {
                PlayerType = "player",
                health = new HealthBar(healthTex, new Vector2(119, 101),
                      new Rectangle(0, 0, healthTex.Width - 20, healthTex.Height - 4)),

                knivesSet = new List<Knife>(),
                Speed = 5f,
                Position = new Vector2(50, 700),
                scale = 0.35f,
                knifeScale = 0.15f,
                origin = new Vector2(Game1.anyPlayerTex.Width / 20, Game1.anyPlayerTex.Height / 2),
                texHeight = Game1.anyPlayerTex.Height * 0.35f,
                color = Color.White,
                Input = new Input()
                {
                    Down = Keys.S,
                    Jump = Keys.Space,
                    Left = Keys.A,
                    Right = Keys.D,
                    Throw = Keys.E,
                }
            };

            clientPlayer = new Sprite(animations)
            {
                PlayerType = "enemy",
                health = new HealthBar(healthTex, new Vector2(1019, 101),
                    new Rectangle(0, 0, healthTex.Width - 20, healthTex.Height - 4)),
                Position = new Vector2(1200, 700),
                knivesSet = new List<Knife>(),
                Speed = 5f,
                scale = 0.35f,
                knifeScale = 0.15f,
                origin = new Vector2(Game1.anyPlayerTex.Width / 20, Game1.anyPlayerTex.Height / 2),
                texHeight = Game1.anyPlayerTex.Height * 0.35f,
                color = new Color(64, 13, 13),
            };
        }

        /// <summary>
        /// creates the connection and receives data from clients in a secondary thread.
        /// </summary>
        protected override void SocketThread()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            client = listener.AcceptTcpClient();

            reader = new BinaryReader(client.GetStream());
            writer = new BinaryWriter(client.GetStream());

            ConnectionEventTrigger();

            while (true)
            {
                ReadAndUpdateCharacter(clientPlayer, serverPlayer);
                WriteCharacterData(serverPlayer, clientPlayer);
            }
        }
    }
}
