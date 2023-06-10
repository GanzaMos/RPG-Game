using System;
using System.Collections.Generic;

namespace UI
{
    public static class DebugTypeIndexDictionary
    {
        public static Dictionary<EDebugType, int> GetDictionary()
        {
            Array enumValues = Enum.GetValues(typeof(EDebugType));
            Dictionary<EDebugType, int> enumIndexDict = new Dictionary<EDebugType, int>();

            for (int i = 0; i < enumValues.Length; i++)
            {
                EDebugType enumValue = (EDebugType) enumValues.GetValue(i);
                enumIndexDict[enumValue] = i;
            }
            return enumIndexDict;
        }
    }


    public enum EDebugType
    {
        //main sensor lists
        SensorInEdgeList,
        SensorInHearingList,
        SensorInVisionList,
        SensorInSenseList,
        SensorInThreatsList,
        
        //detectors
        SensorInEdgeListDetector,
        SensorInVisionRadiusDetector,
        SensorInHearingRadiusDetector,
        SensorInSenseRadiusDetector,
        SensorInThreatListDetector,
        SensorVisionDetector,
        SensorSenseDetector,
        
        //base sensors
        SensorMadeSound,
        SensorAcceptedSound,
        SensorVisionField,
        SensorSenseField,
        
        //awareness
        SensorAwareness,
        SensorAwarenessDetector,
        
        //AIMemory
        AIMemory
    }
}