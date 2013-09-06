﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuTransportation
{
    /// <summary>
    /// This is a Recording Stub implementation of IServiceBus suitable for
    /// unit testing
    /// </summary>
    public class RecordingServiceBus : IServiceBus
    {
        public readonly IList<object> Sent = new List<object>();
        public readonly IList<object> Consumed = new List<object>();
        public readonly IList<DelayedMessage> DelayedSent = new List<DelayedMessage>(); 

        public Task<TResponse> Request<TResponse>(object request, TimeSpan? timeout = null)
        {
            throw new NotSupportedException();
        }

        public void Send<T>(T message)
        {
            Sent.Add(message);
        }

        public void Consume<T>(T message)
        {
            Consumed.Add(message);
        }

        public void DelaySend<T>(T message, DateTime time)
        {
            DelayedSent.Add(new DelayedMessage
            {
                Message = message,
                Time = time
            });
        }

        public void DelaySend<T>(T message, TimeSpan delay)
        {
            DelayedSent.Add(new DelayedMessage
            {
                Message = message,
                Delay = delay
            });
        }

        public class DelayedMessage
        {
            public TimeSpan Delay;
            public DateTime Time;
            public object Message;
        }
    }

    
}