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
namespace Aerial.db.dal.WorkOrderExtra {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/WorkOrderExtra.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/WorkOrderExtra.xsd", IsNullable=false)]
    public partial class WorkOrderExtra {
        
        private Product[] productsField;
        
        private decimal applicationTotalField;
        
        private decimal applicationRateField;
        
        private string applicationUnitOfMeasureField;
        
        private decimal applicationLoadsField;
        
        private decimal applicationAcresPerLoadField;
        
        private decimal applicationAmountPerLoadField;
        
        private string hQNotesField;
        
        public WorkOrderExtra() {
            this.applicationTotalField = ((decimal)(0m));
            this.applicationRateField = ((decimal)(0m));
            this.applicationUnitOfMeasureField = "";
            this.applicationLoadsField = ((decimal)(0m));
            this.applicationAcresPerLoadField = ((decimal)(0m));
            this.applicationAmountPerLoadField = ((decimal)(0m));
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Products")]
        public Product[] Products {
            get {
                return this.productsField;
            }
            set {
                this.productsField = value;
            }
        }
        
        /// <remarks/>
        public decimal ApplicationTotal {
            get {
                return this.applicationTotalField;
            }
            set {
                this.applicationTotalField = value;
            }
        }
        
        /// <remarks/>
        public decimal ApplicationRate {
            get {
                return this.applicationRateField;
            }
            set {
                this.applicationRateField = value;
            }
        }
        
        /// <remarks/>
        public string ApplicationUnitOfMeasure {
            get {
                return this.applicationUnitOfMeasureField;
            }
            set {
                this.applicationUnitOfMeasureField = value;
            }
        }
        
        /// <remarks/>
        public decimal ApplicationLoads {
            get {
                return this.applicationLoadsField;
            }
            set {
                this.applicationLoadsField = value;
            }
        }
        
        /// <remarks/>
        public decimal ApplicationAcresPerLoad {
            get {
                return this.applicationAcresPerLoadField;
            }
            set {
                this.applicationAcresPerLoadField = value;
            }
        }
        
        /// <remarks/>
        public decimal ApplicationAmountPerLoad {
            get {
                return this.applicationAmountPerLoadField;
            }
            set {
                this.applicationAmountPerLoadField = value;
            }
        }
        
        /// <remarks/>
        public string HQNotes {
            get {
                return this.hQNotesField;
            }
            set {
                this.hQNotesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/WorkOrderExtra.xsd")]
    public partial class Product {
        
        private string nameField;
        
        private bool customerSuppliedField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public bool CustomerSupplied {
            get {
                return this.customerSuppliedField;
            }
            set {
                this.customerSuppliedField = value;
            }
        }
    }
}
