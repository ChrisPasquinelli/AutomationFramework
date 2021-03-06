//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CFS.Tables.Definitions {
    using System;
    using MTI.Core;
    using GES.Communications;
    using CFS.DataStructures;
    
    
    [MTI.Core.ProvideAttribute(Categories=new string[] {
            "CFS.Tables.Definitions"})]
    public partial class TableDefinitionTemplate : GES.Communications.TableDefinition {
        
        private CFS.DataStructures.CFEFileHeader _CFEFileHeader;
        
        private CFS.DataStructures.CFETableHeader _CFETableHeader;
        
        public TableDefinitionTemplate() {
            // 
            // Initialize base members
            // 
            this.InitializeBaseMembers();
            // 
            // Construct _CFEFileHeader
            // 
            this._CFEFileHeader = TableDefinitionTemplate.CreateCFEFileHeader();
            // 
            // Construct _CFETableHeader
            // 
            this._CFETableHeader = TableDefinitionTemplate.CreateCFETableHeader();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        public new virtual object FileHeader {
            get {
                return base.FileHeader;
            }
            set {
                base.FileHeader = value;
                this.OnPropertyChanged("TableDefinition.FileHeader");
            }
        }
        
        public new virtual object TableHeader {
            get {
                return base.TableHeader;
            }
            set {
                base.TableHeader = value;
                this.OnPropertyChanged("TableDefinition.TableHeader");
            }
        }
        
        public new virtual GES.Communications.Checksum Checksum {
            get {
                return base.Checksum;
            }
            set {
                base.Checksum = value;
                this.OnPropertyChanged("TableDefinition.Checksum");
            }
        }
        
        public virtual CFS.DataStructures.CFEFileHeader CFEFileHeader {
            get {
                return this._CFEFileHeader;
            }
            set {
                this._CFEFileHeader = value;
                this.OnPropertyChanged("CFEFileHeader");
            }
        }
        
        public virtual CFS.DataStructures.CFETableHeader CFETableHeader {
            get {
                return this._CFETableHeader;
            }
            set {
                this._CFETableHeader = value;
                this.OnPropertyChanged("CFETableHeader");
            }
        }
        
        private void InitializeBaseMembers() {
            // 
            // Creates a new instance of the GES.Communications.TableDefinition class TableDefinition.
            // 
            GES.Communications.DataAttributes dataattributes14 = new GES.Communications.DataAttributes();
            dataattributes14.Name = "ContentType";
            dataattributes14.Static = false;
            dataattributes14.DefaultValue = "1665549617";
            dataattributes14.MinValue = "0";
            dataattributes14.MaxValue = "4294967295";
            dataattributes14.DataType = "UInt32";
            dataattributes14.BitLength = 32;
            dataattributes14.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes14.Equation = "";
            dataattributes14.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes14.LocalPathName = "CFEFileHeader.ContentType";
            GES.Communications.DataAttributes dataattributes15 = new GES.Communications.DataAttributes();
            dataattributes15.Name = "SubType";
            dataattributes15.Static = false;
            dataattributes15.DefaultValue = "0";
            dataattributes15.MinValue = "0";
            dataattributes15.MaxValue = "4294967295";
            dataattributes15.DataType = "UInt32";
            dataattributes15.BitLength = 32;
            dataattributes15.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes15.Equation = "";
            dataattributes15.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes15.LocalPathName = "CFEFileHeader.SubType";
            GES.Communications.DataAttributes dataattributes16 = new GES.Communications.DataAttributes();
            dataattributes16.Name = "Length";
            dataattributes16.Static = false;
            dataattributes16.DefaultValue = "0";
            dataattributes16.MinValue = "0";
            dataattributes16.MaxValue = "4294967295";
            dataattributes16.DataType = "UInt32";
            dataattributes16.BitLength = 32;
            dataattributes16.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes16.Equation = "";
            dataattributes16.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes16.LocalPathName = "CFEFileHeader.Length";
            GES.Communications.DataAttributes dataattributes17 = new GES.Communications.DataAttributes();
            dataattributes17.Name = "SpacecraftID";
            dataattributes17.Static = false;
            dataattributes17.DefaultValue = "0";
            dataattributes17.MinValue = "0";
            dataattributes17.MaxValue = "4294967295";
            dataattributes17.DataType = "UInt32";
            dataattributes17.BitLength = 32;
            dataattributes17.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes17.Equation = "";
            dataattributes17.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes17.LocalPathName = "CFEFileHeader.SpacecraftID";
            GES.Communications.DataAttributes dataattributes18 = new GES.Communications.DataAttributes();
            dataattributes18.Name = "ProcessorID";
            dataattributes18.Static = false;
            dataattributes18.DefaultValue = "0";
            dataattributes18.MinValue = "0";
            dataattributes18.MaxValue = "4294967295";
            dataattributes18.DataType = "UInt32";
            dataattributes18.BitLength = 32;
            dataattributes18.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes18.Equation = "";
            dataattributes18.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes18.LocalPathName = "CFEFileHeader.ProcessorID";
            GES.Communications.DataAttributes dataattributes19 = new GES.Communications.DataAttributes();
            dataattributes19.Name = "ApplicationID";
            dataattributes19.Static = false;
            dataattributes19.DefaultValue = "0";
            dataattributes19.MinValue = "0";
            dataattributes19.MaxValue = "4294967295";
            dataattributes19.DataType = "UInt32";
            dataattributes19.BitLength = 32;
            dataattributes19.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes19.Equation = "";
            dataattributes19.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes19.LocalPathName = "CFEFileHeader.ApplicationID";
            GES.Communications.DataAttributes dataattributes20 = new GES.Communications.DataAttributes();
            dataattributes20.Name = "TimeSeconds";
            dataattributes20.Static = false;
            dataattributes20.DefaultValue = "0";
            dataattributes20.MinValue = "2147483647";
            dataattributes20.MaxValue = "2147483647";
            dataattributes20.DataType = "Int32";
            dataattributes20.BitLength = 32;
            dataattributes20.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes20.Equation = "";
            dataattributes20.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes20.LocalPathName = "CFEFileHeader.TimeSeconds";
            GES.Communications.DataAttributes dataattributes21 = new GES.Communications.DataAttributes();
            dataattributes21.Name = "TimeSubseconds";
            dataattributes21.Static = false;
            dataattributes21.DefaultValue = "0";
            dataattributes21.MinValue = "0";
            dataattributes21.MaxValue = "4294967295";
            dataattributes21.DataType = "UInt32";
            dataattributes21.BitLength = 32;
            dataattributes21.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes21.Equation = "";
            dataattributes21.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes21.LocalPathName = "CFEFileHeader.TimeSubseconds";
            GES.Communications.DataAttributes dataattributes22 = new GES.Communications.DataAttributes();
            dataattributes22.Name = "Description";
            dataattributes22.Static = false;
            dataattributes22.DefaultValue = "";
            dataattributes22.MinValue = "";
            dataattributes22.MaxValue = "";
            dataattributes22.DataType = "String";
            dataattributes22.BitLength = 256;
            dataattributes22.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes22.Equation = "";
            dataattributes22.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes22.LocalPathName = "CFEFileHeader.Description";
            base.FileHeaderAttributes = new GES.Communications.DataAttributes[] {
                    ((GES.Communications.DataAttributes)(dataattributes14)),
                    ((GES.Communications.DataAttributes)(dataattributes15)),
                    ((GES.Communications.DataAttributes)(dataattributes16)),
                    ((GES.Communications.DataAttributes)(dataattributes17)),
                    ((GES.Communications.DataAttributes)(dataattributes18)),
                    ((GES.Communications.DataAttributes)(dataattributes19)),
                    ((GES.Communications.DataAttributes)(dataattributes20)),
                    ((GES.Communications.DataAttributes)(dataattributes21)),
                    ((GES.Communications.DataAttributes)(dataattributes22))};
            GES.Communications.DataAttributes dataattributes23 = new GES.Communications.DataAttributes();
            dataattributes23.Name = "Reserved";
            dataattributes23.Static = false;
            dataattributes23.DefaultValue = "0";
            dataattributes23.MinValue = "0";
            dataattributes23.MaxValue = "4294967295";
            dataattributes23.DataType = "UInt32";
            dataattributes23.BitLength = 32;
            dataattributes23.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes23.Equation = "";
            dataattributes23.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes23.LocalPathName = "CFETableHeader.Reserved";
            GES.Communications.DataAttributes dataattributes24 = new GES.Communications.DataAttributes();
            dataattributes24.Name = "Offset";
            dataattributes24.Static = false;
            dataattributes24.DefaultValue = "0";
            dataattributes24.MinValue = "0";
            dataattributes24.MaxValue = "4294967295";
            dataattributes24.DataType = "UInt32";
            dataattributes24.BitLength = 32;
            dataattributes24.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes24.Equation = "";
            dataattributes24.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes24.LocalPathName = "CFETableHeader.Offset";
            GES.Communications.DataAttributes dataattributes25 = new GES.Communications.DataAttributes();
            dataattributes25.Name = "NumBytes";
            dataattributes25.Static = false;
            dataattributes25.DefaultValue = "0";
            dataattributes25.MinValue = "0";
            dataattributes25.MaxValue = "4294967295";
            dataattributes25.DataType = "UInt32";
            dataattributes25.BitLength = 32;
            dataattributes25.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes25.Equation = "";
            dataattributes25.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes25.LocalPathName = "CFETableHeader.NumBytes";
            GES.Communications.DataAttributes dataattributes26 = new GES.Communications.DataAttributes();
            dataattributes26.Name = "TableName";
            dataattributes26.Static = false;
            dataattributes26.DefaultValue = "";
            dataattributes26.MinValue = "";
            dataattributes26.MaxValue = "";
            dataattributes26.DataType = "String";
            dataattributes26.BitLength = 320;
            dataattributes26.Endian = GES.Communications.DataAttributes.EndianType.BigEndian;
            dataattributes26.Equation = "";
            dataattributes26.EnumeratedValues = new GES.Communications.DataAttributes.Enumeration[0];
            dataattributes26.LocalPathName = "CFETableHeader.TableName";
            base.TableHeaderAttributes = new GES.Communications.DataAttributes[] {
                    ((GES.Communications.DataAttributes)(dataattributes23)),
                    ((GES.Communications.DataAttributes)(dataattributes24)),
                    ((GES.Communications.DataAttributes)(dataattributes25)),
                    ((GES.Communications.DataAttributes)(dataattributes26))};
            base.ContentAttributes = new GES.Communications.DataAttributes[0];
        }
        
        public virtual void Connect() {
            // 
            // Connects subcomponents into graph 
            // 
            this.Connect("base.FileHeader", "CFEFileHeader");
            this.Connect("base.TableHeader", "CFETableHeader");
        }
        
        public override bool OnInitialize() {
            // 
            // Initialize subcomponents
            // 
            return base.OnInitialize();
        }
    }
}
