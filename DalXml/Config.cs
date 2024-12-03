﻿namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_volunteers_xml = "volunteers.xml";


    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }


    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    internal static void Reset()
    {
        NextAssignmentId = 1000;
        NextCallId= 1000;
        Clock = DateTime.Now;
        RiskRange= TimeSpan.FromHours(2);
    }

}
