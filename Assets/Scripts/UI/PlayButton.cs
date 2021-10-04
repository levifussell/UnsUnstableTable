using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    #region variables
    RectTransform rectTransform;
    Text text;

    Vector2 initRect;
    Color initColor;
    #endregion

    #region builtins
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        text = this.GetComponentInChildren<Text>();

        initRect = rectTransform.sizeDelta;
        initColor = text.color;
    }
    #endregion

    #region controls
    void FadeEnter()
    {
        StartCoroutine(FadeTo(
            new Color(224.0f / 255.0f, 47.0f / 255.0f, 47.0f / 255.0f),
            new Vector2(-100.0f, 100.0f),
            //new Vector2(300.0f, 100.0f),
            0.2f
            ));
    }
    void FadeExit()
    {
        StartCoroutine(FadeTo(
            Color.white,
            new Vector2(-300.0f, 100.0f),
            0.2f
            ));
    }

    void FadeTransition()
    {
        StartCoroutine(FadeTo(
            new Color(224.0f / 255.0f, 47.0f / 255.0f, 47.0f / 255.0f),
            new Vector2(300.0f, 100.0f),
            0.2f
            ));
    }

    IEnumerator FadeTo(Color toColor, Vector2 toSize, float secondsToFade)
    {
        float minR = Mathf.Min(text.color.r, toColor.r);
        float minG = Mathf.Min(text.color.g, toColor.g);
        float minB = Mathf.Min(text.color.b, toColor.b);
        float maxR = Mathf.Max(text.color.r, toColor.r);
        float maxG = Mathf.Max(text.color.g, toColor.g);
        float maxB = Mathf.Max(text.color.b, toColor.b);
        Vector2 minRec = Vector2.Min(rectTransform.sizeDelta, toSize);
        Vector2 maxRec = Vector2.Max(rectTransform.sizeDelta, toSize);

        Color colVel = (toColor - text.color) / secondsToFade;
        Vector2 sizeVel = (toSize - rectTransform.sizeDelta) / secondsToFade;

        float timeElapsed = 0.0f;
        while (timeElapsed <= secondsToFade)
        {
            rectTransform.sizeDelta += sizeVel * Time.deltaTime;

            text.color += colVel * Time.deltaTime;

            text.color = new Color(
                Mathf.Clamp(text.color.r, minR, maxR),
                Mathf.Clamp(text.color.r, minB, maxB),
                Mathf.Clamp(text.color.r, minG, maxG)
                );
            rectTransform.sizeDelta = new Vector2(
                Mathf.Clamp(rectTransform.sizeDelta.x, minRec.x, maxRec.x),
                Mathf.Clamp(rectTransform.sizeDelta.y, minRec.y, maxRec.y));

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rectTransform.sizeDelta = toSize;
        text.color = toColor;
    }

    public void Reset()
    {
        rectTransform.sizeDelta = initRect;
        text.color = initColor;
    }

    #endregion

    #region ui events
    public void OnPointerEnter(PointerEventData eventData)
    {
        FadeEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FadeExit();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FadeTransition();
        //this.transform.parent.gameObject.SetActive(false);
        //rectTransform.sizeDelta = new Vector2(-300.0f, 100.0f);
        //text.color = Color.white;
    }
    #endregion
}
