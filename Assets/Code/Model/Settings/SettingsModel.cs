using System.Collections.Generic;
using Code.Controller.FileController;

namespace Code.Model.Settings
{
    /// <summary>
    /// Handles the Settings 
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public static class SettingsModel
    {
        /// <summary>
        /// Creates a SettingsSerializeModel object to store the properties
        /// Call the json controller to save the settings
        /// </summary>
        public static void SaveSettings()
        {
            SettingsSerializeModel serializableSettings = new SettingsSerializeModel
            {
                FpsValue = SettingsInfoModel.FpsValue,
                WindowSize = SettingsInfoModel.WindowSize,
                AllVolume = SettingsInfoModel.AllVolume,
                VfxVolume = SettingsInfoModel.VfxVolume,
                MusicVolume = SettingsInfoModel.MusicVolume,
                IsTextSlowed = SettingsInfoModel.IsTextSlowed,
                LightFlicker = SettingsInfoModel.LightFlicker
            };
            
            JsonController.SaveSettings(serializableSettings);
        }

        /// <summary>
        /// Calls the json controller to load the settings
        /// assigns the values back to their properties
        /// </summary>
        public static void LoadSettings()
        {
            var settings = JsonController.LoadSettings();
            if (settings == null)
            {
                InitializeDefaultValues();
                FillLists();
                return;
            }

            // Video
            SettingsInfoModel.FpsValue = settings.FpsValue;
            SettingsInfoModel.WindowSize = settings.WindowSize;
            // Audio
            SettingsInfoModel.AllVolume.SettingValue = 100;
            SettingsInfoModel.VfxVolume.SettingValue = 100;
            SettingsInfoModel.MusicVolume.SettingValue =100;
            // In Game
            SettingsInfoModel.IsTextSlowed = settings.IsTextSlowed;
            SettingsInfoModel.LightFlicker = settings.LightFlicker;

            FillLists();
        }

        private static void InitializeDefaultValues()
        {
            // Video
            SettingsInfoModel.FpsValue = new SettingsInfoModel.Setting<int>{SettingName = nameof(SettingsInfoModel.FpsValue), InfoLabelText = "Recommended", SettingValue = 240};
            SettingsInfoModel.WindowSize = new SettingsInfoModel.Setting<string>{SettingName = nameof(SettingsInfoModel.WindowSize), InfoLabelText = "Recommended", SettingValue = "1920x1080"};
            // Audio
            SettingsInfoModel.AllVolume = new SettingsInfoModel.Setting<int>{SettingName = nameof(SettingsInfoModel.AllVolume), InfoLabelText = "Recommended", SettingValue = 100};
            SettingsInfoModel.VfxVolume = new SettingsInfoModel.Setting<int>{SettingName = nameof(SettingsInfoModel.VfxVolume), InfoLabelText = "Recommended", SettingValue = 100};
            SettingsInfoModel.MusicVolume = new SettingsInfoModel.Setting<int>{SettingName = nameof(SettingsInfoModel.MusicVolume), InfoLabelText = "Recommended", SettingValue = 100};
            // In Game
            SettingsInfoModel.IsTextSlowed = new SettingsInfoModel.Setting<bool>{SettingName = nameof(SettingsInfoModel.IsTextSlowed), InfoLabelText = "Recommended", SettingValue = false};
            SettingsInfoModel.LightFlicker =new SettingsInfoModel.Setting<bool>{SettingName = nameof(SettingsInfoModel.LightFlicker), InfoLabelText = "Recommended", SettingValue = false};
        }
        
        #region PropertyLists

        private static void FillLists()
        {
            // Video
            SettingsInfoModel.videoList = new SettingsInfoModel.SettingList<int>
            {
                Settings = new List<SettingsInfoModel.Setting<int>>
                {
                    SettingsInfoModel.FpsValue,
                }
            };
            SettingsInfoModel.videoList2 = new SettingsInfoModel.SettingList<string>
            {
                Settings = new List<SettingsInfoModel.Setting<string>>
                {
                    SettingsInfoModel.WindowSize,
                }
            };
            // Audio
            SettingsInfoModel.audioList = new SettingsInfoModel.SettingList<int>
            {
                Settings = new List<SettingsInfoModel.Setting<int>>
                {
                    SettingsInfoModel.AllVolume,
                    SettingsInfoModel.VfxVolume,
                    SettingsInfoModel.MusicVolume
                }
            };
            // In Game
            SettingsInfoModel.inGameList = new SettingsInfoModel.SettingList<bool>
            {
                Settings = new List<SettingsInfoModel.Setting<bool>>
                {
                    SettingsInfoModel.IsTextSlowed,
                    SettingsInfoModel.LightFlicker,
                }
            };
        }
        
        #endregion
    }
}