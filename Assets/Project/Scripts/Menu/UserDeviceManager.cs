using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace WS3
{
    public enum UserDeviceType
    {
        HTC,
        PC
    }
    public class UserDeviceManager : MonoBehaviour
    {

        private void Awake()
        {
            if (!XRSettings.isDeviceActive)
            {
                Debug.Log("No Headset plugges");

                SceneManager.LoadScene("Lobby");
            }
            else if (XRSettings.isDeviceActive && (XRSettings.loadedDeviceName == " Mock HMD" || XRSettings.loadedDeviceName == "MockHMDDisplay"))
            {
                Debug.Log("Using Mock HMD");
                //SceneManager.LoadScene("Lobby_VR");


            }
            else
            {
                Debug.Log("We have a headset " + XRSettings.loadedDeviceName);
                //SceneManager.LoadScene("Lobby_VR");
            }
        }

        public static UserDeviceType GetDeviceUsed()
        {
            // Server execution

            string deviceUsed = AppConfig.Inst.DeviceUsed.ToLower();

            switch (deviceUsed)
            {
                case "htc":
                    // Si l'app config demande du HTC mais que le casque n'est pas branché
                    Debug.LogWarning("AppConfig asked for HTC, but not active, so use PC version");
                    return UnityEngine.XR.XRSettings.isDeviceActive ? UserDeviceType.HTC : UserDeviceType.PC;

                case "pc":
                    return UserDeviceType.PC;

                default: // "auto" and others
                    return UnityEngine.XR.XRSettings.isDeviceActive ? UserDeviceType.HTC : UserDeviceType.PC;
            }
        }

        public static GameObject GetPrefabToSpawnWithDeviceUsed(GameObject pcPrefab, GameObject HTCPrefab)
        {
            UserDeviceType userDeviceType = GetDeviceUsed();
            switch (userDeviceType)
            {
                case UserDeviceType.HTC:
                    return HTCPrefab;
                case UserDeviceType.PC:
                default:
                    return pcPrefab;
            }
        }
    }
}