﻿using System;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class RecordingServiceBusTester
    {
        [Test]
        public void send_records()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.Send(message);

            bus.Sent.Single().ShouldBeTheSameAs(message);
        }

        [Test]
        public void consumed_messages()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.Consume(message);

            bus.Consumed.Single().ShouldBeTheSameAs(message);
        }

        [Test]
        public void send_delayed_by_time_span()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.DelaySend(message, 5.Minutes());

            bus.DelayedSent.Single().Message.ShouldBeTheSameAs(message);
            bus.DelayedSent.Single().Delay.ShouldEqual(5.Minutes());
        }

        [Test]
        public void send_delayed_by_time()
        {
            var time = DateTime.Today.AddHours(5);

            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.DelaySend(message, time);

            bus.DelayedSent.Single().Message.ShouldBeTheSameAs(message);
            bus.DelayedSent.Single().Time.ShouldEqual(time);

        }

        [Test]
        public void send_and_wait()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.SendAndWait(message).Wait();

            // Checking for messages sent
            bus.Sent.Single().ShouldBeTheSameAs(message);

            // Checking for awaited calls
            bus.Await.Single().ShouldBeTheSameAs(message);
        }

        [Test]
        public void send_to_destination()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.Send(destination, message);

            var sentTo = bus.SentDirectlyTo.Single();
            sentTo.Destination.ShouldEqual(destination);
            sentTo.Message.ShouldBeTheSameAs(message);
        }
    }
}