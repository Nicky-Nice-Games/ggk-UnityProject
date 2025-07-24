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

    public void OnSelect(BaseEventData eventData)
    {
        DoWave();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoWave()
    {
        animTarget.DOScaleY(1.5f, animDuration * 0.5f).SetEase(Ease.OutQuad)
            .OnComplete(() => animTarget.DOScaleY(1f, animDuration * 0.5f).SetEase(Ease.OutBack));

        animTarget.DOAnchorPosY(animScale, animDuration * 0.5f).SetRelative().SetEase(Ease.OutSine)
            .OnComplete(() => animTarget.DOAnchorPosY(-animScale, animDuration * 0.5f).SetRelative().SetEase(Ease.InOutSine));
    }
}
