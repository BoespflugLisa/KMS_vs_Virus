using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConfig
{

    // HTC, force the device to HTC, PC, force device to PC Keyboard, AUTO: select the device according to the hardware connected.
    public string DeviceUsed = "AUTO";

    public int LifeNumber;
    public int NbContaminatedPlayerToVictory;

    public float DelayShoot;
    public float DelayTeleport;
    public float RadiusExplosion;
    public float TimeToAreaContamination;

    public Color ColorShotKMS;
    public Color ColorShotVirus;


    private static AppConfig inst = null;

    public static AppConfig Inst
    {
        get
        {
            if (inst == null) inst = new AppConfig();
            return inst;
        }
    }

    public void UpdateValuesFromJsonString(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, Inst);
    }

    public void UpdateValuesFromJsonFile(string filePath)
    {
        UpdateValuesFromJsonString(System.IO.File.ReadAllText(filePath));
    }

    public void UpdateValuesFromJsonFile()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, Application.productName + ".AppConfig.json");
        UpdateValuesFromJsonFile(path);
    }

    public string ToJsonString()
    {
        return JsonUtility.ToJson(Inst, true);
    }

    /// <summary>
    /// Read from the json file called StreamingAssets/{Application.productName+".AppConfig.json"}
    /// </summary>
    /*public void UpdateValuesFromJsonFile()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, Application.productName + ".AppConfig.json");
        Debug.Log("File used: " + path);
        UpdateValuesFromJsonString(System.IO.File.ReadAllText(path));

    }


    /// <summary>
    /// Update the values from a Json String
    /// </summary>
    /// <param name="jsonString"></param>
    public void UpdateValuesFromJsonString(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, Inst);
        Debug.Log(ToJsonString());

    }


    public string ToJsonString()
    {
        return JsonUtility.ToJson(Inst, true);
    }*/

}
