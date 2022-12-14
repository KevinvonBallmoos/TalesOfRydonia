﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Code.Logger;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Class for StoryNode
    /// </summary>
    /// <para name="author">Kevin von Ballmoos></para>
    /// <para name="date">04.12.2022</para>
    public class StoryNode : ScriptableObject
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryNode");
        
        [NonSerialized] public string Text;

        private bool _isChoiceNode = false;
        [SerializeField] private bool isRootNode = false;
        
        [SerializeField] private List<string> childNodes = new List<string>();
        [SerializeField] private Rect storyRect = new (10, 10, 300, 150);
        
#if UNITY_EDITOR
        
        /// <summary>
        /// Sets the value of isStoryChoice to true or false
        /// </summary>
        /// <param name="newIsStoryChoice"></param>
        public void SetChoiceNode(bool newIsStoryChoice)
        {
            Undo.RecordObject(this, "Change Story or Dialogue");
            _isChoiceNode = newIsStoryChoice;
            EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Adds ChildNode
        /// </summary>
        /// <param name="childId"></param>
        public void AddChildNode(string childId)
        {
            Undo.RecordObject(this, "Add Story Link");
            childNodes.Add(childId);
            EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Removes ChildNode
        /// </summary>
        /// <param name="childId"></param>
        public void RemoveChildNode(string childId)
        {
            Undo.RecordObject(this, "Remove Story Link");
            childNodes.Remove(childId);
            EditorUtility.SetDirty(this);
        }

#endif
        
        /// <summary>
        /// Sets the rect to a new position
        /// </summary>
        /// <param name="vector"></param>
        public void SetRect(float x, float y)
        {
            Undo.RecordObject(this, "Move Story Node");
            storyRect.position = new Vector2(x,y);
        }
        
        public bool IsChoiceNode()
        {
            return _isChoiceNode;
        }
        
        public bool IsRootNode()
        {
            return isRootNode;
        }

        public string GetText()
        {
            return Text;
        }

        public List<string> GetChildNodes()
        {
            return childNodes;
        }

        public Rect GetRect()
        {
            return storyRect;
        }

        public Rect GetRect(Vector2 pos)
        {
            storyRect.position = storyRect.position + pos;
            return storyRect;
        }
    }
}