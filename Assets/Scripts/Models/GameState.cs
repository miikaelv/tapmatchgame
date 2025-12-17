using TapMatch.Models.Configs;

namespace TapMatch.Models
{
    // ReadOnly version of GameState for Views
    public interface IGameStateReader
    {
        public IGridReader GridReader { get; }
    }

    // Models and Read/Write and Aggregation Methods stored here for whole game logic
    public class GameState : IGameStateReader
    {
        public readonly GridModel GridModel;
        public IGridReader GridReader => GridModel;

        public GameState(GridConfig gridConfig)
        {
            GridModel = new GridModel(gridConfig, new System.Random());
        }
    }
}