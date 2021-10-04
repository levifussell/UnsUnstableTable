using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TransitionMenu : MonoBehaviour
{
    [SerializeField]
    Image flagTop = null;
    [SerializeField]
    Image flagMiddle = null;
    [SerializeField]
    Image flagBottom = null;

    public Action onTransitionEnd;

    RectTransform topRect;
    RectTransform middleRect;
    RectTransform bottomRect;

    float MIDDLE_OFFSET = Screen.width * 10.0f;
    float BOTTOM_OFFSET = Screen.width * 10.0f;

    Vector2 TOP_OUT_TARGET;
    Vector2 MIDDLE_OUT_TARGET;
    Vector2 BOTTOM_OUT_TARGET;

    private void Awake()
    {
        topRect = flagTop.GetComponent<RectTransform>();
        middleRect = flagMiddle.GetComponent<RectTransform>();
        bottomRect = flagBottom.GetComponent<RectTransform>();

        TOP_OUT_TARGET = new Vector2(Screen.width*1.5f + MIDDLE_OFFSET, 0.0f);
        MIDDLE_OUT_TARGET = new Vector2(Screen.width*1.5f + MIDDLE_OFFSET, 0.0f);
        BOTTOM_OUT_TARGET = new Vector2(Screen.width*1.5f + BOTTOM_OFFSET, 0.0f);

        TransitionReset();
        //StartCoroutine(TransitionStage1());
    }

    // Update is called once per frame
    void Update()
    {
        //Shift(flagTop);
        //Shift(flagMiddle);
        //Shift(flagBottom);
    }

    void TransitionReset()
    {
        topRect.anchoredPosition = new Vector2(-Screen.width - MIDDLE_OFFSET * 2.0f, 0.0f);
        middleRect.anchoredPosition = new Vector2(-Screen.width - MIDDLE_OFFSET, 0.0f);
        bottomRect.anchoredPosition = new Vector2(-Screen.width - BOTTOM_OFFSET, 0.0f);
    }

    public void Transition()
    {
        TransitionReset();
        StartCoroutine(TransitionStage1());
    }

    IEnumerator TransitionStage1()
    {
        yield return new WaitForSeconds(1.0f);
        float distTopRect = Mathf.Abs(0.0f - bottomRect.anchoredPosition.x);
        float moveRate = 0.09f;
        while (distTopRect > Screen.width * 0.01f)
        {
            topRect.anchoredPosition += (Vector2.zero - topRect.anchoredPosition) * moveRate;
            middleRect.anchoredPosition += (Vector2.zero - middleRect.anchoredPosition) * moveRate;
            bottomRect.anchoredPosition += (Vector2.zero - bottomRect.anchoredPosition) * moveRate;
            distTopRect = Mathf.Abs(0.0f - topRect.anchoredPosition.x);
            yield return new WaitForEndOfFrame();
        }

        // do transition. 

        onTransitionEnd?.Invoke();

        // clear the delegates.

        foreach(Delegate d in onTransitionEnd.GetInvocationList())
        {
            onTransitionEnd -= (System.Action)d;
        }
        onTransitionEnd = null;

        yield return TransitionStage2();
    }

    IEnumerator TransitionStage2()
    {
        float distTopRect = Mathf.Abs(TOP_OUT_TARGET.x - topRect.anchoredPosition.x);
        float moveRate = 0.05f;
        while (distTopRect > Screen.width * 0.01f)
        {
            topRect.anchoredPosition += (TOP_OUT_TARGET - topRect.anchoredPosition) * moveRate;
            middleRect.anchoredPosition += (MIDDLE_OUT_TARGET - middleRect.anchoredPosition) * moveRate;
            bottomRect.anchoredPosition += (BOTTOM_OUT_TARGET - bottomRect.anchoredPosition) * moveRate;
            distTopRect = Mathf.Abs(TOP_OUT_TARGET.x - topRect.anchoredPosition.x);
            yield return new WaitForEndOfFrame();
        }
    }

    //void Shift(RectTransform)
    //{
    //    RectTransform rect = im.GetComponent<RectTransform>();
    //    //rect.anchoredPosition += new Vector2(5.0f, 0.0f);
    //    rect.anchoredPosition = new Vector2(-Screen.width, 0.0f);
    //}
}
