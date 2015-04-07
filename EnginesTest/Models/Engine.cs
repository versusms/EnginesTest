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
    
    public partial class Engine
    {
        public Engine()
        {
            this.Tests = new HashSet<Test>();
        }
    
        public int Id { get; set; }
        public string UID { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Configuration { get; set; }
        public int Cylinders { get; set; }
        public double EngineCapacity { get; set; }
        public int ValversPerCylinder { get; set; }
        public string FuelType { get; set; }
        public int RatedTorque { get; set; }
    
        public virtual ICollection<Test> Tests { get; set; }
    }
}
