using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MineSweeper
{
        public class SlotButtonController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
        {
                public Action onLeftClicked;
                public Action onRightClicked;

#if UNITY_ANDROID
                public Action onAndroidLeftHold;
                private bool isPointerDown = false;
                private bool isHoldTriggered = false;
                private float timeHoldTimer = 0f;
                private float holdTimeThreshold = 0.3f;
#endif

                public void OnPointerClick(PointerEventData eventData)
                {
#if UNITY_ANDROID
                        if (!isHoldTriggered)
                                onLeftClicked?.Invoke();
#elif UNITY_STANDALONE
                        if (eventData.button == PointerEventData.InputButton.Left)
                                onLeftClicked?.Invoke();
                        else if (eventData.button == PointerEventData.InputButton.Right)
                                onRightClicked?.Invoke();
#endif
                }

                //安卓平台长按判断
                public void OnPointerDown(PointerEventData eventData)
                {
#if UNITY_ANDROID
                        isPointerDown = true;
                        isHoldTriggered = false;
                        timeHoldTimer = Time.time;
#endif
                }

                public void OnPointerUp(PointerEventData eventData)
                {
#if UNITY_ANDROID
                        isPointerDown = false;
#endif
                }

                public void OnPointerExit(PointerEventData eventData)
                {
#if UNITY_ANDROID
                        isPointerDown = false;
#endif
                }

#if UNITY_ANDROID
                private void Update()
                {
                        if (isPointerDown && !isHoldTriggered)
                        {
                                if (Time.time - timeHoldTimer > holdTimeThreshold)
                                {
                                        isHoldTriggered = true;
                                        onRightClicked?.Invoke();
                                }
                        }
                }
#endif
        }
}
