//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CFS.Commands {
    using System;
    using MTI.Core;
    using GES.Communications;
    using GES.Commands.Definitions;
    using CFS.DataStructures;
    
    
    public partial class NETIFInitSocketCommand {
        
        public static CFS.DataStructures.NETIFInitSocketCommandData CreateNETIFInitSocketCommandData() {
            // 
            // Creates a new instance of the CFS.DataStructures.NETIFInitSocketCommandData class NETIFInitSocketCommandData.
            // 
            CFS.DataStructures.NETIFInitSocketCommandData NETIFInitSocketCommandData = new CFS.DataStructures.NETIFInitSocketCommandData();
            NETIFInitSocketCommandData.ServerPort = 1236;
            NETIFInitSocketCommandData.IPAddress = "192.168.001.203";
            return NETIFInitSocketCommandData;
        }
    }
}