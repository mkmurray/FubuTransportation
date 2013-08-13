﻿using FubuMVC.Core;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Runtime;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using StructureMap;
using FubuMVC.StructureMap;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.Runtime
{
    [TestFixture]
    public class ChannelRouterTester
    {
        private FubuRuntime runtime;
        private IChannelRouter theRouter;
        private HarnessSettings settings;

        [SetUp]
        public void SetUp()
        {
            var container = new Container();
            settings = InMemoryTransport.ToInMemory<HarnessSettings>();
            container.Inject(settings);

            runtime = FubuTransport.For<RoutedRegistry>().StructureMap(container).Bootstrap();

            theRouter = runtime.Factory.Get<IChannelRouter>();
        }

        [Test]
        public void if_destination_is_set_on_the_envelope_that_is_the_only_channel_returned()
        {
            var envelope = new Envelope
            {
                Message = new Message1(),
                Destination = settings.Service4
            };

            theRouter.FindChannels(envelope).Single().Uri.ShouldEqual(settings.Service4);
        }

        [Test]
        public void destination_is_specified_but_The_channel_does_not_exist()
        {
            Exception<UnknownChannelException>.ShouldBeThrownBy(() => {
                var envelope = new Envelope
                {
                    Message = new Message1(),
                    Destination = "unknown://uri".ToUri()
                };

                theRouter.FindChannels(envelope);
            });
        }

        [Test]
        public void use_type_rules_on_the_channel_graph_1()
        {
            var envelope = new Envelope {Message = new Message1()};
            theRouter.FindChannels(envelope).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("Harness:Service1");
        }

        [Test]
        public void use_type_rules_on_the_channel_graph_2()
        {
            var envelope = new Envelope { Message = new Message2() };
            theRouter.FindChannels(envelope).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("Harness:Service1", "Harness:Service3");
        }

        [Test]
        public void use_type_rules_on_the_channel_graph_3()
        {
            var envelope = new Envelope { Message = new Message3() };
            theRouter.FindChannels(envelope).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("Harness:Service2", "Harness:Service3");
        }
    }

    public class RoutedRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public RoutedRegistry()
        {
            Channel(x => x.Service1).PublishesMessage<Message1>();
            Channel(x => x.Service1).PublishesMessage<Message2>();

            Channel(x => x.Service2).PublishesMessage<Message3>();
            Channel(x => x.Service2).PublishesMessage<Message4>();
            
            Channel(x => x.Service3).PublishesMessage<Message2>();
            Channel(x => x.Service3).PublishesMessage<Message3>();

            Channel(x => x.Service4).ReadIncoming();

        }
    }
}