using System;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public class DialogueNode 
    {
        public string uniqueID;
        public string text;
        public string[] children;
        public Rect rect = new Rect(0,0, 200, 100);

    }

}
