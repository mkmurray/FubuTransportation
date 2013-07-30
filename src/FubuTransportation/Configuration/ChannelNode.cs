﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuTransportation.Runtime.Routing;
using System.Linq;

namespace FubuTransportation.Configuration
{
    public class ChannelGraph : IEnumerable<ChannelNode>
    {
        private readonly Cache<string, ChannelNode> _channels = new Cache<string, ChannelNode>(key => new ChannelNode{Key = key});

        public ChannelNode ChannelFor<T>(Expression<Func<T, object>> property)
        {
            return ChannelFor(property.ToAccessor());
        }

        public ChannelNode ChannelFor(Accessor accessor)
        {
            var key = ToKey(accessor);
            var channel = _channels[key];
            channel.SettingAddress = accessor;

            return channel;
        }

        public static string ToKey(Accessor accessor)
        {
            return accessor.OwnerType.Name.Replace("Settings", "") + ":" + accessor.Name;
        }

        public static string ToKey<T>(Expression<Func<T, object>> property)
        {
            return ToKey(property.ToAccessor());
        }

        public IEnumerator<ChannelNode> GetEnumerator()
        {
            return _channels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ChannelNode
    {
        public Accessor SettingAddress;
        public string Key;

        public int ThreadCount = 1;
        public bool Incoming = false;

        public IList<IRoutingRule> Rules = new List<IRoutingRule>();
 
        public bool Publishes(Type type)
        {
            return Rules.Any(x => x.Matches(type));
        }
        
    }

    
}