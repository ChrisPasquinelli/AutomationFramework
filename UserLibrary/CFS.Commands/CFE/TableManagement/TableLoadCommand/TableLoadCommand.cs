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
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "CFS.Commands"})]
    public partial class TableLoadCommand : GES.Communications.CCSDSCommand {
        
        private GES.Commands.Definitions.CFSCommandDefinition _CFSCommandDefinition;
        
        private string _Filename;
        
        public TableLoadCommand() {
            // 
            // Initialize base members
            // 
            this.InitializeBaseMembers();
            // 
            // Construct _CFSCommandDefinition
            // 
            this._CFSCommandDefinition = TableLoadCommand.CreateCFSCommandDefinition();
            // 
            // Construct _Filename
            // 
            this._Filename = TableLoadCommand.CreateFilename();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        public new virtual GES.Communications.CCSDSPacketDefinition PacketDefinition {
            get {
                return base.PacketDefinition;
            }
            set {
                base.PacketDefinition = value;
                this.OnPropertyChanged("CCSDSCommand.PacketDefinition");
            }
        }
        
        public virtual GES.Commands.Definitions.CFSCommandDefinition CFSCommandDefinition {
            get {
                return this._CFSCommandDefinition;
            }
            set {
                this._CFSCommandDefinition = value;
                this.OnPropertyChanged("CFSCommandDefinition");
            }
        }
        
        [MTI.Core.RequireAttribute(Cut=true)]
        public virtual string Filename {
            get {
                return this._Filename;
            }
            set {
                this._Filename = value;
                this.OnPropertyChanged("Filename");
            }
        }
        
        private void InitializeBaseMembers() {
            // 
            // Creates a new instance of the GES.Communications.CCSDSCommand class CCSDSCommand.
            // 
        }
        
        public virtual void Connect() {
            // 
            // Connects subcomponents into graph 
            // 
            this.Connect("base.PacketDefinition", "CFSCommandDefinition");
            this.Connect("CFSCommandDefinition.ApplicationData", "Filename");
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}