using System;
using UnityEngine;
using UnityEngine.UI;

namespace MineSweeper
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Toggle easyLevel;
        [SerializeField] private Toggle normalLevel;
        [SerializeField] private Toggle hardLevel;
        [SerializeField] private Toggle customLevel;
        [SerializeField] private Text remainMineCountText;
        private const string remainText = "Remain: ";
        [SerializeField] private GameObject winText;
        [SerializeField] private GameObject frontPanel;
        [SerializeField] private GameObject customPanel;
        [SerializeField] private InputField columnInputField;
        [SerializeField] private InputField rowInputField;
        [SerializeField] private InputField mineInputField;
        [SerializeField] private Button confirmButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(OnRestart);
            exitButton.onClick.AddListener(OnExit);
            easyLevel.onValueChanged.AddListener(OnChooseEasyLevel);
            normalLevel.onValueChanged.AddListener(OnChooseNormalLevel);
            hardLevel.onValueChanged.AddListener(OnChooseHardLevel);
            customLevel.onValueChanged.AddListener(OnChooseCustomLevel);
            EventManager.Instance.onRemainMineChanged.Register(OnChangeRemainMineCount);
            EventManager.Instance.onGameWin.Register(OnGameWin);
            columnInputField.onValueChanged.AddListener(OnColumnCountChanged);
            rowInputField.onValueChanged.AddListener(OnRowCountChanged);
            columnInputField.onValueChanged.AddListener(OnColumnCountChanged);
            mineInputField.onValueChanged.AddListener(OnMineCountChanged);
            confirmButton.onClick.AddListener(OnConfirmCustom);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveListener(OnRestart);
            exitButton.onClick.RemoveListener(OnExit);
            easyLevel.onValueChanged.RemoveListener(OnChooseEasyLevel);
            normalLevel.onValueChanged.RemoveListener(OnChooseNormalLevel);
            hardLevel.onValueChanged.RemoveListener(OnChooseHardLevel);
            customLevel.onValueChanged.RemoveListener(OnChooseCustomLevel);
            EventManager.Instance.onRemainMineChanged.UnRegister(OnChangeRemainMineCount);
            EventManager.Instance.onGameWin.UnRegister(OnGameWin);
            rowInputField.onValueChanged.RemoveListener(OnRowCountChanged);
            columnInputField.onValueChanged.RemoveListener(OnColumnCountChanged);
            mineInputField.onValueChanged.RemoveListener(OnMineCountChanged);
            confirmButton.onClick.RemoveListener(OnConfirmCustom);
        }

        private void OnRestart()
        {
            winText.SetActive(false);
            frontPanel.SetActive(false);
            EventManager.Instance.onRestartGame.Trigger();
        }

        private void OnExit()
        {
            Application.Quit();
        }

        private void OnChooseEasyLevel(bool isOn)
        {
            winText.SetActive(false);
            frontPanel.SetActive(false);
            if (isOn)
                GameData.Instance.gameLevel = E_Slot_Level.Easy;
        }

        private void OnChooseNormalLevel(bool isOn)
        {
            winText.SetActive(false);
            frontPanel.SetActive(false);
            if (isOn)
                GameData.Instance.gameLevel = E_Slot_Level.Normal;
        }

        private void OnChooseHardLevel(bool isOn)
        {
            winText.SetActive(false);
            frontPanel.SetActive(false);
            if (isOn)
                GameData.Instance.gameLevel = E_Slot_Level.Hard;
        }

        private void OnChooseCustomLevel(bool isOn)
        {
            winText.SetActive(false);
            frontPanel.SetActive(false);
            customPanel.SetActive(isOn);
            if (isOn)
            {
                GameData.Instance.gameLevel = E_Slot_Level.Custom;
                mineInputField.SetTextWithoutNotify(GameData.Instance.mineCount.ToString());
                rowInputField.SetTextWithoutNotify(GameData.Instance.rowCount.ToString());
                columnInputField.SetTextWithoutNotify(GameData.Instance.columnCount.ToString());
            }
        }

        private void OnChangeRemainMineCount(int num)
        {
            winText.SetActive(false);
            frontPanel.SetActive(false);
            remainMineCountText.text = remainText + num.ToString();
        }

        private void OnGameWin()
        {
            winText.SetActive(true);
            frontPanel.SetActive(true);
        }

        private void OnMineCountChanged(string value)
        {
            int newValue = Mathf.Clamp(int.Parse(value), 1, Mathf.Min(GameData.Instance.rowCount * GameData.Instance.columnCount + 1, 150));
            if (newValue != int.Parse(value))
                mineInputField.SetTextWithoutNotify(newValue.ToString());
        }

        private void OnRowCountChanged(string value)
        {
            int newValue = Mathf.Clamp(int.Parse(value), 1, 16);
            if (newValue != int.Parse(value))
                rowInputField.SetTextWithoutNotify(newValue.ToString());

            CheckMineCount();
        }

        private void OnColumnCountChanged(string value)
        {
            int newValue = Mathf.Clamp(int.Parse(value), 1, 30);
            if (newValue != int.Parse(value))
                columnInputField.SetTextWithoutNotify(newValue.ToString());

            CheckMineCount();
        }

        private void CheckMineCount()
        {
            int max = int.Parse(columnInputField.text) * int.Parse(rowInputField.text);
            if (int.Parse(mineInputField.text) >= max)
                mineInputField.SetTextWithoutNotify((max - 1).ToString());
        }

        private void OnConfirmCustom()
        {
            GameData.Instance.columnCount = int.Parse(columnInputField.text);
            GameData.Instance.rowCount = int.Parse(rowInputField.text);
            GameData.Instance.mineCount = int.Parse(mineInputField.text);
            GameData.Instance.gameLevel = E_Slot_Level.Custom;
        }
    }
}
