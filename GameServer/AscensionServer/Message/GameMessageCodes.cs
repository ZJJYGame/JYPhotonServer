using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// GameMessagCodes define the type of a "LiteGame" Message, the meaning and its content.
    /// Messages are used to communicate async with rooms and games.
    /// </summary>
    public enum GameMessageCodes : byte
    {
        /// <summary>
        /// Message is an operatzion.
        /// </summary>
        Operation = 0,

        /// <summary>
        /// Message to remove peer from game.
        /// </summary>
        RemovePeerFromGame = 1,
    }
}
