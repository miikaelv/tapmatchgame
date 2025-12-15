using System;
using System.Collections.Generic;

namespace TapMatch.Models.Configs
{
    [Serializable]
    public class GridConfig
    {
        public int Width;
        public int Height;
        public List<MatchableType> ValidMatchables;
        
        public GridConfig(int width, int height, List<MatchableType> validMatchables)
        {
            Width = width;
            Height = height;
            ValidMatchables = validMatchables;
        }
    }
    
    public enum MatchableType
    {
        None,
        Red,
        Green,
        Blue,
        Yellow,
        Pink,
    }
}