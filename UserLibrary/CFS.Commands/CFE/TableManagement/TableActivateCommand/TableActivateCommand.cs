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
    public partial class TableActivateCommand : GES.Communications.CCSDSCommand {
        
        private GES.Commands.Definitions.CFSCommandDefinition _CFSCommandDefinition;
        
        private string _TableName;
        
        public TableActivateCommand() {
            // 
            // Initialize base members
            // 
            this.InitializeBaseMembers();
            // 
            // Construct _CFSCommandDefinition
            // 
            this._CFSCommandDefinition = TableActivateCommand.CreateCFSCommandDefinition();
            // 
            // Construct _TableName
            // 
            this._TableName = TableActivateCommand.CreateTableName();
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
        public virtual string TableName {
            get {
                return this._TableName;
            }
            set {
                this._TableName = value;
                this.OnPropertyChanged("TableName");
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
            this.Connect("CFSCommandDefinition.ApplicationData", "TableName");
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}
