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
    using CFS.Commands;
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "CFS.Commands"})]
    public partial class CommandSequence3 : GES.Communications.CommandSequence {
        
        private CFS.Commands.TelemetryOutputEnableCommand _TelemetryOutputEnableCommand1;
        
        private CFS.Commands.TelemetryOutputEnableCommand _TelemetryOutputEnableCommand2;
        
        private CFS.Commands.TelemetryOutputEnableCommand _TelemetryOutputEnableCommand3;
        
        public CommandSequence3() {
            // 
            // Initialize base members
            // 
            this.InitializeBaseMembers();
            // 
            // Construct _TelemetryOutputEnableCommand1
            // 
            this._TelemetryOutputEnableCommand1 = CommandSequence3.CreateTelemetryOutputEnableCommand1();
            // 
            // Construct _TelemetryOutputEnableCommand2
            // 
            this._TelemetryOutputEnableCommand2 = CommandSequence3.CreateTelemetryOutputEnableCommand2();
            // 
            // Construct _TelemetryOutputEnableCommand3
            // 
            this._TelemetryOutputEnableCommand3 = CommandSequence3.CreateTelemetryOutputEnableCommand3();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        public new virtual GES.Communications.ICommand[] Commands {
            get {
                return base.Commands;
            }
            set {
                base.Commands = value;
                this.OnPropertyChanged("CommandSequence.Commands");
            }
        }
        
        public virtual CFS.Commands.TelemetryOutputEnableCommand TelemetryOutputEnableCommand1 {
            get {
                return this._TelemetryOutputEnableCommand1;
            }
            set {
                this._TelemetryOutputEnableCommand1 = value;
                this.OnPropertyChanged("TelemetryOutputEnableCommand1");
            }
        }
        
        public virtual CFS.Commands.TelemetryOutputEnableCommand TelemetryOutputEnableCommand2 {
            get {
                return this._TelemetryOutputEnableCommand2;
            }
            set {
                this._TelemetryOutputEnableCommand2 = value;
                this.OnPropertyChanged("TelemetryOutputEnableCommand2");
            }
        }
        
        public virtual CFS.Commands.TelemetryOutputEnableCommand TelemetryOutputEnableCommand3 {
            get {
                return this._TelemetryOutputEnableCommand3;
            }
            set {
                this._TelemetryOutputEnableCommand3 = value;
                this.OnPropertyChanged("TelemetryOutputEnableCommand3");
            }
        }
        
        private void InitializeBaseMembers() {
            // 
            // Creates a new instance of the GES.Communications.CommandSequence class CommandSequence.
            // 
        }
        
        public virtual void Connect() {
            // 
            // Connects subcomponents into graph 
            // 
            this.Connect("base.Commands", "TelemetryOutputEnableCommand1");
            this.Connect("base.Commands", "TelemetryOutputEnableCommand3");
            this.Connect("base.Commands", "TelemetryOutputEnableCommand2");
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}
