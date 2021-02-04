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
        
        public static GES.Communications.CCSDSPacketDefinition CreateCCSDSPacketDefinition() {
            // 
            // Creates a new instance of the GES.Communications.CCSDSPacketDefinition class CCSDSPacketDefinition.
            // 
            GES.Communications.CCSDSPacketDefinition CCSDSPacketDefinition = new GES.Communications.CCSDSPacketDefinition();
            GES.Communications.DataAttributes dataattributes1 = new GES.Communications.DataAttributes();
            dataattributes1.Name = "Checksum";
            dataattributes1.Static = false;
            dataattributes1.DefaultValue = "0";
            dataattributes1.MinValue = "0";
            dataattributes1.MaxValue = "255";
            dataattributes1.DataType = "Byte";
            dataattributes1.BitLength = 8;
            dataattributes1.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes1.Equation = "";
            dataattributes1.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes1.LocalPathName = "CommandSecondaryHeader.Checksum";
            GES.Communications.DataAttributes dataattributes2 = new GES.Communications.DataAttributes();
            dataattributes2.Name = "FunctionCode";
            dataattributes2.Static = false;
            dataattributes2.DefaultValue = "0";
            dataattributes2.MinValue = "0";
            dataattributes2.MaxValue = "255";
            dataattributes2.DataType = "Byte";
            dataattributes2.BitLength = 8;
            dataattributes2.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes2.Equation = "";
            dataattributes2.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes2.LocalPathName = "CommandSecondaryHeader.FunctionCode";
            CCSDSPacketDefinition.SecondaryHeaderAttributes = new GES.Communications.DataAttributes[] {
                    ((GES.Communications.DataAttributes)(dataattributes1)),
                    ((GES.Communications.DataAttributes)(dataattributes2))};
            return CCSDSPacketDefinition;
        }
    }
}
