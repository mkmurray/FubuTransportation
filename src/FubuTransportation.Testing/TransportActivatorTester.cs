﻿using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Subscriptions;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class when_activating_the_transport_subsystem : InteractionContext<TransportActivator>
    {
        private ChannelGraph theGraph;

        protected override void beforeEach()
        {
            theGraph = MockFor<ChannelGraph>();
            Services.PartialMockTheClassUnderTest();

             ClassUnderTest.Stub(x => x.OpenChannels());
            ClassUnderTest.Stub(x => x.ExecuteActivators());

            ClassUnderTest.Activate(new IPackageInfo[0], new PackageLog());
        }

        [Test]
        public void reads_the_settings()
        {
            theGraph.AssertWasCalled(x => x.ReadSettings(MockFor<IServiceLocator>()));
        }

        [Test]
        public void should_start_the_channels()
        {
            ClassUnderTest.AssertWasCalled(x => x.OpenChannels());
        }

        [Test]
        public void should_start_receiving()
        {
            theGraph.AssertWasCalled(x => x.StartReceiving(MockFor<IHandlerPipeline>()));
        }

        [Test]
        public void should_invoke_activators()
        {
            ClassUnderTest.AssertWasCalled(x => x.ExecuteActivators());
        }
    }

    [TestFixture]
    public class when_starting_the_subscriptions : InteractionContext<TransportActivator>
    {
        private ChannelGraph theGraph;
        private ITransport[] theTransports;

        protected override void beforeEach()
        {
            theGraph = MockFor<ChannelGraph>();
            theTransports = Services.CreateMockArrayFor<ITransport>(5);

            ClassUnderTest.OpenChannels();
        }


        [Test]
        public void starts_each_transport()
        {
            theTransports.Each(transport => transport.AssertWasCalled(x => x.OpenChannels(theGraph)));
        }
    }

    [TestFixture]
    public class when_starting_the_subscriptions_and_there_are_unknown_channels
    {
        [Test]
        public void should_throw_an_exception_listing_the_channels_that_are_missing()
        {
            var graph = new ChannelGraph();
            graph.Add(new ChannelNode
            {
                Key = "Foo:1",
                Uri = "foo://1".ToUri()
            });

            graph.Add(new ChannelNode
            {
                Key = "Foo:2",
                Uri = "foo://2".ToUri()
            });

            var subscriptions = new TransportActivator(graph, null, null,
                new ITransport[] {new InMemoryTransport()},
                Enumerable.Empty<IFubuTransportActivator>());


            Exception<InvalidOrMissingTransportException>.ShouldBeThrownBy(subscriptions.OpenChannels);
        }
    }

    [TestFixture]
    public class when_starting_the_subscriptions_and_there_are_activators :
        InteractionContext<TransportActivator>
    {
        private IFubuTransportActivator[] theActivators;

        protected override void beforeEach()
        {
            theActivators = Services.CreateMockArrayFor<IFubuTransportActivator>(5);
            ClassUnderTest.ExecuteActivators();
        }

        [Test]
        public void invokes_each_activator()
        {
            theActivators.Each(activator => activator.AssertWasCalled(x => x.Activate()));
        }
    }
}