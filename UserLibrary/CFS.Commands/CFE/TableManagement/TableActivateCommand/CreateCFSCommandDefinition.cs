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
    
    
    public partial class TableActivateCommand {
        
        public static GES.Commands.Definitions.CFSCommandDefinition CreateCFSCommandDefinition() {
            // 
            // Creates a new instance of the GES.Commands.Definitions.CFSCommandDefinition class CFSCommandDefinition.
            // 
            GES.Commands.Definitions.CFSCommandDefinition CFSCommandDefinition = new GES.Commands.Definitions.CFSCommandDefinition();
            CFSCommandDefinition.CommandSecondaryHeader.FunctionCode = 5;
            CFSCommandDefinition.PrimaryHeader.Version = 0;
            CFSCommandDefinition.PrimaryHeader.Type = 1;
            CFSCommandDefinition.PrimaryHeader.SecondaryFlag = 1;
            CFSCommandDefinition.PrimaryHeader.ApplicationId = 6148;
            CFSCommandDefinition.PrimaryHeader.Grouping = 2;
            CFSCommandDefinition.PrimaryHeader.SequenceCount = 0;
            CFSCommandDefinition.PrimaryHeader.PacketDataLength = 41;
            GES.Communications.DataAttributes dataattributes2 = new GES.Communications.DataAttributes();
            dataattributes2.Name = "TableName";
            dataattributes2.Static = false;
            dataattributes2.DefaultValue = "";
            dataattributes2.MinValue = "";
            dataattributes2.MaxValue = "";
            dataattributes2.DataType = "String";
            dataattributes2.BitLength = 320;
            dataattributes2.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes2.Equation = "";
            dataattributes2.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes2.LocalPathName = "TableName";
            CFSCommandDefinition.ApplicationDataAttributes = new GES.Communications.DataAttributes[] {
                    ((GES.Communications.DataAttributes)(dataattributes2))};
            return CFSCommandDefinition;
        }
    }
}
