using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    private float Transparence;
    public bool FadeIn;
    public float Step = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        Transparence = 0.0f;
        FadeIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        Transparence = Mathf.Clamp(Transparence,0,1);
        if (FadeIn)
        {
            Transparence += Step;

        } else
        {
            Transparence -= Step;
        }

        GetComponent<CanvasGroup>().alpha = Transparence;
    }

    public void CoroutineFade(FadeScript respawn)
    {
        StartCoroutine(FadeInFadeOut(respawn));
    }

    public IEnumerator FadeInFadeOut(FadeScript instance)
    {
        instance.FadeIn = true;
        yield return new WaitForSeconds(2.0f);
        instance.FadeIn = false;
    }
}
