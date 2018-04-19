using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repositories;
using MySql.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Entities;
using Entities.Tenant;
using MySQL.Data.Entity.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccess
{
    public class DispatchApidbContext : DbContext
    {
        private readonly AppTenant tenant;

        public virtual DbSet<Subscriber> Subscriber { get; set; }
        public virtual DbSet<DispatchMessage> DispatchMessages { get; set; }
        public virtual DbSet<DispatchDestinationAddresses> DispatchDestinationAddresses { get; set; }
        public virtual DbSet<SubscriberByOriginationDestination> SubscriberByOriginationDestination { get; set; }
        public virtual DbSet<DispatchUnmatchedMessages> DispatchUnmatchedMessages { get; set; }
        public virtual DbSet<SubscriberInfoBySubscriberIds> SubscriberInfoBySubscriberIds { get; set; }
        public virtual DbSet<DispatchCenters> DispatchCenters { get; set; }

        public DispatchApidbContext(DbContextOptions options, AppTenant tenant) : base(options)
        {
            this.tenant = tenant;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(tenant.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Subscriber>()
              .ToTable("tbl_subscribers");
            modelBuilder.Entity<DispatchMessage>()
            .ToTable("dispatchmessages");
            modelBuilder.Entity<DispatchDestinationAddresses>()
            .ToTable("dispatchdestinationaddresses");
            modelBuilder.Entity<DispatchUnmatchedMessages>()
               .ToTable("dispatchunmatchedmessages");
            modelBuilder.Entity<DispatchCenters>()
              .ToTable("dispatchcenters");
            modelBuilder.Entity<SubscriberByOriginationDestination>()
                .ToTable("subscriberbyoriginationdestination");

        }
    }
}
