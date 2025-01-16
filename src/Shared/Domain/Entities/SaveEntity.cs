using Newtonsoft.Json;

namespace SaveEntity
{
    public static class SaveSystem
    {
        public class SaveData
        {
            public int Score { get; set; } = 0;
            public int Lives { get; set; } = 0;
        }

        public static string GetSavePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"MS", "save.json");
        }

        public static void Save(int score, int lives)
        {
            SaveData data = new SaveData();
            data.Score = score;
            data.Lives = lives;

            string json = JsonConvert.SerializeObject(data);
            
            string directory = Path.GetDirectoryName(GetSavePath());
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(GetSavePath(), json);
        }

        public static SaveData Load()
        {
            if (!File.Exists(GetSavePath()))
            {
                return new SaveData();
            }

            string json = File.ReadAllText(GetSavePath());
            return JsonConvert.DeserializeObject<SaveData>(json);
        }
    }
}
