using System;
using RPG.Attributes;
using UnityEngine;
using Random = System.Random;

namespace UI
{
    public class DebugTextTest : MonoBehaviour
    {
        [SerializeField] GameObject target;

        [SerializeField] string dataString1;
        [SerializeField] string dataString2;
        [SerializeField] string dataString3;

        [SerializeField] float timeToDestroyText;

        public Character ch;
        
        void Awake()
        {
            ch = target.GetComponent<Character>() ?? throw new Exception($"Missing Character for {name}");
        }

        public void SendObjectDataText()
        {
            ch.Events.DebuggerClearCertainDebugText(12, EDebugType.SensorInThreatsList);
            ch.Events.DebuggerAddLine(12, EDebugType.SensorInThreatsList, dataString1 + " " + new Random().Next(0,11));
            ch.Events.DebuggerAddLine(12, EDebugType.SensorInThreatsList, dataString2 + " " + new Random().Next(0,11));
            ch.Events.DebuggerAddLine(12, EDebugType.SensorInThreatsList, dataString2 + " " + new Random().Next(0,11));
            print("Pressed SendObjectDataText");
        }

        public void SetDataOrder(int order)
        {
            ch.Events.DebuggerSetOrder(12, EDebugType.SensorInThreatsList, order);
            print("Pressed SetDataOrder");
        }
        
        public void ClearData()
        {
            ch.Events.DebuggerClearCertainDebugText(12, EDebugType.SensorInThreatsList);
            print("Pressed ClearData");
        }
        
        public void VisionInRange()
        {
            ch.Events.DebuggerAddText(13, EDebugType.SensorInVisionList, "In Vision Range");
            print("Pressed VisionInRange");
        }

        public void VisionOutRange()
        {
            ch.Events.DebuggerAddText(13, EDebugType.SensorInVisionList, "Out of Vision Range");
            print("Pressed VisionOutRange");
        }
        
        public void SetVisionOrder(int order)
        {
            ch.Events.DebuggerSetOrder(13, EDebugType.SensorInVisionList, order);
            print("Pressed SetVisionOrder");
        }
        
        public void ClearVision()
        {
            ch.Events.DebuggerClearCertainDebugText(13, EDebugType.SensorInVisionList);
            print("Pressed ClearVision");
        }
        
        public void Hearing(float volume)
        {
            ch.Events.DebuggerAddDestructibleLine(14, EDebugType.SensorInHearingList, "Noise, volume == " + volume, timeToDestroyText);
            print("Pressed Hearing");
        }
        
        public void SetHearingOrder(int order)
        {
            ch.Events.DebuggerSetOrder(14, EDebugType.SensorInHearingList, order);
            print("Pressed SetHearingOrder");
        }
        
        public void ClearHearing()
        {
            ch.Events.DebuggerClearCertainDebugText(14, EDebugType.SensorInHearingList);
            print("Pressed ClearHearing");
        }
        
        public void ClearAll()
        {
            ch.Events.DebuggerClearAllText();
            print("Pressed ClearAll");
        }
    }
}