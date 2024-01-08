namespace MineSweeper
{
    public enum E_Slot_ActualState
    {
        IsMine,
        HasMineAround,
        HasNoMineAround
    }

    public enum E_Slot_SignState
    {
        NotSigned,
        SignedMine,
        SignedNotSure
    }

    public enum E_Slot_Level
    {
        Easy,
        Normal,
        Hard,
        Custom
    }
}
