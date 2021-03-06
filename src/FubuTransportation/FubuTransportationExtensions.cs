﻿using FubuMVC.Core;
using FubuMVC.Core.Registration.Conventions;
using FubuTransportation.Async;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Monitoring;
using FubuTransportation.Polling;
using FubuTransportation.Registration.Nodes;
using FubuTransportation.Sagas;
using FubuTransportation.ScheduledJobs;
using FubuTransportation.ScheduledJobs.Configuration;

namespace FubuTransportation
{
    public class FubuTransportationExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Add<ImportHandlers>();
            registry.Policies.Add<ApplyScheduledJobRouting>();
            registry.Services<FubuTransportServiceRegistry>();
            registry.Services<PollingServicesRegistry>();
            registry.Services<ScheduledJobServicesRegistry>();
            registry.Services<MonitoringServiceRegistry>();
            registry.Policies.Add<RegisterPollingJobs>();
            registry.Policies.Add<RegisterScheduledJobs>();
            registry.Policies.Add<StatefulSagaConvention>();
            registry.Policies.Add<AsyncHandlingConvention>();

            if (FubuTransport.AllQueuesInMemory)
            {
                registry.Policies.Add<AllQueuesInMemoryPolicy>();
            }

            registry.Policies.Add<InMemoryQueueRegistration>();

            registry.Policies.Add<ReorderBehaviorsPolicy>(x => {
                x.ThisNodeMustBeBefore<StatefulSagaNode>();
                x.ThisNodeMustBeAfter<HandlerCall>();
            });
        }
    }



}