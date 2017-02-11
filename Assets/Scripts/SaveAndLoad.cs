using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveAndLoad {

    static string savePath = Application.persistentDataPath + "/save.dat";

    public static void ClearData () {
        WriteOnFile(new GameData());
        Debug.Log("Data Cleared");
    }

    public static void Save () {
        WriteOnFile(GatherData());
    }

    public static void Load () {
        if (File.Exists(savePath)) {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            GameData gd = (GameData) bf.Deserialize(file);
            UpdateGameWithNewData(gd);
            file.Close();
        }
    }

    static void WriteOnFile(GameData gd){
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, gd);
        file.Close();
    }

    static GameData GatherData () {
        GameData gd = new GameData();
        gd.barrelsBeatRecord = DifficultyManager.barrelsBeatRecord;

        return gd;
    }

    static void UpdateGameWithNewData (GameData gd) {
        DifficultyManager.barrelsBeatRecord = gd.barrelsBeatRecord;
    }

    [System.Serializable]
    class GameData
    {
        public int barrelsBeatRecord = 1;
    }
}
