using TapMatch.Models.Configs;
using UnityEngine;

namespace TapMatch.UnityServices.ScriptableAssets
{
    [CreateAssetMenu(fileName = "GridConfiguration", menuName = "TapMatch/GridConfiguration")]
    public class GridConfiguration : ScriptableObject
    {
        public GridConfig GridConfig;
    }
}