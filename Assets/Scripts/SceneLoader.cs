using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    string DefaultSceneName = "UnityGears";
    
    // Start is called before the first frame update
    
    void Start()
    {
        string SceneName;

        if (URLParameters.GetSearchParameters().TryGetValue("scene", out SceneName) == false)
            SceneName = DefaultSceneName;

        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }
}
