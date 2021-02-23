namespace AscensionServer.Lite

{
    /// <summary>
    /// Interface of a message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the action.
        /// </summary>
        byte Action { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        object Message { get; }
    }
}


