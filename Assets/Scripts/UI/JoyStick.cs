﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 안쓰는거(옛날 조이스틱 스크립트)
public class JoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;    // 조이스틱 배경
    private Image joystickImg;  // 조이스틱
    private Vector3 inputVector;    // 이동 벡터값

    void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    // 배경이미지가 터치받으면 조이스틱이 터치받은 곳으로 이동
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2 - 1, pos.y * 2 - 1, 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // 조이스틱 이동
            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x *
                (bgImg.rectTransform.sizeDelta.x / 3), inputVector.y * (bgImg.rectTransform.sizeDelta.y / 3));
        }
    }
    
    // 터치하고 있을 때
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    // 터치 안할 때 원위치
    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    // 플레이어 컨트롤 스크립트에서 x값을 받기 위함
    public float GetHorizontalValue()
    {
        return inputVector.x;
    }

    // 플레이어 컨트롤 스크립트에서 y값을 받기 위함
    public float GetVerticalValue()
    {
        return inputVector.y;
    }

}
