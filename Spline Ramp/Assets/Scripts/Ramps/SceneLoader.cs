using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI loading_text;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisplayLoadingText());
        LoadScene();
    }
    
    public void LoadScene()
    {
        IEnumerator Loading()
        {
            AsyncOperation loading_Operation = SceneManager.LoadSceneAsync(1);

            while(true)
            {
                if (loading_Operation.isDone)
                    break;
                yield return new WaitForSeconds(0.2f);
            }

            StartCoroutine(FadeLoadingText());
            yield return null;
        }

        StartCoroutine(Loading());
    }

    IEnumerator DisplayLoadingText()
    {
        for (int i = 0; i <= 10; i++)
        {
            byte alpha = Convert.ToByte((int)(255 * (i / 10f)));
            loading_text.faceColor = new Color32(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeLoadingText()
    {
        for (int i = 10; i>=0; i--)
        {
            byte alpha = Convert.ToByte((int)(255 * (i / 10f)));
            loading_text.faceColor = new Color32(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
