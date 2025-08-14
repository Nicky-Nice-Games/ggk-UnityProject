using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class InputFieldVisual : MonoBehaviour
{
    private UnityEngine.UI.InputField inputField;

    public RectTransform animTarget;
    public float animDuration = 0.3f;
    public float animScale = 20f;
    public Vector2 anchoredPos;

    public void OnSelect(BaseEventData eventData)
    {
        DoWave();
    }

    void Start()
    {
        anchoredPos = animTarget.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoWave()
    {
        //kill tweens and reset position if it's not in the right place for a tween
        if (anchoredPos != new Vector2(0f, 0f) && animTarget.anchoredPosition != anchoredPos)
        {
            animTarget.DOKill();
            animTarget.anchoredPosition = anchoredPos;
        }

        animTarget.DOScaleY(1.5f, animDuration * 0.5f).SetEase(Ease.OutQuad)
            .OnComplete(() => animTarget.DOScaleY(1f, animDuration * 0.5f).SetEase(Ease.OutBack));

        animTarget.DOAnchorPosY(animScale, animDuration * 0.5f).SetRelative().SetEase(Ease.OutSine)
            .OnComplete(() => animTarget.DOAnchorPosY(-animScale, animDuration * 0.5f).SetRelative().SetEase(Ease.InOutSine));
    }
}
