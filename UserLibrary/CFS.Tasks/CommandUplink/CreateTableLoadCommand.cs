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
        
        public static CFS.Commands.TableLoadCommand CreateTableLoadCommand() {
            // 
            // Creates a new instance of the CFS.Commands.TableLoadCommand class TableLoadCommand.
            // 
            CFS.Commands.TableLoadCommand TableLoadCommand = new CFS.Commands.TableLoadCommand();
            TableLoadCommand.Filename = "/cf/table.tbl";
            return TableLoadCommand;
        }
    }
}
