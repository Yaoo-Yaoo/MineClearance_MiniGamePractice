using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MineSweeper
{
    public class SlotActualButtonController : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
    {
#if UNITY_STANDALONE
        public Action onLeftRightClicked;
        private bool isLeftClicked = false;
        private bool isRightClicked = false;
#endif

        public void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                isLeftClicked = true;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                isRightClicked = true;
            }

            if (isLeftClicked && isRightClicked)
            {
                onLeftRightClicked?.Invoke();
            }
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            isLeftClicked = false;
            isRightClicked = false;
#endif
        }
    }
}
