﻿using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_volunteers_xml = "volunteers.xml";

    /// <summary>
    /// Gets the next assignment ID and increments it in the configuration.
    /// </summary>
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    /// <summary>
    /// Gets the next call ID and increments it in the configuration.
    /// </summary>
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    /// <summary>
    /// Gets or sets the current clock time in the configuration.
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    /// <summary>
    /// Gets or sets the risk range time span in the configuration.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    /// <summary>
    /// Resets the configuration values to their defaults.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextAssignmentId = 1000;
        NextCallId = 1000;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(2);
    }
}
