using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    //This script use Instance IDs as keys for it main Dictionary
    //If some script need to send here different messages - it needs to generate unique ID's for each of them.
    
    [RequireComponent(typeof(CanvasGroup))]
    public class DebugTextController : MonoBehaviour
    {
        //Serializable

        //Cashed
        CanvasGroup _canvasGroup;
        Text _debugText;
        Character _ch; 
        
        //Local arrays
        Dictionary<DebugTypeAndID, DebugText> _textList = new Dictionary<DebugTypeAndID, DebugText>();
        
        //class DebugTypeAndSource
        //EDebugType - one per class
        //InstanceID - an array of them

        List<DebugTypeAndID> _debugTypeAndIDList = new List<DebugTypeAndID>();
        
        IOrderedEnumerable<KeyValuePair<DebugTypeAndID, DebugText>> _sortedList;
        List<string> _lines = new List<string>();
        List<DebugText.DebugTextLine> _textsToRemove = new List<DebugText.DebugTextLine>();
        List<DebugTypeAndID> _keysToRemove = new List<DebugTypeAndID>();
        Dictionary<EDebugType, int> _defaultEnumOrder = new Dictionary<EDebugType, int>();

        //Local variables
        bool _containSelfDestructText = false;

        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _canvasGroup = GetComponent<CanvasGroup>() ?? InstError<CanvasGroup>();
            _debugText = GetComponentInChildren<Text>() ?? InstError<Text>();
            _defaultEnumOrder = DebugTypeIndexDictionary.GetDictionary();
            _canvasGroup.alpha = 0;
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character) throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        

        void OnEnable()
        {
            _ch.Events.DebuggerClearAllText += ClearAllText;
            _ch.Events.DebuggerForceAddText += ForceAddText;
            _ch.Events.DebuggerClearCertainDebugText += ClearCertainDebugText;
            _ch.Events.DebuggerSetOrder += SetTextOrder;
            _ch.Events.DebuggerAddLine += AddLine;
            _ch.Events.DebuggerAddDestructibleLine += AddDestructibleLine;
            _ch.Events.DebuggerAddText += AddText;
            _ch.Events.DebuggerAddDestructibleText += AddDestructibleText;
        }

        void OnDisable()
        {
            _ch.Events.DebuggerClearAllText -= ClearAllText;
            _ch.Events.DebuggerForceAddText -= ForceAddText;
            _ch.Events.DebuggerClearCertainDebugText -= ClearCertainDebugText;
            _ch.Events.DebuggerSetOrder -= SetTextOrder;
            _ch.Events.DebuggerAddLine -= AddLine;
            _ch.Events.DebuggerAddDestructibleLine -= AddDestructibleLine;
            _ch.Events.DebuggerAddText -= AddText;
            _ch.Events.DebuggerAddDestructibleText -= AddDestructibleText;
        }

        void Start()
        {
            CanvasAlphaCheck();
            GetComponentInChildren<Image>().enabled = true;
        }

        void Update()
        {
            if (_containSelfDestructText)
                ReduceSelfDestructedTextTimers();
        }
        
        #region Local Debug methods

        void ReduceSelfDestructedTextTimers()
        {
            //we will change it back to TRUE if will find any self-destructive timer that >0
            _containSelfDestructText = false;
            
            //Check all the text lines in _textList
            foreach (var pair in _textList)
            {
                _textsToRemove.Clear(); //it's for text lines with timer <= 0
                _keysToRemove.Clear(); // it's for pair where no lines left or they have timer <= 0
                
                foreach (var textLine in pair.Value.debugTextLines)
                {
                    if (textLine.IsDestructible == false) continue;

                    textLine.TimeToDestroy -= Time.deltaTime;

                    if (textLine.TimeToDestroy <= 0)
                        _textsToRemove.Add(textLine);
                    else
                        _containSelfDestructText = true;
                }
                
                //remove lines with expired timer (value) from Dictionary pair
                foreach (var textToRemove in _textsToRemove)
                    pair.Value.debugTextLines.Remove(textToRemove);
                
                if (pair.Value.debugTextLines.Count == 0)
                    _keysToRemove.Add(pair.Key);
            }

            //if Dictionary pair value have no lines - remove it from Dictionary
            foreach (var keyToRemove in _keysToRemove)
                _textList.Remove(keyToRemove);
            
            PrintList();
        }


        [ContextMenu("Sort")]
        void SortList()
        {
            _sortedList = _textList.OrderBy(pair => pair.Value.TextOrder);
            _textList = _sortedList.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        
        [ContextMenu("PrintList")]
        void PrintList()
        {
            //clear previous text
            _debugText.text = string.Empty;
            
            //Hide or show debugger
            CanvasAlphaCheck();
            
            //print every string value from the Dictionary
            _lines.Clear();
            
            foreach (var pair in _textList)
                foreach (var textClass in pair.Value.debugTextLines)
                    _lines.Add(textClass.Text);
            
            _debugText.text = string.Join("\n", _lines);
        }

        
        //Anything in the dictionary? Or in the text field? Make it visible
        //No? Hide it
        [ContextMenu("CanvasAlphaCheck")]
        void CanvasAlphaCheck()
        {
            if (_textList.Count != 0 || _debugText.text != String.Empty)
                _canvasGroup.alpha = 1;
            else
                _canvasGroup.alpha = 0;
        }

        #endregion

        
        #region Public Debug methods

        //in case if need to add some text fast, without order or Enums
        public void ForceAddText(string text)
        {
            if (_debugText.text == string.Empty)
                _debugText.text = text;
            else
                _debugText.text += "\n" + text;
            
            CanvasAlphaCheck();
        }
        
        
        [ContextMenu("ClearAllText")]
        public void ClearAllText()
        {
            _debugText.text = String.Empty;
            _textList.Clear();
            _debugTypeAndIDList.Clear();
            CanvasAlphaCheck();
        }


        public void ClearCertainDebugText(int instanceID, EDebugType debugType)
        {
            var workTypeAndID = GetExistingTypeAndID(instanceID, debugType);
            
            if (workTypeAndID == null) return;
            
            if (_textList.ContainsKey(workTypeAndID))
            {
                _textList.Remove(workTypeAndID);
                _debugTypeAndIDList.Remove(workTypeAndID);
                PrintList();
            }
        }

        
        //todo need to change SetTextOrder logic
        //this will change order only for ID + debugType pare.
        //More reasonable will be change order of ALL debugType in _textList
        //Also need to create a mechanism and interface to change this order from the Inspector in debugTextController body.
        public void SetTextOrder(int instanceID, EDebugType debugType, int newOrder)
        {
            var workTypeAndID = GetTypeAndIDInstance(instanceID, debugType);
            
            if (_textList.ContainsKey(workTypeAndID))
                _textList[workTypeAndID].TextOrder = newOrder;

            SortList();
            PrintList();
        }
        

        //Add new line of text
        public void AddLine(int instanceID, EDebugType debugType, string text)
        {
            //Create new or get existing TypeAndID
            var workTypeAndID = GetTypeAndIDInstance(instanceID, debugType);
            
            //Nothing in Dictionary? Create new one and add a text line
            if (!_textList.ContainsKey(workTypeAndID))       
            {
                //_textList.Add(debugType, new DebugText(text));
                _textList.Add(workTypeAndID, new DebugText());
                _textList[workTypeAndID].AddText(text);
                _textList[workTypeAndID].TextOrder = _defaultEnumOrder[debugType];
                SortList();
            }
            //there is a text line + NO text in it? Add a text to existing line 
            else if (_textList[workTypeAndID].debugTextLines.Count == 1 && _textList[workTypeAndID].debugTextLines[0].Text == null)  
            {
                _textList[workTypeAndID].debugTextLines[0].Text = text;
            }
            //there is a text line + ANY text in it? Add a new line to it
            else                                        
            {
                _textList[workTypeAndID].AddText(text);
            }

            PrintList();
        }

        
        //Add new line of text
        //Add a self-destroy timer to this line
        public void AddDestructibleLine(int instanceID, EDebugType debugType, string text, float timeToDestroy)
        {
            var workTypeAndID = GetTypeAndIDInstance(instanceID, debugType);
            
            //Nothing in Dictionary? Create new one and add a text line
            //Set destructible timer to it 
            if (!_textList.ContainsKey(workTypeAndID))       
            {
                _textList.Add(workTypeAndID, new DebugText());
                _textList[workTypeAndID].AddDestructibleText(text, timeToDestroy);
                _textList[workTypeAndID].TextOrder = _defaultEnumOrder[debugType];
                SortList();
            }
            //there is a text line + NO text in it? Add a text to existing line 
            //Set destructible timer to it 
            else if (_textList[workTypeAndID].debugTextLines.Count == 1 && _textList[workTypeAndID].debugTextLines[0].Text == null)  //nothing in Text? Add it 
            {
                _textList[workTypeAndID].debugTextLines[0].Text = text;
                _textList[workTypeAndID].debugTextLines[0].IsDestructible = true;
                _textList[workTypeAndID].debugTextLines[0].TimeToDestroy = timeToDestroy;
            }
            //there is a text line + ANY text in it? Add a new line to it
            //Set destructible timer to it 
            else
            {
                _textList[workTypeAndID].AddDestructibleText(text, timeToDestroy);
            }

            //Activate Update, so destructible timer will count down every second
            _containSelfDestructText = true;
            
            PrintList();
        }
        
        
        //Add whole new text, not a line
        //Replace an old text if it was there
        public void AddText(int instanceID, EDebugType debugType, string text)
        {
            var workTypeAndID = GetTypeAndIDInstance(instanceID, debugType);
            
            //Nothing in Dictionary? Create new pair there and add a text
            //Set destructible timer to it 
            if (!_textList.ContainsKey(workTypeAndID))
            {
                _textList.Add(workTypeAndID, new DebugText());
                _textList[workTypeAndID].AddText(text);
                _textList[workTypeAndID].TextOrder = _defaultEnumOrder[debugType];
                SortList();
            }
            //Already in Dictionary? 
            //Replace all text there
            else
            {
                _textList[workTypeAndID].debugTextLines.Clear();
                _textList[workTypeAndID].AddText(text);
            }

            PrintList();
        }
        
        
        //Add whole new text, not a line
        //Replace an old text if it was there
        //Add a self-destroy timer to this text
        public void AddDestructibleText(int instanceID, EDebugType debugType, string text, float timeToDestroy)
        {
            var workTypeAndID = GetTypeAndIDInstance(instanceID, debugType);
            
            if (!_textList.ContainsKey(workTypeAndID))
            {
                _textList.Add(workTypeAndID, new DebugText());
                _textList[workTypeAndID].AddDestructibleText(text, timeToDestroy);
                _textList[workTypeAndID].TextOrder = _defaultEnumOrder[debugType];
                SortList();
            }
            else
            {
                _textList[workTypeAndID].debugTextLines.Clear();
                _textList[workTypeAndID].AddDestructibleText(text, timeToDestroy);
            }

            _containSelfDestructText = true;
            PrintList();
        }
        
        
        //create a new newTypeAndID in _debugTypeAndIDList by ID and debugType
        //if there already exist one - it will be returned
        DebugTypeAndID GetTypeAndIDInstance(int instanceID, EDebugType debugType)
        {
            foreach (var element in _debugTypeAndIDList)
            {
                if (element.InstanceID != instanceID) continue;
                if (element.DebugType != debugType) continue;
                return element;
            }

            var newTypeAndID = new DebugTypeAndID(instanceID, debugType);
            _debugTypeAndIDList.Add(newTypeAndID);
            return newTypeAndID;

        }

        
        DebugTypeAndID GetExistingTypeAndID(int instanceID, EDebugType debugType)
        {
            foreach (var element in _debugTypeAndIDList)
            {
                if (element.InstanceID != instanceID) continue;
                if (element.DebugType != debugType) continue;
                return element;
            }
            return null;
        }

        #endregion
    }
}