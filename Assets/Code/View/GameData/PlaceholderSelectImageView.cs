﻿using UnityEngine;
using UnityEngine.UI;

namespace Code.View.GameData
{
    /// <summary>
    /// Enables the Image on the clicked Save slot
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">05.05.2023</para>
    public class PlaceholderSelectImageView : MonoBehaviour
    {
        // Save slot view
        public GameObject placeholderView;

        /// <summary>
        /// When a save placeholder is selected enables the image
        /// </summary>
        public void GameObject_Click()
        { 
            SetImage();
        }
        
        /// <summary>
        /// Disables the select Image on all saves, when a save is selected
        /// Enables the select Image on the current holders game object
        /// </summary>
        private void SetImage()
        {
            // Disable all Images
            var holders = placeholderView.GetComponentsInChildren<Image>();
            for (var i = 0; i < holders.Length; i++)
            {
                if (i is 1 or 3 or 5)
                    holders[i].enabled = false;
            }
            // Enable Image of current game object 
            gameObject.GetComponentsInChildren<Image>()[1].enabled = true;
        }
    }
}