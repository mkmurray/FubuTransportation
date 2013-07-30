﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FubuCore;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues;
using Rhino.Queues.Model;

namespace FubuTransportation.RhinoQueues.Testing
{
    [TestFixture]
    public class RhinoQueuesTransportTester : InteractionContext<RhinoQueuesTransport>
    {
        private RhinoQueuesSettings _settings;
        private IPersistentQueue _queue;

        protected override void beforeEach()
        {
            _queue = MockFor<IPersistentQueue>();
            _settings = new RhinoQueuesSettings();
            _settings.Queues.Add(new QueueSetting {QueueName = "test1", ThreadCount = 1});
            Services.Inject(_settings);
            Services.Container.Configure(x => x.For<IMessageSerializer>().Use<SimpleSerializer>());
        }

        [Test]
        public void starting_with_more_than_one_thread_on_single_queue_should_should_have_correct_started_threadcount()
        {
            _queue.Expect(x => x.Receive(Arg<string>.Is.Equal("test1")))
                .Return(new Message() {Data = new byte[] {}});
            var receiver = MockFor<IReceiver>();
            _settings.Queues[0].ThreadCount = 3;
            
            ClassUnderTest.StartReceiving(new ChannelOptions(), receiver);
            ClassUnderTest.ThreadCount.ShouldEqual(3);
        }

        [Test]
        public void starting_with_multiple_queues_should_should_have_correct_started_threadcount()
        {
            _queue.Expect(x => x.Receive(Arg<string>.Is.Anything))
                .Return(new Message() { Data = new byte[] { } });
            var receiver = MockFor<IReceiver>();
            var queue = new QueueSetting {QueueName = "test2", ThreadCount = 4};
            _settings.Queues.Add(queue);

            ClassUnderTest.StartReceiving(new ChannelOptions(), receiver);
            ClassUnderTest.ThreadCount.ShouldEqual(5);
        }

        [Test]
        public void starts_the_persistent_queue()
        {
            _queue.Expect(x => x.Receive(Arg<string>.Is.Anything))
                .Return(new Message() { Data = new byte[] { } });
            ClassUnderTest.StartReceiving(new ChannelOptions(), MockFor<IReceiver>());
            _queue.AssertWasCalled(x => x.Start());
        }

        [Test]
        public void matches_rhino_queues_positive()
        {
            ClassUnderTest.Matches(new Uri("rhino.queues://machine/something")).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_when_protocol_is_not_rhino_queues()
        {
            ClassUnderTest.Matches(new Uri("http://rhino.queues//something")).ShouldBeFalse();
        }

        [TearDown]
        public void Teardown()
        {
            ClassUnderTest.Dispose();
        }
    }

    public class SimpleSerializer : IMessageSerializer
    {
        public void Serialize(object message2, Stream stream)
        {
            var bytes = Encoding.UTF8.GetBytes(message2.As<string>());
            stream.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream message)
        {
            using (var reader = new StreamReader(message, Encoding.UTF8))
            {
                return new object[]{reader.ReadToEnd()};
            }
        }

        public string ContentType
        {
            get { return "text/plain"; }
        }
    }
}