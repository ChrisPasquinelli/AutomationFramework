//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GES.Commands.Definitions {
    using System;
    using MTI.Core;
    using GES.Communications;
    using CFS.DataStructures;
    
    
    public partial class CFSCommandDefinition {
        
        public static GES.Communications.XORChecksum CreateXORChecksum() {
            // 
            // Creates a new instance of the GES.Communications.XORChecksum class XORChecksum.
            // 
            GES.Communications.XORChecksum XORChecksum = new GES.Communications.XORChecksum();
            XORChecksum.ChecksumIndex = 6;
            XORChecksum.ByteOrder = GES.Communications.ByteOrder.Sequential;
            return XORChecksum;
        }
    }
}
