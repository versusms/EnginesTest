﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DataModelContainer : DbContext
    {
        public DataModelContainer()
            // Подключение к LocalDB
            //: base("name=LocalDataModelContainer")
            // Подключение к удаленному MSSQL-серверу
            : base("name=RemoteDataModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Engine> Engines { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
    }
}
