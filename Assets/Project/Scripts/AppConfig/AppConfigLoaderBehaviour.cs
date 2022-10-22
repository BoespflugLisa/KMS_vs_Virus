using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConfigLoaderBehaviour : MonoBehaviour {

    [SerializeField] private string m_SceneToLoadAfterAppConfig;
    [SerializeField] private string m_AppConfigFilePath;

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(m_AppConfigFilePath))
            AppConfig.Inst.UpdateValuesFromJsonFile();
        else
            AppConfig.Inst.UpdateValuesFromJsonFile(m_AppConfigFilePath);

        if (!string.IsNullOrEmpty(m_SceneToLoadAfterAppConfig))
            UnityEngine.SceneManagement.SceneManager.LoadScene(m_SceneToLoadAfterAppConfig);
    }

}
