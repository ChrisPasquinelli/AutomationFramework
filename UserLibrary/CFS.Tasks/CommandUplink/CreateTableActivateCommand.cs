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
    
    
    public partial class CommandUplink {
        
        public static CFS.Commands.TableActivateCommand CreateTableActivateCommand() {
            // 
            // Creates a new instance of the CFS.Commands.TableActivateCommand class TableActivateCommand.
            // 
            CFS.Commands.TableActivateCommand TableActivateCommand = new CFS.Commands.TableActivateCommand();
            TableActivateCommand.TableName = "SC.ATS_TBL2";
            return TableActivateCommand;
        }
    }
}
