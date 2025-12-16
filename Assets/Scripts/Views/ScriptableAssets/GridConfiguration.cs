using TapMatch.Models.Configs;
using UnityEngine;

namespace TapMatch.Views.ScriptableAssets
{
    [CreateAssetMenu(fileName = "GridConfiguration", menuName = "TapMatch/GridConfiguration")]
    public class GridConfiguration : ScriptableObject
    {
        public GridConfig GridConfig;
    }
}