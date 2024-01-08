using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MineSweeper
{
    [CreateAssetMenu(fileName = "SlotSpritesSetting", menuName = "Slot Sprites Setting")]
    public class SlotSpritesSettingSO : ScriptableObject
    {
        public GameObject slotPrefab;
        public SlotSpritesSetting slotSpritesSetting;
    }
}
