using OutwardGameSettings.Serializable;
using OutwardGameSettings.Utility.Enums;
using OutwardGameSettings.Utility.Timer.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OutwardGameSettings.Managers
{
    public class DataSerializer
    {
        private static DataSerializer _instance;

        private DataSerializer()
        {
            this.configPath = Path.Combine(OutwardModsCommunicator.Managers.PathsManager.ConfigPath, "Game_Settings");
            this.savesPath = Path.Combine(this.configPath, "Saves");
        }

        public static DataSerializer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataSerializer();

                return _instance;
            }
        }

        public string configPath = "";
        public string savesPath = "";

        public bool GetCharacterEventsSavePath(out string savePath)
        {
            if (Global.Lobby == null)
            {
                savePath = Path.Combine(this.savesPath, "Error");
                return false;
            }

            Character character = CharacterManager.Instance.GetFirstLocalCharacter();

            if (character?.UID == null)
            {
                savePath = Path.Combine(this.savesPath, "Error");
                return false;
            }

            savePath = Path.Combine(this.savesPath, character.UID.Value, "Events.xml");
            return true;
        }

        public static void SaveToFile(string path, EventSaveData data)
        {
            var serializer = new XmlSerializer(typeof(EventSaveData));
            using (var writer = new StreamWriter(path))
                serializer.Serialize(writer, data);
        }

        public static EventSaveData LoadFromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var serializer = new XmlSerializer(typeof(EventSaveData));
            using (var reader = new StreamReader(path))
                return serializer.Deserialize(reader) as EventSaveData;
        }

        public void SaveEventsToXml(string filePath, EventSaveData events)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var serializer = new XmlSerializer(typeof(EventSaveData));

                var xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = false
                };

                using (var writer = XmlWriter.Create(filePath, xmlWriterSettings))
                {
                    serializer.Serialize(writer, events);
                }
            }
            catch (Exception ex)
            {
                OutwardGameSettings.LogMessage($"DataSerializer@SaveEventsToXml failed saving '{filePath}': {ex.Message}");
            }
        }

        public void SaveEventsToXml(string filePath, int lastDay, int lastHour, List<InGameEventController> events)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var file = BuildEventsFile(lastDay, lastHour, events);

                var serializer = new XmlSerializer(typeof(EventSaveData));

                var xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = false
                };

                using (var writer = XmlWriter.Create(filePath, xmlWriterSettings))
                {
                    serializer.Serialize(writer, file);
                }
            }
            catch (Exception ex)
            {
                OutwardGameSettings.LogMessage($"DataSerializer@SaveEventsToXml failed saving '{filePath}': {ex.Message}");
            }
        }

        public bool SaveCurrentCharacterEvents()
        {
            if (!DataSerializer.Instance.GetCharacterEventsSavePath(out string savePath))
                return false;

            if (string.IsNullOrEmpty(savePath))
                return false;

            EventSaveData saveData = EnemyHourlyEventsHelper.CreateCurrentCharacterSaveData();

            if (saveData == null)
                return false;

            SaveEventsToXml(savePath, saveData);
            return true;
        }

        public EventSaveData LoadCurrentCharacterEvents()
        {
            if (!DataSerializer.Instance.GetCharacterEventsSavePath(out string loadPath))
                return new EventSaveData();

            if (string.IsNullOrEmpty(loadPath))
                return new EventSaveData();

            if (!File.Exists(loadPath))
                return new EventSaveData();

            return Load(loadPath);
        }

        public EventSaveData Load(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    OutwardGameSettings.LogMessage($"EventSaveData file not found at: {path}");
                    return null;
                }

                XmlSerializer serializer = new(typeof(EventSaveData));

                using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
                return serializer.Deserialize(fs) as EventSaveData;
            }
            catch (Exception ex)
            {
                OutwardGameSettings.LogMessage($"Failed to load EventSaveData file at '{path}': {ex.Message}");
                return null;
            }
        }


        public EventSaveData BuildEventsFile(int lastDay, int lastHour, List<InGameEventController> controllers)
        {
            var file = new EventSaveData
            {
                LastDay = lastDay,
                LastHour = lastHour,
                Accumulators = new List<float>()
            };

            foreach (var controller in controllers)
            {
                file.Accumulators.Add(controller.Accumulator);
            }

            return file;
        }
    }
}
