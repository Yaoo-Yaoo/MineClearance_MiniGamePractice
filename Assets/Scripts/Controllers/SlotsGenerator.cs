using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MineSweeper
{
    public class SlotsGenerator : MonoBehaviour
    {
        [SerializeField] private SlotSpritesSettingSO slotSpritesSettingSO;
        [SerializeField] private RectTransform uiSlotsPanel;
        [SerializeField] private float camOffsetRate = 0.2f;
        private GameObject slotPrefab;
        private RectTransform slotRect;
        private SlotData[,] allSlots;
        private int xCount;
        private int yCount;

        private void Awake()
        {
            if (slotSpritesSettingSO != null)
            {
                slotPrefab = slotSpritesSettingSO.slotPrefab;
                slotRect = slotPrefab.GetComponentInChildren<RectTransform>();
            }

            EventManager.Instance.onFindBoundary.Register(OnFindBoundary);
            EventManager.Instance.onGameOver.Register(OnGameOver);
            EventManager.Instance.onRestartGame.Register(OnRestart);
            EventManager.Instance.onLevelChanged.Register(OnLevelChanged);
            EventManager.Instance.onLeftRightClicked.Register(OnCheckAround);
        }

        private void Start()
        {
            GameData.Instance.gameLevel = E_Slot_Level.Easy;
        }

        private void OnDestroy()
        {
            EventManager.Instance.onFindBoundary.UnRegister(OnFindBoundary);
            EventManager.Instance.onGameOver.UnRegister(OnGameOver);
            EventManager.Instance.onRestartGame.UnRegister(OnRestart);
            EventManager.Instance.onLevelChanged.UnRegister(OnLevelChanged);
            EventManager.Instance.onLeftRightClicked.UnRegister(OnCheckAround);
        }

        public void CreateSlots(int xCount, int yCount, int mineCount)
        {
            if (slotPrefab == null || uiSlotsPanel == null) return;

            this.xCount = xCount;
            this.yCount = yCount;
            GameData.Instance.remainSignedMineCount = mineCount;
            GameData.Instance.needClickSlotCount = xCount * yCount - mineCount;

            //生成格子
            allSlots = new SlotData[xCount, yCount];
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    GameObject slot = Instantiate(slotPrefab);
                    slot.transform.SetParent(uiSlotsPanel);
                    slot.transform.localPosition = Vector3.right * (i - (float)(xCount - 1) / 2) * slotRect.sizeDelta.x + Vector3.up * (j - (float)(yCount - 1) / 2) * slotRect.sizeDelta.y;
                    slot.transform.localScale = Vector3.one;
                    slot.name = $"{i},{j}";
                    allSlots[i, j] = new SlotData(slot, slotSpritesSettingSO.slotSpritesSetting);
                }
            }
            float scaleFactor = Camera.main.orthographicSize * 2 / Mathf.Max(xCount * Screen.height / (Screen.width - 500), yCount);
            uiSlotsPanel.localScale = Vector3.one * scaleFactor * (1 - camOffsetRate);

            //随机雷的索引
            List<int> mineSlotsIndex = new List<int>();
            int maxIndex = xCount * yCount;
            while (mineSlotsIndex.Count < mineCount)
            {
                int randomIndex = Random.Range(0, maxIndex);
                if (!mineSlotsIndex.Contains(randomIndex))
                    mineSlotsIndex.Add(randomIndex);
            }

            //设置雷格子
            SlotData[] mineSlots = new SlotData[mineCount];
            for (int i = 0; i < mineCount; i++)
            {
                GameObject slot = uiSlotsPanel.GetChild(mineSlotsIndex[i]).gameObject;
                string[] slotPos = slot.name.Split(',');
                SlotData slotData = allSlots[System.Int32.Parse(slotPos[0]), System.Int32.Parse(slotPos[1])];
                slotData.slotActualState = E_Slot_ActualState.IsMine;
                slotData.mineCountAround = -1;
                mineSlots[i] = slotData;
            }

            //计算非雷格子周围雷的个数
            for (int i = 0; i < mineCount; i++)
            {
                string[] slotPos = mineSlots[i].slotObj.name.Split(',');
                int indexX = System.Int32.Parse(slotPos[0]);
                int indexY = System.Int32.Parse(slotPos[1]);
                for (int x = Mathf.Max(0, indexX - 1); x <= Mathf.Min(indexX + 1, xCount - 1); x++)
                {
                    for (int y = Mathf.Max(0, indexY - 1); y <= Mathf.Min(indexY + 1, yCount - 1); y++)
                    {
                        SlotData slotData = allSlots[x, y];
                        if (slotData.slotActualState == E_Slot_ActualState.IsMine) continue;

                        if (slotData.slotActualState != E_Slot_ActualState.HasMineAround)
                            slotData.slotActualState = E_Slot_ActualState.HasMineAround;
                        slotData.mineCountAround++;
                        slotData.slotObj.GetComponentInChildren<Text>().text = slotData.mineCountAround.ToString();
                    }
                }
            }
        }

        private void ClearSlots()
        {
            if (allSlots == null || allSlots.GetLength(0) == 0 || allSlots.GetLength(1) == 0) return;

            foreach (SlotData slotData in allSlots)
            {
                Destroy(slotData.slotObj);
            }
            allSlots = new SlotData[0, 0];
            xCount = 0;
            yCount = 0;
        }

        private void OnFindBoundary(int xIndex, int yIndex)
        {
            CheckAround(xIndex, yIndex);
        }

        private void CheckAround(int xIndex, int yIndex)
        {
            for (int x = Mathf.Max(0, xIndex - 1); x <= Mathf.Min(xIndex + 1, xCount - 1); x++)
            {
                for (int y = Mathf.Max(0, yIndex - 1); y <= Mathf.Min(yIndex + 1, yCount - 1); y++)
                {
                    SlotData currentSlotData = allSlots[x, y];
                    if (currentSlotData.hasShown) continue;

                    if (currentSlotData.slotActualState == E_Slot_ActualState.HasMineAround)  //找到边界
                    {
                        currentSlotData.ShowActual();
                        GameData.Instance.needClickSlotCount--;
                    }
                    else if (currentSlotData.slotActualState == E_Slot_ActualState.HasNoMineAround)  //递归
                    {
                        currentSlotData.ShowActual();
                        GameData.Instance.needClickSlotCount--;
                        CheckAround(x, y);
                    }
                }
            }
        }

        private void OnGameOver()
        {
            foreach (SlotData slotData in allSlots)
            {
                if (!slotData.hasShown) slotData.ShowActual();
            }
        }

        private void OnRestart()
        {
            OnLevelChanged(GameData.Instance.gameLevel);
        }

        private void OnLevelChanged(E_Slot_Level level)
        {
            StopAllCoroutines();
            ClearSlots();
            StartCoroutine(DelayCreateNewSlots(level));
        }

        private IEnumerator DelayCreateNewSlots(E_Slot_Level level)
        {
            while (uiSlotsPanel.childCount > 0) yield return null;

            CreateSlots(GameData.Instance.columnCount, GameData.Instance.rowCount, GameData.Instance.mineCount);
        }

        private void OnCheckAround(int xIndex, int yIndex)
        {
            SlotData centerSlotData = allSlots[xIndex, yIndex];
            int aroundSignedMineCount = 0;
            bool isGameOver = false;
            List<SlotData> wrongSlots = new List<SlotData>();

            //检查标记雷的数量是否正确
            for (int x = Mathf.Max(0, xIndex - 1); x <= Mathf.Min(xIndex + 1, xCount - 1); x++)
            {
                for (int y = Mathf.Max(0, yIndex - 1); y <= Mathf.Min(yIndex + 1, yCount - 1); y++)
                {
                    if (x == xIndex && y == yIndex) continue;

                    SlotData currentSlotData = allSlots[x, y];
                    if (currentSlotData.slotSignedState == E_Slot_SignState.SignedMine) aroundSignedMineCount++;
                    if (currentSlotData.isSignedWrong)
                    {
                        isGameOver = true;
                        wrongSlots.Add(currentSlotData);
                    }
                }
            }
            if (centerSlotData.mineCountAround != aroundSignedMineCount) return;
            //检查雷的标记是否正确
            if (isGameOver)
            {
                foreach (SlotData slot in wrongSlots)
                {
                    slot.OnWrong();
                }
                EventManager.Instance.onGameOver.Trigger();
                return;
            }
            //若正确无误
            for (int x = Mathf.Max(0, xIndex - 1); x <= Mathf.Min(xIndex + 1, xCount - 1); x++)
            {
                for (int y = Mathf.Max(0, yIndex - 1); y <= Mathf.Min(yIndex + 1, yCount - 1); y++)
                {
                    if (x == xIndex && y == yIndex) continue;

                    SlotData currentSlotData = allSlots[x, y];
                    if (currentSlotData.slotActualState != E_Slot_ActualState.IsMine && !currentSlotData.hasShown)
                    {
                        currentSlotData.ShowActual();
                        GameData.Instance.needClickSlotCount--;
                        if (currentSlotData.slotActualState == E_Slot_ActualState.HasNoMineAround) OnFindBoundary(x, y);
                    }
                }
            }
        }
    }
}
