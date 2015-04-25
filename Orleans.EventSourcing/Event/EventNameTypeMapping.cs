﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;
using Orleans.Concurrency;
using System.Collections.Concurrent;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Orleans.EventSourcing
{
    public class EventNameTypeMapping
    {
        private static readonly IDictionary<int, Type> eventTypeCodeMappings = new Dictionary<int, Type>();

        private static readonly object _lock = new object();

        public static bool TryGetEventType(int typeCode, out Type eventType)
        {
            return eventTypeCodeMappings.TryGetValue(typeCode, out eventType);
        }

        public static bool TryGetEventTypeCode(Type eventType, out int typeCode)
        {
            typeCode = (from kv in eventTypeCodeMappings where eventType == kv.Value select kv.Key).FirstOrDefault();

            return typeCode > 0;
        }

        public static void RegisterEventType(int typeCode, Type type)
        {
            lock (_lock)
            {
                if (IsEventType(type) && !eventTypeCodeMappings.ContainsKey(typeCode))
                    eventTypeCodeMappings.Add(typeCode, type);
            }
        }


        private static bool IsEventType(Type grainType)
        {
            return grainType.IsClass && !grainType.IsAbstract && typeof(IEvent).IsAssignableFrom(grainType);
        }
    }

}
