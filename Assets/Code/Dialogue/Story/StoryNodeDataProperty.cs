using System.Collections.Generic;

namespace Code.Dialogue.Story
{
    public abstract class StoryNodeDataProperty
    {
        protected StoryNodeDataProperty(
            string nodeId, 
            string labelText, 
            string text, 
            bool isChoiceNode, 
            bool isRootNode, 
            bool isEndOfStory, 
            bool isEndOfChapter, 
            bool isGameOver, 
            string image, 
            string item, 
            string background, 
            List<string> childNodes)
        {
            NodeId = nodeId;
            LabelText = labelText;
            Text = text;
            IsChoiceNode = isChoiceNode;
            IsRootNode = isRootNode;
            IsEndOfStory = isEndOfStory;
            IsEndOfChapter = isEndOfChapter;
            IsGameOver = isGameOver;
            Image = image;
            Item = item;
            Background = background;
            ChildNodes = childNodes;
        }

        public string NodeId { get; }
        public string LabelText { get; }
        public string Text { get; }
        public bool IsChoiceNode { get; }
        public bool IsRootNode { get; }
        public bool IsEndOfStory { get; }
        public bool IsEndOfChapter { get; }
        public bool IsGameOver { get; }
        public string Image { get; }
        public string Item { get; }
        public string Background { get; }
        public List<string> ChildNodes { get; }
    }
}