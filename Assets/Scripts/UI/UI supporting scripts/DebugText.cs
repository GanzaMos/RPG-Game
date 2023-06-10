using System;
using System.Collections.Generic;

namespace UI
{
    //Uses in DebugTextController
    //It's a line or several line of text
    //Text can be set in specific order with each other
    //Text can be self destructed 
    public class DebugText
    {
        int _textOrder = 10;
        public int TextOrder
        {
            get => _textOrder;
            set => _textOrder = value;
        }
        
        public List<DebugTextLine> debugTextLines = new List<DebugTextLine>();
        public class DebugTextLine
        {
            public string Text { get; set; }
            public bool IsDestructible { get; set; }
            public float TimeToDestroy { get; set; }
        }
        
        //set text
        //no timer
        public void AddText(string text)
        {
            var textClass = new DebugTextLine();
            debugTextLines.Add(textClass);
            
            textClass.Text = text;
        }

        //set text
        //set timer and IsDestructible flag
        public void AddDestructibleText(string text, float timeToDestroy)
        {
            var textClass = new DebugTextLine();
            debugTextLines.Add(textClass);
            
            textClass.Text = text;
            textClass.IsDestructible = true;
            textClass.TimeToDestroy = timeToDestroy;
        }
    }
}