﻿using System;
using System.Collections.Generic;

namespace FubuTransportation.Monitoring
{
    public interface IPersistentTask
    {
        Uri Subject { get; }
        void AssertAvailable();
        void Activate();
        void Deactivate();
        bool IsActive();

        ITransportPeer AssignOwner(IEnumerable<ITransportPeer> peers);
    }
}