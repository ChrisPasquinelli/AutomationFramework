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
    using CFS.Commands;
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "CFS.Commands"})]
    public partial class StartATSCommand : CFS.Commands.CFSCommand {
        
        private ushort _AtsId;
        
        public StartATSCommand() {
            // 
            // Initialize base members
            // 
            this.InitializeBaseMembers();
            // 
            // Construct _AtsId
            // 
            this._AtsId = StartATSCommand.CreateAtsId();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        public new virtual object ApplicationData {
            get {
                return base.ApplicationData;
            }
            set {
                base.ApplicationData = value;
                this.OnPropertyChanged("CFSCommand.ApplicationData");
            }
        }
        
        public virtual ushort AtsId {
            get {
                return this._AtsId;
            }
            set {
                this._AtsId = value;
                this.OnPropertyChanged("AtsId");
            }
        }
        
        private void InitializeBaseMembers() {
            // 
            // Creates a new instance of the CFS.Commands.CFSCommand class CFSCommand.
            // 
            base.CCSDSPacketDefinition.PrimaryHeader.ApplicationId = 6313;
            GES.Communications.DataAttributes dataattributes2 = new GES.Communications.DataAttributes();
            dataattributes2.Name = "AtsId";
            dataattributes2.Static = false;
            dataattributes2.DefaultValue = "0";
            dataattributes2.MinValue = "0";
            dataattributes2.MaxValue = "65535";
            dataattributes2.DataType = "UInt16";
            dataattributes2.BitLength = 16;
            dataattributes2.Endian = GES.Communications.DataAttributes.EndianType.LittleEndian;
            dataattributes2.Equation = "";
            dataattributes2.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes2.LocalPathName = "AtsId";
            base.CCSDSPacketDefinition.ApplicationDataAttributes = new GES.Communications.DataAttributes[] {
                    ((GES.Communications.DataAttributes)(dataattributes2))};
            base.CommandSecondaryHeader.FunctionCode = 2;
            base.CCSDSPacketDefinition.PrimaryHeader.Version = 0;
            base.CCSDSPacketDefinition.PrimaryHeader.Type = 1;
            base.CCSDSPacketDefinition.PrimaryHeader.SecondaryFlag = 1;
            base.CCSDSPacketDefinition.PrimaryHeader.ApplicationId = 6313;
            base.CCSDSPacketDefinition.PrimaryHeader.Grouping = 2;
            base.CCSDSPacketDefinition.PrimaryHeader.SequenceCount = 0;
            base.CCSDSPacketDefinition.PrimaryHeader.PacketDataLength = 3;
        }
        
        public virtual void Connect() {
            // 
            // Connects subcomponents into graph 
            // 
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}