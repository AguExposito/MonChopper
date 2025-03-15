using System.IO;
using UnityEngine;

public class JsonManager
{
    public static int fileId;
    public static void SaveToFile(string fileName, object data)
    {
        if (!PlayerPrefs.HasKey("fileId"))
        {
            PlayerPrefs.SetInt("fileId", fileId);
        }
        fileId=PlayerPrefs.GetInt("fileId", 0);
        PlayerPrefs.SetInt("fileId", fileId+1);
        PlayerPrefs.Save();
        fileName = fileName+fileId;
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Archivo guardado en: " + path);
    }

    public static T LoadFromFile<T>(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        Debug.LogError("No se encontró el archivo: " + path);
        return default;
    }
    public static T ScriptableObjectLoadFromFile<T>(string fileName) where T : ScriptableObject
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            T instance = ScriptableObject.CreateInstance<T>(); // Crear la instancia antes
            JsonUtility.FromJsonOverwrite(json, instance); // Sobreescribir con los datos del JSON
            return instance;
        }
        Debug.LogError("No se encontró el archivo: " + path);
        return null;
    }
}