//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnginesTest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Measurement
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int Torque { get; set; }
        public int RPM { get; set; }
        public double FuelConsumption { get; set; }
        public double TCoolant { get; set; }
        public double TOil { get; set; }
        public double TFuel { get; set; }
        public double TExhaustGas { get; set; }
        public double POil { get; set; }
        public double PExhaustGas { get; set; }
        public double PowerHP { get; set; }
        public double PowerKWh { get; set; }
    
        public virtual Test Test { get; set; }
    }
}
