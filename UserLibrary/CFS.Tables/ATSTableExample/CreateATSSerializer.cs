//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CFS.Tables {
    using System;
    using MTI.Core;
    using GES.Communications;
    using CFS.Tables.Definitions;
    using CFS.Commands;
    
    
    public partial class ATSTableExample {
        
        public static GES.Communications.TimeSequenceSerializer CreateATSSerializer() {
            // 
            // Creates a new instance of the GES.Communications.TimeSequenceSerializer class ATSSerializer.
            // 
            GES.Communications.TimeSequenceSerializer ATSSerializer = new GES.Communications.TimeSequenceSerializer();
            ATSSerializer.MissionEpochTime.DateTime = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            ATSSerializer.StartTime.BaseStandard = GES.TimeSystems.TimeStandard.CoordinatedUniversalTime;
            ATSSerializer.StartTime.DateTime = new System.DateTime(1980, 1, 12, 14, 5, 0, 0);
            ATSSerializer.Mode = GES.Communications.TimeSequenceSerializer.OperationMode.Write;
            return ATSSerializer;
        }
    }
}
