namespace AscensionServer.Lite

{
    using System.Collections;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This class implements the Join event.
    /// </summary>
    public class JoinEvent : LiteEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinEvent"/> class.
        /// </summary>
        /// <param name="actorNr">
        /// The sender actor nr.
        /// </param>
        /// <param name="actors">
        /// The actors in the game.
        /// </param>
        public JoinEvent(int actorNr, int[] actors)
            : base(actorNr)
        {
            //this.Code = (byte)EventCode.Join;
            this.Actors = actors;
        }

        /// <summary>
        /// Gets or sets the actor properties of the joined actor.
        /// </summary>
        //[DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable ActorProperties { get; set; }

        /// <summary>
        /// Gets or sets the actors in the game.
        /// </summary>
        //[DataMember(Code = (byte)ParameterKey.Actors)]
        public int[] Actors { get; set; }
    }
}