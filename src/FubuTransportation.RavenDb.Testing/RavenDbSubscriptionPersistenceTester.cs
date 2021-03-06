﻿using System;
using System.Diagnostics;
using System.Linq;
using Amazon.SimpleWorkflow.Model;
using FubuMVC.Core;
using FubuMVC.RavenDb;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Subscriptions;
using FubuTransportation.Testing;
using NUnit.Framework;
using StructureMap;

namespace FubuTransportation.RavenDb.Testing
{
    [TestFixture]
    public class RavenDbSubscriptionPersistenceTester
    {
        private FubuRuntime runtime;
        private RavenDbSubscriptionPersistence persistence;

        [SetUp]
        public void SetUp()
        {
            runtime = FubuTransport.DefaultPolicies().StructureMap().Bootstrap();
            runtime.Factory.Get<IContainer>().UseInMemoryDatastore();

            persistence = runtime.Factory.Get<RavenDbSubscriptionPersistence>();
        }

        [TearDown]
        public void TearDown()
        {
            runtime.Dispose();
        }

        [Test]
        public void persist_and_load_all_new_subscriptions()
        {
            var subscriptions = new Subscription[]
            {
                ObjectMother.ExistingSubscription("Node1"),
                ObjectMother.ExistingSubscription("Node1"),
                ObjectMother.ExistingSubscription("Node1"),
                ObjectMother.ExistingSubscription("Node1"),
                ObjectMother.ExistingSubscription("Node1")
            };

            var subscriptions2 = new Subscription[]
            {
                ObjectMother.ExistingSubscription("Node2"),
                ObjectMother.ExistingSubscription("Node2"),
            };

            persistence.Persist(subscriptions);
            persistence.Persist(subscriptions2);

            var loaded = persistence.LoadSubscriptions("Node1", SubscriptionRole.Subscribes);
            loaded.ShouldHaveTheSameElementsAs(subscriptions);

            persistence.LoadSubscriptions("Node2", SubscriptionRole.Subscribes)
                .ShouldHaveTheSameElementsAs(subscriptions2);
        }

        [Test]
        public void persist_and_load_transport_nodes()
        {
            var node1 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box1",
                NodeName = "Node1",
                Addresses = new Uri[] { "memory://1".ToUri(), "memory://2".ToUri() }
            };

            persistence.Persist(node1);

            var node2 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box2",
                NodeName = "Node2",
                Addresses = new Uri[] { "memory://3".ToUri(), "memory://4".ToUri() }
            };

            persistence.Persist(node2);

            persistence.NodesForGroup("Node1").Single()
                .ShouldEqual(node1);

            persistence.NodesForGroup("Node2").Single()
                .ShouldEqual(node2);
        }

        [Test]
        public void load_a_specific_node()
        {
            var node1 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box1",
                NodeName = "Node1",
                Addresses = new Uri[] { "memory://1".ToUri(), "memory://2".ToUri() }
            };


            var node2 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box2",
                NodeName = "Node2",
                Addresses = new Uri[] { "memory://3".ToUri(), "memory://4".ToUri() }
            };

            persistence.Persist(node1, node2);

            persistence.LoadNode(node2.Id)
                .MachineName.ShouldEqual(node2.MachineName);

            persistence.LoadNode(node1.Id)
                .MachineName.ShouldEqual(node1.MachineName);
        }

        [Test]
        public void alter_a_node()
        {
            var node1 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box1",
                NodeName = "Node1",
                Addresses = new Uri[] { "memory://1".ToUri(), "memory://2".ToUri() }
            };


            var node2 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box2",
                NodeName = "Node2",
                Addresses = new Uri[] { "memory://3".ToUri(), "memory://4".ToUri() }
            };

            var subject = "foo://1".ToUri();

            persistence.Persist(node1, node2);


            persistence.Alter(node1.Id, x => x.AddOwnership(subject));

            persistence.LoadNode(node1.Id).OwnedTasks
                .ShouldContain(subject);
        }
    }
}