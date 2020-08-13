namespace AscensionServer.Lite

{
    /// <summary>
    ///   Defines the operation codes used by the Lite application.
    ///   These codes match events (in parts).
    /// </summary>
    public enum NewOperationCode : byte
    {
        /// <summary>
        ///   The operation code for the <see cref="JoinRequest">join</see> operation.
        /// </summary>
        Join = 255, 

        /// <summary>
        ///   Operation code for the <see cref="LeaveRequest">leave</see> operation.
        /// </summary>
        Leave = 254, 

        /// <summary>
        ///   Operation code for the <see cref="RaiseEventRequest">raise event</see> operation.
        /// </summary>
        RaiseEvent = 253, 

        /// <summary>
        ///   Operation code for the <see cref="SetPropertiesRequest">set properties</see> operation.
        /// </summary>
        SetProperties = 252, 

        /// <summary>
        ///   Operation code for the <see cref="GetPropertiesRequest">get properties</see> operation.
        /// </summary>
        GetProperties = 251, 
        

        /// <summary>
        ///   Operation code for the ping operation.
        /// </summary>
        Ping = 249,

        ChangeGroups = 248,
    }
}