﻿using Serilog.Core;
using Serilog.Events;

namespace KoshelekTestTask.Infrastructure.Loggers;

public class LogEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
    {
        le.RemovePropertyIfPresent("SourceContext");
        le.RemovePropertyIfPresent("RequestId");
        le.RemovePropertyIfPresent("RequestPath");
        le.RemovePropertyIfPresent("ActionId");
        le.RemovePropertyIfPresent("ActionName");

        le.AddPropertyIfAbsent(lepf.CreateProperty("MachineName", Environment.MachineName));
    }
}