using System;
using System.Collections.Generic;
using System.Linq;
using TapMatch.Models.Configs;
using UnityEngine;

namespace TapMatch.Views.ScriptableConfigs
{
    [CreateAssetMenu(fileName = "MatchableColorConfiguration", menuName = "TapMatch/MatchableColorConfiguration")]
    public class MatchableColorConfiguration : ScriptableObject
    {
        [SerializeField] private List<MatchableColorData> MatchableColorsData;

        public Dictionary<MatchableType, Color> GetMatchableColorDictionary() =>
            MatchableColorsData.ToDictionary(k => k.Type, v => v.Sprite);
    }

    [Serializable]
    public class MatchableColorData
    {
        public MatchableType Type;
        public Color Sprite;
    }
}