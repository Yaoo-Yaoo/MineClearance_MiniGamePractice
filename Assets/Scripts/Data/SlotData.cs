using System;
using UnityEngine;
using UnityEngine.UI;

namespace MineSweeper
{
    public class SlotData
    {
        public GameObject slotObj;
        private SlotSpritesSetting slotSpritesSetting;

        private E_Slot_ActualState m_slotActualState;
        public E_Slot_ActualState slotActualState
        {
            get => m_slotActualState;
            set
            {
                m_slotActualState = value;
                if (value == E_Slot_ActualState.IsMine)
                    slotObj.transform.GetChild(0).GetComponent<Image>().sprite = slotSpritesSetting.mineSprite;
                else
                    slotObj.transform.GetChild(0).GetComponent<Image>().sprite = slotSpritesSetting.emptySprite;
            }
        }

        private int m_mineCountAround;
        public int mineCountAround
        {
            get => m_mineCountAround;
            set
            {
                m_mineCountAround = value;
                if (value == 0 || value == -1) return;
                slotObj.GetComponentInChildren<Text>().color = slotSpritesSetting.numberSprites[value - 1];
            }
        }

        public bool hasShown;

        private E_Slot_SignState m_slotSignedState;
        public E_Slot_SignState slotSignedState
        {
            get => m_slotSignedState;
            set
            {
                m_slotSignedState = value;
                switch (value)
                {
                    case E_Slot_SignState.NotSigned:
                        slotObj.transform.GetChild(4).GetComponent<Image>().sprite = slotSpritesSetting.unclickedSprite;
                        break;
                    case E_Slot_SignState.SignedMine:
                        slotObj.transform.GetChild(4).GetComponent<Image>().sprite = slotSpritesSetting.signedMineSprite;
                        break;
                    case E_Slot_SignState.SignedNotSure:
                        slotObj.transform.GetChild(4).GetComponent<Image>().sprite = slotSpritesSetting.signedNotSureSprite;
                        break;
                }
            }
        }

        private bool m_isSignedWrong;
        public bool isSignedWrong
        {
            get => m_isSignedWrong;
            set
            {
                m_isSignedWrong = value;
                slotObj.transform.Find("Wrong").gameObject.SetActive(value);
            }
        }

        public SlotData(GameObject slot, SlotSpritesSetting slotSpritesSetting)
        {
            slotObj = slot;
            this.slotSpritesSetting = slotSpritesSetting;
            //默认全部都不是雷
            slotActualState = E_Slot_ActualState.HasNoMineAround;
            mineCountAround = 0;
            hasShown = false;
            slotSignedState = E_Slot_SignState.NotSigned;
            isSignedWrong = false;
            //注册按钮点击事件
            SlotButtonController slotButtonController = slotObj.GetComponentInChildren<SlotButtonController>();
            slotButtonController.onLeftClicked += OnLeftClicked;
            slotButtonController.onRightClicked += OnRightClicked;
            SlotActualButtonController slotActualButtonController = slotObj.GetComponentInChildren<SlotActualButtonController>();
#if UNITY_STANDALONE
            slotActualButtonController.onLeftRightClicked += OnLeftRightClicked;
#endif
        }

        private void OnLeftClicked()
        {
            if (slotSignedState != E_Slot_SignState.NotSigned) return;
            if (hasShown) return;

            ShowActual();
            EventManager.Instance.onPlayClickAudio.Trigger();
            switch (slotActualState)
            {
                case E_Slot_ActualState.IsMine:
                    OnWrong();
                    EventManager.Instance.onGameOver.Trigger();
                    break;
                case E_Slot_ActualState.HasMineAround:
                    GameData.Instance.needClickSlotCount--;
                    break;
                case E_Slot_ActualState.HasNoMineAround:
                    string[] slotPos = slotObj.name.Split(',');
                    EventManager.Instance.onFindBoundary.Trigger(System.Int32.Parse(slotPos[0]), System.Int32.Parse(slotPos[1]));
                    GameData.Instance.needClickSlotCount--;
                    break;
            }
        }

        public void OnWrong()
        {
            slotObj.transform.GetChild(0).GetComponent<Image>().sprite = slotSpritesSetting.mineBoom;
        }

        private void OnRightClicked()
        {
            if (hasShown) return;

            EventManager.Instance.onPlayClickAudio.Trigger();
            switch (slotSignedState)
            {
                case E_Slot_SignState.NotSigned:
                    slotSignedState = E_Slot_SignState.SignedMine;
                    if (slotActualState != E_Slot_ActualState.IsMine) isSignedWrong = true;
                    SignMine(true);
                    GameData.Instance.remainSignedMineCount--;
                    break;
                case E_Slot_SignState.SignedMine:
                    slotSignedState = E_Slot_SignState.SignedNotSure;
                    if (isSignedWrong) isSignedWrong = false;
                    SignMine(false);
                    GameData.Instance.remainSignedMineCount++;
                    break;
                case E_Slot_SignState.SignedNotSure:
                    slotSignedState = E_Slot_SignState.NotSigned;
                    if (isSignedWrong) isSignedWrong = false;
                    SignMine(false);
                    break;
            }
        }

        private void SignMine(bool isSignedMine)
        {
            if (slotActualState == E_Slot_ActualState.IsMine)
            {
                if (isSignedMine)
                    slotObj.transform.GetChild(0).GetComponent<Image>().sprite = slotSpritesSetting.signedRightSprite;
                else
                    slotObj.transform.GetChild(0).GetComponent<Image>().sprite = slotSpritesSetting.mineSprite;
            }
        }

        private void OnLeftRightClicked()
        {
            if (slotActualState == E_Slot_ActualState.HasMineAround)
            {
                string[] slotPos = slotObj.name.Split(',');
                EventManager.Instance.onLeftRightClicked.Trigger(System.Int32.Parse(slotPos[0]), System.Int32.Parse(slotPos[1]));
            }
        }

        public void ShowActual()
        {
            SlotButtonController slotButtonController = slotObj.GetComponentInChildren<SlotButtonController>();
            slotButtonController.onLeftClicked -= OnLeftClicked;
            slotButtonController.onRightClicked -= OnRightClicked;
            slotObj.transform.GetChild(4).gameObject.SetActive(false);
            hasShown = true;
        }
    }
}
