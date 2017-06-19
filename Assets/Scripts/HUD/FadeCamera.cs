using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCamera : MonoBehaviour {

    public Image fadeImage;
    public float fadeSpeed;
    private float targetAlpha = 0;
    [HideInInspector] public bool doneFading = false;
    
    // Use this for initialization
    void Start () {
        fadeImage = GameObject.Find("FadeScreen").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update () {
        Color currentColor = fadeImage.color;
        if (Mathf.Abs(currentColor.a - targetAlpha) > 0.0001f)
        {
            currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            fadeImage.color = currentColor;
        }
	}
    
    public void FadeToClear()
    {
        targetAlpha = 0.0f;
        fadeImage.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void FadeToBlack()
    {
        targetAlpha = 1.0f;
        fadeImage.color = Color.black;
    }
}
