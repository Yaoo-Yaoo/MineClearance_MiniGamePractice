using System;
using System.Collections.Generic;
using UnityEngine;

namespace MineSweeper
{
    [Serializable]
    public class SlotSpritesSetting
    {
        [Header("格子实际数据相关贴图")]
        public Sprite mineSprite;
        public Color[] numberSprites = new Color[8];
        public Sprite emptySprite;

        [Header("格子外观数据相关贴图")]
        public Sprite unclickedSprite;
        public Sprite signedMineSprite;
        public Sprite signedNotSureSprite;
        public Sprite mineBoom;
        public Sprite signedRightSprite;
    }
}
