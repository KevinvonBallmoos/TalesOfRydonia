using System;
using System.Diagnostics;
using System.IO;
using Code.Dialogue.Story;
using Code.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Code
{
    /// <summary>
    /// Is in Control of the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.01.2023</para>
    public class GameManager : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // Story UI
        private static StoryUI _storyUI;
        // StoryHolder
        private static StoryHolder _selectedStory;
        // GameManager
        public static GameManager Gm;
        // GameState
        public GameState State;
        public static event Action<GameState> OnGameStateChanged;
        // Ending Screen
        public GameObject endingScreen;

        [NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfStory;

        private int _chapter;
        private int _part;
        private string _runPath;
        private string _storyPath;
        
        public enum GameState
        {
            NewGame,
            LoadGame
        }

        private void Awake()
        {
            if (Gm == null)
                Gm = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            try
            {
                _runPath = $"{Application.dataPath}/Resources/";
                _storyUI = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>();
                _selectedStory = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();

                _chapter = 1;
            }
            catch (Exception ex)
            {
                _logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        public void UpdateGameStates(GameState newState)
        {
            State = newState;
            switch (newState)
            {
                case GameState.NewGame:
                    LoadNewGame();
                    break;
                case GameState.LoadGame:
                    //LoadSavedScene();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            OnGameStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// Starts a new Game
        /// </summary>
        public static void LoadNewGame()
        {
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Loads the saved Scene
        /// </summary>
        /// <param name="scene"></param>
        public static void LoadSavedScene(string scene)
        {
            SceneManager.LoadScene(int.Parse(scene[5].ToString()));
        }

        /// <summary>
        /// Checks if its Game Over or end of Chapter
        /// </summary>
        private void Update()
        {
            if (IsEndOfChapter)
                LoadNextChapter();
            if (IsEndOfStory)
                LoadNextStoryPart();
            if (IsGameOver)
                LoadGameOverScreen();
        }

        /// <summary>
        /// Loads next chapter
        /// </summary>
        private void LoadNextChapter()
        {
            IsEndOfChapter = false;
            _part = GetPath();
            _chapter++;
            _storyPath = $@"Story/Part{_part}/Story{_part}Chapter{_chapter}.asset";
            
            if (!File.Exists($@"{_runPath}{_storyPath}")) return;
            _selectedStory.selectedChapter = Resources.Load<Story>(_storyPath.Replace(".asset", ""));
            _selectedStory.Start();
            _logger.LogEntry("GameManager Log", $"Next chapter: Story{_part}Chapter{_chapter}", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Load next Story / next scene
        /// </summary>
        private void LoadNextStoryPart()
        {
            IsEndOfStory = false;
            _part++;
            _logger.LogEntry("GameManager Log", $"Next Story Part: Story{_part}Chapter{_chapter}",
                GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Load GameOver Screen
        /// </summary>
        private void LoadGameOverScreen()
        {
            IsGameOver = false;
            endingScreen.SetActive(true);
            _logger.LogEntry("GameManager Log", $"Game Over! ", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// When the next Chapter Button is clicked
        /// </summary>
        public void NextChapter_Click()
        {
            _storyUI.Start();
        }

        /// <summary>
        /// When the next Chapter Button is clicked
        /// </summary>
        public void NextStory_Click()
        {
            SceneManager.LoadScene(_part);
        }

        private int GetPath()
        {
            var path = _selectedStory.selectedChapter.name;
            foreach (var t in path)
            {
                if (char.IsDigit(t))
                    return int.Parse(t.ToString());
            }
            return 0;
        }
    }
}
