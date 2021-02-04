//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GES.Telemetry.Definitions {
    using System;
    using MTI.Core;
    using GES.Communications;
    using CFS.DataStructures;
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "GES.Telemetry.Definitions"})]
    public partial class CFSTelemetryDefinition : GES.Communications.CCSDSPacketDefinition {
        
        private CFS.DataStructures.TelemetrySecondaryHeader _TelemetrySecondaryHeader;
        
        public CFSTelemetryDefinition() {
            // 
            // Initialize base members
            // 
            this.InitializeBaseMembers();
            // 
            // Construct _TelemetrySecondaryHeader
            // 
            this._TelemetrySecondaryHeader = CFSTelemetryDefinition.CreateTelemetrySecondaryHeader();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        [MTI.Core.RequireAttribute(Cut=true, Alias="CCSDSPacketDefinition.ApplicationData")]
        public virtual object ApplicationData {
            get {
                return base.ApplicationData;
            }
            set {
                base.ApplicationData = value;
                this.OnPropertyChanged("CCSDSPacketDefinition.ApplicationData");
            }
        }
        
        public virtual CFS.DataStructures.TelemetrySecondaryHeader TelemetrySecondaryHeader {
            get {
                return this._TelemetrySecondaryHeader;
            }
            set {
                this._TelemetrySecondaryHeader = value;
                this.OnPropertyChanged("TelemetrySecondaryHeader");
            }
        }
        
        private void InitializeBaseMembers() {
            // 
            // Creates a new instance of the GES.Communications.CCSDSPacketDefinition class CCSDSPacketDefinition.
            // 
            GES.Communications.DataAttributes dataattributes3 = new GES.Communications.DataAttributes();
            dataattributes3.Name = "Seconds";
            dataattributes3.Static = false;
            dataattributes3.DefaultValue = "0";
            dataattributes3.MinValue = "0";
            dataattributes3.MaxValue = "4294967295";
            dataattributes3.DataType = "UInt32";
            dataattributes3.BitLength = 32;
            dataattributes3.Endian = GES.Communications.DataAttributes.EndianType.LittleEndian;
            dataattributes3.Equation = "";
            dataattributes3.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes3.LocalPathName = "TelemetrySecondaryHeader.Seconds";
            GES.Communications.DataAttributes dataattributes4 = new GES.Communications.DataAttributes();
            dataattributes4.Name = "Subseconds";
            dataattributes4.Static = false;
            dataattributes4.DefaultValue = "0";
            dataattributes4.MinValue = "0";
            dataattributes4.MaxValue = "65535";
            dataattributes4.DataType = "UInt16";
            dataattributes4.BitLength = 16;
            dataattributes4.Endian = GES.Communications.DataAttributes.EndianType.LittleEndian;
            dataattributes4.Equation = "";
            dataattributes4.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes4.LocalPathName = "TelemetrySecondaryHeader.Subseconds";
            base.SecondaryHeaderAttributes = new GES.Communications.DataAttributes[] {
                    ((GES.Communications.DataAttributes)(dataattributes3)),
                    ((GES.Communications.DataAttributes)(dataattributes4))};
        }
        
        public virtual void Connect() {
            // 
            // Connects subcomponents into graph 
            // 
            this.Connect("base.SecondaryHeader", "TelemetrySecondaryHeader");
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}