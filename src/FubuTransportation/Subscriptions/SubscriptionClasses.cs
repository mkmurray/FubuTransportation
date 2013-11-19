﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuTransportation.Subscriptions
{
    /*
     * ChannelGraph.ToTransportNode() : TransportNode
     * ChannelGraph.ToSubscriptionRequirements() : SubscriptionRequirement*
     * 
     * 
     * 
     * 
     */

    /// <summary>
    /// Sent to peer groups
    /// </summary>
    public class SubscriptionRequested
    {
        private readonly IList<Subscription> _subscriptions = new List<Subscription>();

        public Subscription[] Subscriptions
        {
            get { return _subscriptions.ToArray(); }
            set
            {
                _subscriptions.Clear();
                if (value != null) _subscriptions.AddRange(value);
            }
        }
    }

    /// <summary>
    /// Sent to "peer" nodes as a "coat check" message to refill subscriptions
    /// </summary>
    public class SubscriptionsChanged
    {
        
    }



    public class SubscriptionRequirement
    {
        public Uri Source { get; set; }
        public string MessageType { get; set; }
        public Uri Receiver { get; set; } // If local, this will get filled in

        protected bool Equals(SubscriptionRequirement other)
        {
            return Equals(Source, other.Source) && string.Equals(MessageType, other.MessageType) && Equals(Receiver, other.Receiver);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscriptionRequirement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MessageType != null ? MessageType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Receiver != null ? Receiver.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class Subscription
    {
        public Guid Id { get; set; }
        public Uri Source { get; set; }
        public Uri Receiver { get; set; }
        public string MessageType { get; set; }
    }

    public class NodeGroup
    {
        public string NodeName { get; set; }

        private readonly IList<TransportNode> _nodes = new List<TransportNode>();

        public TransportNode[] Nodes
        {
            get
            {
                return _nodes.ToArray();
            }
            set
            {
                _nodes.Clear();
                if (value != null) _nodes.AddRange(value);
            }
        }

        public void Add(TransportNode node)
        {
            _nodes.Add(node);
        }

        public void Remote(TransportNode node)
        {
            _nodes.Remove(node);
        }

        public TransportNode FindNode(Uri uri)
        {
            throw new NotImplementedException();
        }
    }

    public class TransportNode
    {
        private readonly IList<Uri> _addresses = new List<Uri>();

        public Guid Id { get; set; }

        public Uri[] Addresses
        {
            get
            {
                return _addresses.ToArray();
            }
            set
            {
                _addresses.Clear();
                if (value != null) _addresses.AddRange(value);
            }
        }

        protected bool Equals(TransportNode other)
        {
            // TODO: UT this little bad boy
            throw new NotImplementedException();
            //return _addresses.OrderBy(x => x.ToString()).SequenceEqual(other._addresses.OrderBy(x => x.ToString()));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TransportNode) obj);
        }

        public override int GetHashCode()
        {
            return (_addresses != null ? _addresses.GetHashCode() : 0);
        }
    }


}