//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CFS.Tasks {
    using System;
    using MTI.Core;
    using GES.Communications;
    using CFS.Commands;
    using CFS.Telemetry;
    
    
    public partial class CommandUplink {
        
        public static GES.Communications.SocketTransceiver CreateTransmitter() {
            // 
            // Creates a new instance of the GES.Communications.SocketTransceiver class Transmitter.
            // 
            GES.Communications.SocketTransceiver Transmitter = new GES.Communications.SocketTransceiver();
            Transmitter.AddressOrHostName = "192.168.1.201";
            Transmitter.Protocol = System.Net.Sockets.ProtocolType.Udp;
            Transmitter.Port = 1234;
            Transmitter.Verbose = true;
            return Transmitter;
        }
    }
}
