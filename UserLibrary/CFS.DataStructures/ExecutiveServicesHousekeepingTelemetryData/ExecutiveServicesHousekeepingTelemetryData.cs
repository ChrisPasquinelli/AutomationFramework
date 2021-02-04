//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CFS.DataStructures {
    using System;
    using MTI.Core;
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "CFS.DataStructures"})]
    public partial class ExecutiveServicesHousekeepingTelemetryData : MTI.Core.Component {
        
        private byte _CommandCounter;
        
        private byte _CommandErrorCounter;
        
        private ushort _CFECoreChecksum;
        
        private byte _CFEMajorVersion;
        
        private byte _CFEMinorVersion;
        
        private byte _CFERevision;
        
        private byte _CFEMissionRevision;
        
        private byte _OSALMajorVersion;
        
        private byte _OSALMinorVersion;
        
        private byte _OSALRevision;
        
        private byte _OSALMissionRevision;
        
        private uint _SysLogBytesUsed;
        
        private uint _SysLogSize;
        
        private uint _SysLogEntries;
        
        private uint _SysLogMode;
        
        private uint _ERLogIndex;
        
        private uint _ERLogEntries;
        
        private uint _RegisteredCoreApps;
        
        private uint _RegisteredExternalApps;
        
        private uint _RegisteredTasks;
        
        private uint _RegisteredLibs;
        
        private uint _ResetType;
        
        private uint _ResetSubType;
        
        private uint _ProcessorResets;
        
        private uint _MaxProcessorResets;
        
        private uint _BootSource;
        
        private uint _PerfState;
        
        private uint _PerfMode;
        
        private uint _PerfTriggerCount;
        
        private uint _PerfFilterMask1;
        
        private uint _PerfFilterMask2;
        
        private uint _PerfFilterMask3;
        
        private uint _PerfFilterMask4;
        
        private uint _PerfTriggerMask1;
        
        private uint _PerfTriggerMask2;
        
        private uint _PerfTriggerMask3;
        
        private uint _PerfTriggerMask4;
        
        private uint _PerfDataStart;
        
        private uint _PerfDataEnd;
        
        private uint _PerfDataCount;
        
        private uint _PerfDataToWrite;
        
        private uint _HeapBytesFree;
        
        private uint _HeapBlockFree;
        
        private uint _HeapMaxBlockSize;
        
        public ExecutiveServicesHousekeepingTelemetryData() {
            // 
            // Construct _CommandCounter
            // 
            this._CommandCounter = ExecutiveServicesHousekeepingTelemetryData.CreateCommandCounter();
            // 
            // Construct _CommandErrorCounter
            // 
            this._CommandErrorCounter = ExecutiveServicesHousekeepingTelemetryData.CreateCommandErrorCounter();
            // 
            // Construct _CFECoreChecksum
            // 
            this._CFECoreChecksum = ExecutiveServicesHousekeepingTelemetryData.CreateCFECoreChecksum();
            // 
            // Construct _CFEMajorVersion
            // 
            this._CFEMajorVersion = ExecutiveServicesHousekeepingTelemetryData.CreateCFEMajorVersion();
            // 
            // Construct _CFEMinorVersion
            // 
            this._CFEMinorVersion = ExecutiveServicesHousekeepingTelemetryData.CreateCFEMinorVersion();
            // 
            // Construct _CFERevision
            // 
            this._CFERevision = ExecutiveServicesHousekeepingTelemetryData.CreateCFERevision();
            // 
            // Construct _CFEMissionRevision
            // 
            this._CFEMissionRevision = ExecutiveServicesHousekeepingTelemetryData.CreateCFEMissionRevision();
            // 
            // Construct _OSALMajorVersion
            // 
            this._OSALMajorVersion = ExecutiveServicesHousekeepingTelemetryData.CreateOSALMajorVersion();
            // 
            // Construct _OSALMinorVersion
            // 
            this._OSALMinorVersion = ExecutiveServicesHousekeepingTelemetryData.CreateOSALMinorVersion();
            // 
            // Construct _OSALRevision
            // 
            this._OSALRevision = ExecutiveServicesHousekeepingTelemetryData.CreateOSALRevision();
            // 
            // Construct _OSALMissionRevision
            // 
            this._OSALMissionRevision = ExecutiveServicesHousekeepingTelemetryData.CreateOSALMissionRevision();
            // 
            // Construct _SysLogBytesUsed
            // 
            this._SysLogBytesUsed = ExecutiveServicesHousekeepingTelemetryData.CreateSysLogBytesUsed();
            // 
            // Construct _SysLogSize
            // 
            this._SysLogSize = ExecutiveServicesHousekeepingTelemetryData.CreateSysLogSize();
            // 
            // Construct _SysLogEntries
            // 
            this._SysLogEntries = ExecutiveServicesHousekeepingTelemetryData.CreateSysLogEntries();
            // 
            // Construct _SysLogMode
            // 
            this._SysLogMode = ExecutiveServicesHousekeepingTelemetryData.CreateSysLogMode();
            // 
            // Construct _ERLogIndex
            // 
            this._ERLogIndex = ExecutiveServicesHousekeepingTelemetryData.CreateERLogIndex();
            // 
            // Construct _ERLogEntries
            // 
            this._ERLogEntries = ExecutiveServicesHousekeepingTelemetryData.CreateERLogEntries();
            // 
            // Construct _RegisteredCoreApps
            // 
            this._RegisteredCoreApps = ExecutiveServicesHousekeepingTelemetryData.CreateRegisteredCoreApps();
            // 
            // Construct _RegisteredExternalApps
            // 
            this._RegisteredExternalApps = ExecutiveServicesHousekeepingTelemetryData.CreateRegisteredExternalApps();
            // 
            // Construct _RegisteredTasks
            // 
            this._RegisteredTasks = ExecutiveServicesHousekeepingTelemetryData.CreateRegisteredTasks();
            // 
            // Construct _RegisteredLibs
            // 
            this._RegisteredLibs = ExecutiveServicesHousekeepingTelemetryData.CreateRegisteredLibs();
            // 
            // Construct _ResetType
            // 
            this._ResetType = ExecutiveServicesHousekeepingTelemetryData.CreateResetType();
            // 
            // Construct _ResetSubType
            // 
            this._ResetSubType = ExecutiveServicesHousekeepingTelemetryData.CreateResetSubType();
            // 
            // Construct _ProcessorResets
            // 
            this._ProcessorResets = ExecutiveServicesHousekeepingTelemetryData.CreateProcessorResets();
            // 
            // Construct _MaxProcessorResets
            // 
            this._MaxProcessorResets = ExecutiveServicesHousekeepingTelemetryData.CreateMaxProcessorResets();
            // 
            // Construct _BootSource
            // 
            this._BootSource = ExecutiveServicesHousekeepingTelemetryData.CreateBootSource();
            // 
            // Construct _PerfState
            // 
            this._PerfState = ExecutiveServicesHousekeepingTelemetryData.CreatePerfState();
            // 
            // Construct _PerfMode
            // 
            this._PerfMode = ExecutiveServicesHousekeepingTelemetryData.CreatePerfMode();
            // 
            // Construct _PerfTriggerCount
            // 
            this._PerfTriggerCount = ExecutiveServicesHousekeepingTelemetryData.CreatePerfTriggerCount();
            // 
            // Construct _PerfFilterMask1
            // 
            this._PerfFilterMask1 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfFilterMask1();
            // 
            // Construct _PerfFilterMask2
            // 
            this._PerfFilterMask2 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfFilterMask2();
            // 
            // Construct _PerfFilterMask3
            // 
            this._PerfFilterMask3 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfFilterMask3();
            // 
            // Construct _PerfFilterMask4
            // 
            this._PerfFilterMask4 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfFilterMask4();
            // 
            // Construct _PerfTriggerMask1
            // 
            this._PerfTriggerMask1 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfTriggerMask1();
            // 
            // Construct _PerfTriggerMask2
            // 
            this._PerfTriggerMask2 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfTriggerMask2();
            // 
            // Construct _PerfTriggerMask3
            // 
            this._PerfTriggerMask3 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfTriggerMask3();
            // 
            // Construct _PerfTriggerMask4
            // 
            this._PerfTriggerMask4 = ExecutiveServicesHousekeepingTelemetryData.CreatePerfTriggerMask4();
            // 
            // Construct _PerfDataStart
            // 
            this._PerfDataStart = ExecutiveServicesHousekeepingTelemetryData.CreatePerfDataStart();
            // 
            // Construct _PerfDataEnd
            // 
            this._PerfDataEnd = ExecutiveServicesHousekeepingTelemetryData.CreatePerfDataEnd();
            // 
            // Construct _PerfDataCount
            // 
            this._PerfDataCount = ExecutiveServicesHousekeepingTelemetryData.CreatePerfDataCount();
            // 
            // Construct _PerfDataToWrite
            // 
            this._PerfDataToWrite = ExecutiveServicesHousekeepingTelemetryData.CreatePerfDataToWrite();
            // 
            // Construct _HeapBytesFree
            // 
            this._HeapBytesFree = ExecutiveServicesHousekeepingTelemetryData.CreateHeapBytesFree();
            // 
            // Construct _HeapBlockFree
            // 
            this._HeapBlockFree = ExecutiveServicesHousekeepingTelemetryData.CreateHeapBlockFree();
            // 
            // Construct _HeapMaxBlockSize
            // 
            this._HeapMaxBlockSize = ExecutiveServicesHousekeepingTelemetryData.CreateHeapMaxBlockSize();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        public virtual byte CommandCounter {
            get {
                return this._CommandCounter;
            }
            set {
                this._CommandCounter = value;
                this.OnPropertyChanged("CommandCounter");
            }
        }
        
        public virtual byte CommandErrorCounter {
            get {
                return this._CommandErrorCounter;
            }
            set {
                this._CommandErrorCounter = value;
                this.OnPropertyChanged("CommandErrorCounter");
            }
        }
        
        public virtual ushort CFECoreChecksum {
            get {
                return this._CFECoreChecksum;
            }
            set {
                this._CFECoreChecksum = value;
                this.OnPropertyChanged("CFECoreChecksum");
            }
        }
        
        public virtual byte CFEMajorVersion {
            get {
                return this._CFEMajorVersion;
            }
            set {
                this._CFEMajorVersion = value;
                this.OnPropertyChanged("CFEMajorVersion");
            }
        }
        
        public virtual byte CFEMinorVersion {
            get {
                return this._CFEMinorVersion;
            }
            set {
                this._CFEMinorVersion = value;
                this.OnPropertyChanged("CFEMinorVersion");
            }
        }
        
        public virtual byte CFERevision {
            get {
                return this._CFERevision;
            }
            set {
                this._CFERevision = value;
                this.OnPropertyChanged("CFERevision");
            }
        }
        
        public virtual byte CFEMissionRevision {
            get {
                return this._CFEMissionRevision;
            }
            set {
                this._CFEMissionRevision = value;
                this.OnPropertyChanged("CFEMissionRevision");
            }
        }
        
        public virtual byte OSALMajorVersion {
            get {
                return this._OSALMajorVersion;
            }
            set {
                this._OSALMajorVersion = value;
                this.OnPropertyChanged("OSALMajorVersion");
            }
        }
        
        public virtual byte OSALMinorVersion {
            get {
                return this._OSALMinorVersion;
            }
            set {
                this._OSALMinorVersion = value;
                this.OnPropertyChanged("OSALMinorVersion");
            }
        }
        
        public virtual byte OSALRevision {
            get {
                return this._OSALRevision;
            }
            set {
                this._OSALRevision = value;
                this.OnPropertyChanged("OSALRevision");
            }
        }
        
        public virtual byte OSALMissionRevision {
            get {
                return this._OSALMissionRevision;
            }
            set {
                this._OSALMissionRevision = value;
                this.OnPropertyChanged("OSALMissionRevision");
            }
        }
        
        public virtual uint SysLogBytesUsed {
            get {
                return this._SysLogBytesUsed;
            }
            set {
                this._SysLogBytesUsed = value;
                this.OnPropertyChanged("SysLogBytesUsed");
            }
        }
        
        public virtual uint SysLogSize {
            get {
                return this._SysLogSize;
            }
            set {
                this._SysLogSize = value;
                this.OnPropertyChanged("SysLogSize");
            }
        }
        
        public virtual uint SysLogEntries {
            get {
                return this._SysLogEntries;
            }
            set {
                this._SysLogEntries = value;
                this.OnPropertyChanged("SysLogEntries");
            }
        }
        
        public virtual uint SysLogMode {
            get {
                return this._SysLogMode;
            }
            set {
                this._SysLogMode = value;
                this.OnPropertyChanged("SysLogMode");
            }
        }
        
        public virtual uint ERLogIndex {
            get {
                return this._ERLogIndex;
            }
            set {
                this._ERLogIndex = value;
                this.OnPropertyChanged("ERLogIndex");
            }
        }
        
        public virtual uint ERLogEntries {
            get {
                return this._ERLogEntries;
            }
            set {
                this._ERLogEntries = value;
                this.OnPropertyChanged("ERLogEntries");
            }
        }
        
        public virtual uint RegisteredCoreApps {
            get {
                return this._RegisteredCoreApps;
            }
            set {
                this._RegisteredCoreApps = value;
                this.OnPropertyChanged("RegisteredCoreApps");
            }
        }
        
        public virtual uint RegisteredExternalApps {
            get {
                return this._RegisteredExternalApps;
            }
            set {
                this._RegisteredExternalApps = value;
                this.OnPropertyChanged("RegisteredExternalApps");
            }
        }
        
        public virtual uint RegisteredTasks {
            get {
                return this._RegisteredTasks;
            }
            set {
                this._RegisteredTasks = value;
                this.OnPropertyChanged("RegisteredTasks");
            }
        }
        
        public virtual uint RegisteredLibs {
            get {
                return this._RegisteredLibs;
            }
            set {
                this._RegisteredLibs = value;
                this.OnPropertyChanged("RegisteredLibs");
            }
        }
        
        public virtual uint ResetType {
            get {
                return this._ResetType;
            }
            set {
                this._ResetType = value;
                this.OnPropertyChanged("ResetType");
            }
        }
        
        public virtual uint ResetSubType {
            get {
                return this._ResetSubType;
            }
            set {
                this._ResetSubType = value;
                this.OnPropertyChanged("ResetSubType");
            }
        }
        
        public virtual uint ProcessorResets {
            get {
                return this._ProcessorResets;
            }
            set {
                this._ProcessorResets = value;
                this.OnPropertyChanged("ProcessorResets");
            }
        }
        
        public virtual uint MaxProcessorResets {
            get {
                return this._MaxProcessorResets;
            }
            set {
                this._MaxProcessorResets = value;
                this.OnPropertyChanged("MaxProcessorResets");
            }
        }
        
        public virtual uint BootSource {
            get {
                return this._BootSource;
            }
            set {
                this._BootSource = value;
                this.OnPropertyChanged("BootSource");
            }
        }
        
        public virtual uint PerfState {
            get {
                return this._PerfState;
            }
            set {
                this._PerfState = value;
                this.OnPropertyChanged("PerfState");
            }
        }
        
        public virtual uint PerfMode {
            get {
                return this._PerfMode;
            }
            set {
                this._PerfMode = value;
                this.OnPropertyChanged("PerfMode");
            }
        }
        
        public virtual uint PerfTriggerCount {
            get {
                return this._PerfTriggerCount;
            }
            set {
                this._PerfTriggerCount = value;
                this.OnPropertyChanged("PerfTriggerCount");
            }
        }
        
        public virtual uint PerfFilterMask1 {
            get {
                return this._PerfFilterMask1;
            }
            set {
                this._PerfFilterMask1 = value;
                this.OnPropertyChanged("PerfFilterMask1");
            }
        }
        
        public virtual uint PerfFilterMask2 {
            get {
                return this._PerfFilterMask2;
            }
            set {
                this._PerfFilterMask2 = value;
                this.OnPropertyChanged("PerfFilterMask2");
            }
        }
        
        public virtual uint PerfFilterMask3 {
            get {
                return this._PerfFilterMask3;
            }
            set {
                this._PerfFilterMask3 = value;
                this.OnPropertyChanged("PerfFilterMask3");
            }
        }
        
        public virtual uint PerfFilterMask4 {
            get {
                return this._PerfFilterMask4;
            }
            set {
                this._PerfFilterMask4 = value;
                this.OnPropertyChanged("PerfFilterMask4");
            }
        }
        
        public virtual uint PerfTriggerMask1 {
            get {
                return this._PerfTriggerMask1;
            }
            set {
                this._PerfTriggerMask1 = value;
                this.OnPropertyChanged("PerfTriggerMask1");
            }
        }
        
        public virtual uint PerfTriggerMask2 {
            get {
                return this._PerfTriggerMask2;
            }
            set {
                this._PerfTriggerMask2 = value;
                this.OnPropertyChanged("PerfTriggerMask2");
            }
        }
        
        public virtual uint PerfTriggerMask3 {
            get {
                return this._PerfTriggerMask3;
            }
            set {
                this._PerfTriggerMask3 = value;
                this.OnPropertyChanged("PerfTriggerMask3");
            }
        }
        
        public virtual uint PerfTriggerMask4 {
            get {
                return this._PerfTriggerMask4;
            }
            set {
                this._PerfTriggerMask4 = value;
                this.OnPropertyChanged("PerfTriggerMask4");
            }
        }
        
        public virtual uint PerfDataStart {
            get {
                return this._PerfDataStart;
            }
            set {
                this._PerfDataStart = value;
                this.OnPropertyChanged("PerfDataStart");
            }
        }
        
        public virtual uint PerfDataEnd {
            get {
                return this._PerfDataEnd;
            }
            set {
                this._PerfDataEnd = value;
                this.OnPropertyChanged("PerfDataEnd");
            }
        }
        
        public virtual uint PerfDataCount {
            get {
                return this._PerfDataCount;
            }
            set {
                this._PerfDataCount = value;
                this.OnPropertyChanged("PerfDataCount");
            }
        }
        
        public virtual uint PerfDataToWrite {
            get {
                return this._PerfDataToWrite;
            }
            set {
                this._PerfDataToWrite = value;
                this.OnPropertyChanged("PerfDataToWrite");
            }
        }
        
        public virtual uint HeapBytesFree {
            get {
                return this._HeapBytesFree;
            }
            set {
                this._HeapBytesFree = value;
                this.OnPropertyChanged("HeapBytesFree");
            }
        }
        
        public virtual uint HeapBlockFree {
            get {
                return this._HeapBlockFree;
            }
            set {
                this._HeapBlockFree = value;
                this.OnPropertyChanged("HeapBlockFree");
            }
        }
        
        public virtual uint HeapMaxBlockSize {
            get {
                return this._HeapMaxBlockSize;
            }
            set {
                this._HeapMaxBlockSize = value;
                this.OnPropertyChanged("HeapMaxBlockSize");
            }
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
