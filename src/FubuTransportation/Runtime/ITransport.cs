﻿using System;
using System.Collections.Generic;
using FubuCore;
using FubuTransportation.Configuration;

namespace FubuTransportation.Runtime
{
    public interface IChannel : IDisposable
    {
        Uri Address { get; }
        void StartReceiving(ChannelOptions options, IReceiver receiver);
    }

    public interface ITransport : IDisposable, IChannel
    {
        // Really for identification

        // Envelope might have a reference to its parent
        //void Send(Uri destination, Envelope envelope);

        // Nope, change this to matching on protocol
        bool Matches(Uri uri);

        
    }
}