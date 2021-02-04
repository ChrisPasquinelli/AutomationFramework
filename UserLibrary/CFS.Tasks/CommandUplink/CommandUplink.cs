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
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "CFS.Tasks"})]
    public partial class CommandUplink : MTI.Core.Component {
        
        private bool _Active;
        
        private GES.Communications.SocketTransceiver _Transmitter;
        
        private GES.Communications.CommandTask _CommandTask;
        
        private CFS.Commands.TelemetryOutputEnableCommand _TelemetryOutputEnableCommand;
        
        private CFS.Commands.NETIFInitSocketCommand _NETIFInitSocketCommand;
        
        private CFS.Commands.TableLoadCommand _TableLoadCommand;
        
        private CFS.Commands.TableValidateCommand _TableValidateCommand;
        
        private CFS.Commands.TableActivateCommand _TableActivateCommand;
        
        private CFS.Commands.StartATSCommand _StartATSCommand;
        
        private GES.Communications.SocketTransceiver _Receiver;
        
        private GES.Communications.TelemetryTask _TelemetryTask;
        
        private CFS.Telemetry.EventServicesHousekeepingTelemetery _EventServicesHousekeepingTelemetery;
        
        private CFS.Telemetry.ExecutiveServicesHousekeepingTelemetery _ExecutiveServicesHousekeepingTelemetery;
        
        public CommandUplink() {
            // 
            // Construct _Active
            // 
            this._Active = CommandUplink.CreateActive();
            // 
            // Construct _Transmitter
            // 
            this._Transmitter = CommandUplink.CreateTransmitter();
            // 
            // Construct _CommandTask
            // 
            this._CommandTask = CommandUplink.CreateCommandTask();
            // 
            // Construct _TelemetryOutputEnableCommand
            // 
            this._TelemetryOutputEnableCommand = CommandUplink.CreateTelemetryOutputEnableCommand();
            // 
            // Construct _NETIFInitSocketCommand
            // 
            this._NETIFInitSocketCommand = CommandUplink.CreateNETIFInitSocketCommand();
            // 
            // Construct _TableLoadCommand
            // 
            this._TableLoadCommand = CommandUplink.CreateTableLoadCommand();
            // 
            // Construct _TableValidateCommand
            // 
            this._TableValidateCommand = CommandUplink.CreateTableValidateCommand();
            // 
            // Construct _TableActivateCommand
            // 
            this._TableActivateCommand = CommandUplink.CreateTableActivateCommand();
            // 
            // Construct _StartATSCommand
            // 
            this._StartATSCommand = CommandUplink.CreateStartATSCommand();
            // 
            // Construct _Receiver
            // 
            this._Receiver = CommandUplink.CreateReceiver();
            // 
            // Construct _TelemetryTask
            // 
            this._TelemetryTask = CommandUplink.CreateTelemetryTask();
            // 
            // Construct _EventServicesHousekeepingTelemetery
            // 
            this._EventServicesHousekeepingTelemetery = CommandUplink.CreateEventServicesHousekeepingTelemetery();
            // 
            // Construct _ExecutiveServicesHousekeepingTelemetery
            // 
            this._ExecutiveServicesHousekeepingTelemetery = CommandUplink.CreateExecutiveServicesHousekeepingTelemetery();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        [MTI.Core.RequireAttribute(Cut=true)]
        public virtual bool Active {
            get {
                return this._Active;
            }
            set {
                this._Active = value;
                this.OnPropertyChanged("Active");
            }
        }
        
        public virtual GES.Communications.SocketTransceiver Transmitter {
            get {
                return this._Transmitter;
            }
            set {
                this._Transmitter = value;
                this.OnPropertyChanged("Transmitter");
            }
        }
        
        public virtual GES.Communications.CommandTask CommandTask {
            get {
                return this._CommandTask;
            }
            set {
                this._CommandTask = value;
                this.OnPropertyChanged("CommandTask");
            }
        }
        
        public virtual CFS.Commands.TelemetryOutputEnableCommand TelemetryOutputEnableCommand {
            get {
                return this._TelemetryOutputEnableCommand;
            }
            set {
                this._TelemetryOutputEnableCommand = value;
                this.OnPropertyChanged("TelemetryOutputEnableCommand");
            }
        }
        
        public virtual CFS.Commands.NETIFInitSocketCommand NETIFInitSocketCommand {
            get {
                return this._NETIFInitSocketCommand;
            }
            set {
                this._NETIFInitSocketCommand = value;
                this.OnPropertyChanged("NETIFInitSocketCommand");
            }
        }
        
        public virtual CFS.Commands.TableLoadCommand TableLoadCommand {
            get {
                return this._TableLoadCommand;
            }
            set {
                this._TableLoadCommand = value;
                this.OnPropertyChanged("TableLoadCommand");
            }
        }
        
        public virtual CFS.Commands.TableValidateCommand TableValidateCommand {
            get {
                return this._TableValidateCommand;
            }
            set {
                this._TableValidateCommand = value;
                this.OnPropertyChanged("TableValidateCommand");
            }
        }
        
        public virtual CFS.Commands.TableActivateCommand TableActivateCommand {
            get {
                return this._TableActivateCommand;
            }
            set {
                this._TableActivateCommand = value;
                this.OnPropertyChanged("TableActivateCommand");
            }
        }
        
        public virtual CFS.Commands.StartATSCommand StartATSCommand {
            get {
                return this._StartATSCommand;
            }
            set {
                this._StartATSCommand = value;
                this.OnPropertyChanged("StartATSCommand");
            }
        }
        
        public virtual GES.Communications.SocketTransceiver Receiver {
            get {
                return this._Receiver;
            }
            set {
                this._Receiver = value;
                this.OnPropertyChanged("Receiver");
            }
        }
        
        public virtual GES.Communications.TelemetryTask TelemetryTask {
            get {
                return this._TelemetryTask;
            }
            set {
                this._TelemetryTask = value;
                this.OnPropertyChanged("TelemetryTask");
            }
        }
        
        public virtual CFS.Telemetry.EventServicesHousekeepingTelemetery EventServicesHousekeepingTelemetery {
            get {
                return this._EventServicesHousekeepingTelemetery;
            }
            set {
                this._EventServicesHousekeepingTelemetery = value;
                this.OnPropertyChanged("EventServicesHousekeepingTelemetery");
            }
        }
        
        public virtual CFS.Telemetry.ExecutiveServicesHousekeepingTelemetery ExecutiveServicesHousekeepingTelemetery {
            get {
                return this._ExecutiveServicesHousekeepingTelemetery;
            }
            set {
                this._ExecutiveServicesHousekeepingTelemetery = value;
                this.OnPropertyChanged("ExecutiveServicesHousekeepingTelemetery");
            }
        }
        
        public virtual void Connect() {
            // 
            // Connects subcomponents into graph 
            // 
            this.Connect("Transmitter.Active", "Active");
            this.Connect("Transmitter.IncomingQueue", "CommandTask.UplinkQueue");
            this.Connect("CommandTask.Active", "Transmitter.Connection");
            this.Connect("CommandTask.Commands", "TelemetryOutputEnableCommand");
            this.Connect("CommandTask.Commands", "NETIFInitSocketCommand");
            this.Connect("CommandTask.Commands", "TableLoadCommand");
            this.Connect("CommandTask.Commands", "TableValidateCommand");
            this.Connect("CommandTask.Commands", "TableActivateCommand");
            this.Connect("CommandTask.Commands", "StartATSCommand");
            this.Connect("Receiver.Active", "Active");
            this.Connect("TelemetryTask.Active", "Receiver.Connection");
            this.Connect("TelemetryTask.Telemetry", "ExecutiveServicesHousekeepingTelemetery");
            this.Connect("TelemetryTask.Telemetry", "EventServicesHousekeepingTelemetery");
            this.Connect("TelemetryTask.DownlinkQueue", "Receiver.OutgoingQueue");
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}