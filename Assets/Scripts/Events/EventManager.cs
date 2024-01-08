using System;

namespace MineSweeper
{
    public class EventManager
    {
        public SlotEvent<int, int> onFindBoundary;
        public SlotEvent onGameOver;
        public SlotEvent onRestartGame;
        public SlotEvent<E_Slot_Level> onLevelChanged;
        public SlotEvent<int> onRemainMineChanged;
        public SlotEvent onGameWin;
        public SlotEvent<int, int> onLeftRightClicked;
        public SlotEvent onPlayClickAudio;

        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventManager();
                return _instance;
            }
        }
        private EventManager()
        {
            onFindBoundary = new SlotEvent<int, int>();
            onGameOver = new SlotEvent();
            onRestartGame = new SlotEvent();
            onLevelChanged = new SlotEvent<E_Slot_Level>();
            onRemainMineChanged = new SlotEvent<int>();
            onGameWin = new SlotEvent();
            onLeftRightClicked = new SlotEvent<int, int>();
            onPlayClickAudio = new SlotEvent();
        }
    }

    public class SlotEvent
    {
        private Action eventAction;

        public void Register(Action action)
        {
            eventAction += action;
        }

        public void UnRegister(Action action)
        {
            eventAction -= action;
        }

        public void Trigger()
        {
            eventAction?.Invoke();
        }
    }

    public class SlotEvent<T>
    {
        private Action<T> eventAction;

        public void Register(Action<T> action)
        {
            eventAction += action;
        }

        public void UnRegister(Action<T> action)
        {
            eventAction -= action;
        }

        public void Trigger(T param)
        {
            eventAction?.Invoke(param);
        }
    }

    public class SlotEvent<T0, T1>
    {
        private Action<T0, T1> eventAction;

        public void Register(Action<T0, T1> action)
        {
            eventAction += action;
        }

        public void UnRegister(Action<T0, T1> action)
        {
            eventAction -= action;
        }

        public void Trigger(T0 param0, T1 param1)
        {
            eventAction?.Invoke(param0, param1);
        }
    }
}
