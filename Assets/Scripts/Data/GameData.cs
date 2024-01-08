namespace MineSweeper
{
    public class GameData
    {
        private E_Slot_Level m_gameLevel;
        public E_Slot_Level gameLevel
        {
            get => m_gameLevel;
            set
            {
                m_gameLevel = value;
                switch (value)
                {
                    case E_Slot_Level.Easy:
                        columnCount = 9;
                        rowCount = 9;
                        mineCount = 10;
                        break;
                    case E_Slot_Level.Normal:
                        columnCount = 16;
                        rowCount = 16;
                        mineCount = 40;
                        break;
                    case E_Slot_Level.Hard:
                        columnCount = 30;
                        rowCount = 16;
                        mineCount = 99;
                        break;
                    case E_Slot_Level.Custom:
                        break;
                }
                EventManager.Instance.onLevelChanged.Trigger(value);
            }
        }

        public int rowCount;
        public int columnCount;
        public int mineCount;

        private int m_remainSignedMineCount;
        public int remainSignedMineCount
        {
            get => m_remainSignedMineCount;
            set
            {
                m_remainSignedMineCount = value;
                EventManager.Instance.onRemainMineChanged.Trigger(value);
                if (value == 0 && needClickSlotCount == 0) EventManager.Instance.onGameWin.Trigger();
            }
        }

        private int m_needClickSlotCount;
        public int needClickSlotCount
        {
            get => m_needClickSlotCount;
            set
            {
                m_needClickSlotCount = value;
                if (value == 0 && remainSignedMineCount == 0) EventManager.Instance.onGameWin.Trigger();
            }
        }

        private static GameData _instance;
        public static GameData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameData();
                return _instance;
            }
        }
        private GameData() { }
    }
}
