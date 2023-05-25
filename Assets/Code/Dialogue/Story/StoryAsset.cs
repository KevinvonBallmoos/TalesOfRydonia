﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Reloads the properties of the Story assets
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">10.05.2023</para>
    [CreateAssetMenu(fileName = "Chapter", menuName = "Viewer", order = 0)]
    public class StoryAsset : ScriptableObject
    {
        // Object
        private static List<Object> _storyAssets = new();
        // Lists with nodes
        private readonly List<NodeInfo> _nodes = new ();
        [field: SerializeField] public List<StoryNode> StoryNodes { get; private set; } = new();
        // Dictionary to store nodes 
        [NonSerialized] private readonly Dictionary<string, StoryNode> _nodeLookup = new ();
        // Boolean that is true when the nodes have been read
        [NonSerialized] public bool HasReadNodes;
        // Current Asset
        private StoryAsset _currentAsset;
        
        private class NodeInfo
        {
            public StoryNode Node { get; set; }
            public bool IsTrue { get; set; }
        }
        
        #region Reload StoryAssets
        
        /// <summary>
        /// Loads all Story Assets
        /// </summary>
        public static void LoadStoryObjects()
        {
            _storyAssets = Resources.LoadAll($@"Story/", typeof(StoryAsset)).ToList();
        }
        
        /// <summary>
        /// Reloads all Story Assets
        /// </summary>
        public IEnumerable ReloadStoryAssets(Object asset)
        {
            ReadNodes((StoryAsset)asset);
            yield return asset;
        }

        #endregion
        
        #region ReadNodes and Properties
        
        /// <summary>
        /// Reads the Nodes from the Xml File and puts them in the right order
        /// </summary>
        /// <param name="chapter"></param>
        public void ReadNodes(StoryAsset chapter)
        {
            _currentAsset = chapter;
            HasReadNodes = false;
            var xmlDoc = new XmlDocument();
            var xmlFile = Resources.Load<TextAsset>($"StoryFiles/{chapter.name}");
            xmlDoc.LoadXml(xmlFile.text);
            
            var choiceNodes = xmlDoc.GetElementsByTagName("Choice");
            foreach (XmlNode choice in choiceNodes)
            {
                var id = choice.Attributes?[0].Value;
                if (CheckNodes(id)) continue;
                var node = CreateNode(choice, id, true);
                _nodes.Add(new NodeInfo { Node = node, IsTrue = true });
            }

            var storyNodes = xmlDoc.GetElementsByTagName("Node");
            foreach (XmlNode story in storyNodes)
            {
                var id = story.Attributes?[0].Value;
                if (CheckNodes(id)) continue;
                var node = CreateNode(story, id, false);
                _nodes.Add(new NodeInfo{ Node = node, IsTrue = true });
            }

            var i = 0;
            while (i < _nodes.Count)
            {
                if (_nodes[i].IsTrue)
                {
                    i++;
                    continue;
                }

                foreach (var n in _nodes)
                {
                    if (n.Node.GetChildNodes().Contains(_nodes[i].Node.name))
                        n.Node.RemoveChildNode(_nodes[i].Node.name);
                }
                _nodes.RemoveAt(i);
            }
            
            StoryNodes.Clear();
            foreach (var n in _nodes)
            {
                ReadProperties(n.Node, xmlDoc);
                StoryNodes.Add(n.Node);
            }

            SaveChildNodes();
            SaveNodesToAssetDatabase();
            
            HasReadNodes = true;
        }
        
        /// <summary>
        /// Checks if the node already exists 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckNodes(string id)
        {
            foreach (var n in _nodes)
            {
                if (n.Node.GetNodeId() == id)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the Properties from the Xml to the according node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xmlDoc"></param>
        /// §
        private void ReadProperties(StoryNode node, XmlDocument xmlDoc)
        {
            var nodeList = node.IsChoiceNode()? xmlDoc.GetElementsByTagName("Choice"): xmlDoc.GetElementsByTagName("Node");
            var xmlNode = nodeList.Cast<XmlNode>().FirstOrDefault(n => node.GetText() == n.InnerText);
            int? attributesCount = xmlNode?.Attributes?.Count;

            if (attributesCount == 0) return;
            
            for (var i = 0; i < attributesCount; i++)
            {
                var attribute = xmlNode.Attributes?[i].Name;
                switch (attribute)
                {
                    case "node":
                        //if (isNew)
                            AddStoryChild(node, xmlNode.Attributes[attribute].Value);
                        break;
                    case "choice":
                        //if (isNew)
                            AddStoryChild(node, xmlNode.Attributes[attribute].Value);
                        break;
                    case "image":
                        node.SetImage(xmlNode.Attributes[attribute].Value);
                        break;
                    case "item":
                        node.SetItem(xmlNode.Attributes[attribute].Value);
                        break;
                    case "isRootNode":
                        node.SetIsRootNode(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isGameOver":
                        node.SetIsGameOver(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isEndOfChapter":
                        node.SetIsEndOfChapter(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isEndOfStory":
                        node.SetIsEndOfStory(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "background":
                        node.SetBackground(xmlNode.Attributes[attribute].Value);
                        break;
                }
            }
            
            foreach (var child in _nodes)
            {
                if (!child.Node.IsRootNode()) continue;
                child.Node.SetTextRect(child.Node.GetRect().x + 5, child.Node.GetRect().y + 5, child.Node.GetRect().width - 50, child.Node.GetRect().height -50);
                SetNodePosition(child.Node);
                break;
            }
        }

        /// <summary>
        /// Adds the Child/s to the parent Node
        /// </summary>
        private void AddStoryChild(StoryNode node, string id)
        {
            var ids = id.Split(',');
            foreach (var i in ids)
            {
                foreach (var n in _nodes)
                {
                    if (n.Node.GetNodeId() != i)continue;
                    node.AddChildNode(n.Node.name);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Saves all node ids in a Dictionary
        /// </summary>
        private void SaveChildNodes()
        {
            _nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                if (node != null)
                    _nodeLookup[node.name] = node;
            }
        }
        
        /// <summary>
        /// Sets the Position of all nodes
        /// </summary>
        /// <param name="node"></param>
        private void SetNodePosition(StoryNode node)
        {
            var children = node.GetChildNodes();
            if (children.Count == 0) return;

            for (var i = 0; i < children.Count; i++)
            {
                var child = node.GetChildNodes()[i];
                foreach (var n in _nodes)
                {
                    if (n.Node.name != child) continue;
                    n.Node.SetRect(node.GetRect().position.x + 350, node.GetRect().position.y + (i * 200));
                    n.Node.SetTextRect(n.Node.GetRect().x + 5, n.Node.GetRect().y + 5, n.Node.GetRect().width - 20, n.Node.GetRect().height -20);
                    SetNodePosition(n.Node);
                    break;
                }
            }
        }
        
        #endregion
        
        #region Create, Add Nodes

        /// <summary>
        /// Creates a new Node and a unique GUID
        /// saves the label, id, text and type of the node
        /// </summary>
        /// <param name="node">Node name</param>
        /// <param name="id">id Attribute in xml File</param>
        /// <param name="isChoice">Declares if Node is a choice or not</param>
        /// <returns>new Node</returns>
        private static StoryNode CreateNode(XmlNode node, string id, bool isChoice)
        {
            var child = CreateInstance<StoryNode>();
            child.name = Guid.NewGuid().ToString();
            if (node == null) return child;
            child.SetLabelAndNodeId(node.Name, id);
            child.SetText(node.InnerText);
            child.SetChoiceNode(isChoice);

            return child;
        }

        #endregion
        
        #region Getter and Setter
        
        /// <summary>
        /// Return the List of DialogueNodes
        /// </summary>
        /// <returns>nodes</returns>
        public IEnumerable<StoryNode> GetAllNodes()
        {
            return StoryNodes;
        }

        /// <summary>
        /// Returns all Child nodes from the Parent node
        /// </summary>
        /// <param name="parentNode">Parent Node to get the child nodes from</param>
        /// <returns>Child nodes of the parent Node</returns>
        public List<StoryNode> GetAllChildNodes(StoryNode parentNode)
        {
            var childNodes = new List<StoryNode>();
            foreach (var childID in parentNode.GetChildNodes())
            {
                // Checks the Node Dictionary if there is a key with this id
                if (_nodeLookup.ContainsKey(childID))
                    childNodes.Add(_nodeLookup[childID]);
            }
            return childNodes;
        }
        
        /// <summary>
        /// Returns RootNode
        /// </summary>
        /// <returns>if found rootNode else null</returns>
        public StoryNode GetRootNode()
        {
            foreach (var node in GetAllNodes())
            {
                if (node.IsRootNode())
                    return node;
            }
            return null;
        }
        
        /// <summary>
        /// Returns all Choice nodes from current node
        /// </summary>
        /// <param name="currentNode">Current Node to get the choice nodes from</param>
        /// <returns>All choice nodes of the current Node</returns>
        public IEnumerable<StoryNode> GetChoiceNodes(StoryNode currentNode)
        {
            foreach (var child in GetAllChildNodes(currentNode))
            {
                if (child.IsChoiceNode())
                    yield return child;
            }
        }
        
        /// <summary>
        /// Returns all Story nodes from current node
        /// </summary>
        /// <param name="currentNode">Current Node to get the story Nodes from</param>
        /// <returns>All story nodes of the current Node</returns>
        public IEnumerable<StoryNode> GetStoryNodes(StoryNode currentNode)
        {
            foreach (var child in GetAllChildNodes(currentNode))
            {
                if (!child.IsChoiceNode())
                    yield return child;
            }
        }

        public static List<Object> GetStoryAssets()
        {
            return _storyAssets;
        }

        #endregion
        
        #region AssetDatabase

        /// <summary>
        /// Adds nodes to Asset Database
        /// </summary>
        private void SaveNodesToAssetDatabase()
        {
            // Assuming 'savedDataPath' is the path to the asset containing the saved data
            var savedData = AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Story/" + _currentAsset.name + ".asset");

            foreach (var n in _nodes)
            {
                if (!savedData.Contains(n.Node))
                {
                    if (AssetDatabase.GetAssetPath(n.Node) == "")
                        AssetDatabase.AddObjectToAsset(n.Node, this);
                }
                else if (!n.IsTrue)
                    AssetDatabase.RemoveObjectFromAsset(n.Node);
            }
        }
        
        #endregion
    }
}