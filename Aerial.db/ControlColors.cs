﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace Aerial.db.ControlColors {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/ButtonColors.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/ButtonColors.xsd", IsNullable=false)]
    public partial class ControlColors {
        
        private Control[] controlField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Control")]
        public Control[] Control {
            get {
                return this.controlField;
            }
            set {
                this.controlField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/ButtonColors.xsd")]
    public partial class Control {
        
        private string idField;
        
        private int foreRField;
        
        private int foreGField;
        
        private int foreBField;
        
        private int backRField;
        
        private int backGField;
        
        private int backBField;
        
        /// <remarks/>
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public int ForeR {
            get {
                return this.foreRField;
            }
            set {
                this.foreRField = value;
            }
        }
        
        /// <remarks/>
        public int ForeG {
            get {
                return this.foreGField;
            }
            set {
                this.foreGField = value;
            }
        }
        
        /// <remarks/>
        public int ForeB {
            get {
                return this.foreBField;
            }
            set {
                this.foreBField = value;
            }
        }
        
        /// <remarks/>
        public int BackR {
            get {
                return this.backRField;
            }
            set {
                this.backRField = value;
            }
        }
        
        /// <remarks/>
        public int BackG {
            get {
                return this.backGField;
            }
            set {
                this.backGField = value;
            }
        }
        
        /// <remarks/>
        public int BackB {
            get {
                return this.backBField;
            }
            set {
                this.backBField = value;
            }
        }
    }
}
