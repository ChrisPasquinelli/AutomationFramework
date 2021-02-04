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
    public partial class CFEFileHeader : MTI.Core.Component {
        
        private uint _ContentType;
        
        private uint _SubType;
        
        private uint _Length;
        
        private uint _SpacecraftID;
        
        private uint _ProcessorID;
        
        private uint _ApplicationID;
        
        private int _TimeSeconds;
        
        private uint _TimeSubseconds;
        
        private string _Description;
        
        public CFEFileHeader() {
            // 
            // Construct _ContentType
            // 
            this._ContentType = CFEFileHeader.CreateContentType();
            // 
            // Construct _SubType
            // 
            this._SubType = CFEFileHeader.CreateSubType();
            // 
            // Construct _Length
            // 
            this._Length = CFEFileHeader.CreateLength();
            // 
            // Construct _SpacecraftID
            // 
            this._SpacecraftID = CFEFileHeader.CreateSpacecraftID();
            // 
            // Construct _ProcessorID
            // 
            this._ProcessorID = CFEFileHeader.CreateProcessorID();
            // 
            // Construct _ApplicationID
            // 
            this._ApplicationID = CFEFileHeader.CreateApplicationID();
            // 
            // Construct _TimeSeconds
            // 
            this._TimeSeconds = CFEFileHeader.CreateTimeSeconds();
            // 
            // Construct _TimeSubseconds
            // 
            this._TimeSubseconds = CFEFileHeader.CreateTimeSubseconds();
            // 
            // Construct _Description
            // 
            this._Description = CFEFileHeader.CreateDescription();
            // 
            // Connect Graph
            // 
            this.Connect();
        }
        
        public virtual uint ContentType {
            get {
                return this._ContentType;
            }
            set {
                this._ContentType = value;
                this.OnPropertyChanged("ContentType");
            }
        }
        
        public virtual uint SubType {
            get {
                return this._SubType;
            }
            set {
                this._SubType = value;
                this.OnPropertyChanged("SubType");
            }
        }
        
        public virtual uint Length {
            get {
                return this._Length;
            }
            set {
                this._Length = value;
                this.OnPropertyChanged("Length");
            }
        }
        
        public virtual uint SpacecraftID {
            get {
                return this._SpacecraftID;
            }
            set {
                this._SpacecraftID = value;
                this.OnPropertyChanged("SpacecraftID");
            }
        }
        
        public virtual uint ProcessorID {
            get {
                return this._ProcessorID;
            }
            set {
                this._ProcessorID = value;
                this.OnPropertyChanged("ProcessorID");
            }
        }
        
        public virtual uint ApplicationID {
            get {
                return this._ApplicationID;
            }
            set {
                this._ApplicationID = value;
                this.OnPropertyChanged("ApplicationID");
            }
        }
        
        public virtual int TimeSeconds {
            get {
                return this._TimeSeconds;
            }
            set {
                this._TimeSeconds = value;
                this.OnPropertyChanged("TimeSeconds");
            }
        }
        
        public virtual uint TimeSubseconds {
            get {
                return this._TimeSubseconds;
            }
            set {
                this._TimeSubseconds = value;
                this.OnPropertyChanged("TimeSubseconds");
            }
        }
        
        public virtual string Description {
            get {
                return this._Description;
            }
            set {
                this._Description = value;
                this.OnPropertyChanged("Description");
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
