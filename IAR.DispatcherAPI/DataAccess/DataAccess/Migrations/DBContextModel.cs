using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccess.Migrations
{
    [DbContext(typeof(DispatchApidbContext))]
    public class DBContextModel : ModelSnapshot
    {
        public virtual DbSet<Subscriber> Subscriber { get; set; }
        public virtual DbSet<DispatchMessage> DispatchMessage { get; set; }
        public virtual DbSet<DispatchDestinationAddresses> DispatchDestinationAddresses { get; set; }
        public virtual DbSet<SubscriberByOriginationDestination> SubscriberByOriginationDestination { get; set; }
        public virtual DbSet<DispatchUnmatchedMessages> DispatchUnmatchedMessages { get; set; }
        public virtual DbSet<SubscriberInfoBySubscriberIds> SubscriberInfoBySubscriberIds { get; set; }
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            BuilderSubcriber(modelBuilder);
            BuilderDispatchMessage(modelBuilder);
            BuilderDispatchDestination(modelBuilder);
            BuilderDispatchUnmatchedMessages(modelBuilder);
            BuilderSubscriberInfoBySubscriber(modelBuilder);
            BuilderDispatchCenters(modelBuilder);
        }

        private void BuilderSubcriber(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscriber>()
               .ToTable("tbl_subscribers");
            modelBuilder.Entity<Subscriber>()
                .Property(s => s.Id);
            modelBuilder.Entity<Subscriber>()
                .Property(s => s.Subscribername);
            modelBuilder.Entity<Subscriber>()
                .Property(s => s.Usedispatchfromattachment);

        }

        private void BuilderDispatchMessage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DispatchMessage>()
             .ToTable("dispatchmessages");
            modelBuilder.Entity<DispatchMessage>()
                .Property(s => s.Id);
            modelBuilder.Entity<DispatchMessage>()
                .Property(s => s.Arrivedon);
            modelBuilder.Entity<DispatchMessage>()
               .Property(s => s.Messageheader);
            modelBuilder.Entity<DispatchMessage>()
               .Property(s => s.Messagebody);
            modelBuilder.Entity<DispatchMessage>()
                .Property(a => a.Subscriberid);
            modelBuilder.Entity<DispatchMessage>()
               .Property(s => s.Destinationemailaddress);
            modelBuilder.Entity<DispatchMessage>()
               .Property(s => s.Originationemailaddress);
            modelBuilder.Entity<DispatchMessage>()
              .Property(s => s.Messagesubject);
        }

        private void BuilderDispatchDestination(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DispatchDestinationAddresses>()
           .ToTable("dispatchdestinationaddresses");
            modelBuilder.Entity<DispatchDestinationAddresses>()
                .Property(s => s.Id);
            modelBuilder.Entity<DispatchDestinationAddresses>()
                .Property(a => a.Subscriberid);
            modelBuilder.Entity<DispatchDestinationAddresses>()
               .Property(s => s.Emailaddress);
            modelBuilder.Entity<DispatchDestinationAddresses>()
               .Property(s => s.Name);
            modelBuilder.Entity<DispatchDestinationAddresses>()
              .Property(s => s.cr_isdeleted);
            modelBuilder.Entity<DispatchDestinationAddresses>()
              .Property(s => s.cr_lastupdated);
        }

        private void BuilderDispatchUnmatchedMessages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .ToTable("dispatchunmatchedmessages");
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Id);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Arrivedon);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Messageheader);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Messagesubject);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Messagebody);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Messagefrom);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Messageto);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Destsubscriberid);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Destsubscribername);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Destsubscriberstatusid);
            modelBuilder.Entity<DispatchUnmatchedMessages>()
                .Property(s => s.Destsubscriberstatusname);

        }

        private void BuilderSubscriberInfoBySubscriber(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Mailingcountry);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Fullname);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Id);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Loginname);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Mailingstate);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Statusid);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Statusname);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Subscribername);
            modelBuilder.Entity<SubscriberInfoBySubscriberIds>()
               .Property(s => s.Subscribertypeid);

        }

        private void BuilderDispatchCenters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DispatchCenters>()
               .ToTable("dispatchcenters");
            modelBuilder.Entity<DispatchCenters>()
                .Property(s => s.Id);
            modelBuilder.Entity<DispatchCenters>()
                .Property(s => s.ApiKey);
            modelBuilder.Entity<DispatchCenters>()
                .Property(s => s.DispatcherMasterName);
            modelBuilder.Entity<DispatchCenters>()
                .Property(s => s.DispatcherUserPassword);

        }
    }
}
