# tapmatchgame

**Overview**
A casual matching game where players tap pairs of matching objects on a grid. The project is structured around modular Services, ViewControllers and Models, with clear separation between game logic, state management and view presentation. Core systems are organized using dependency injection using VContainer. The game is structured around asynchronous execution using UniTask.

Configurations are stored in ScriptableObjects **GridConfiguration** and **MatchableColorConfiguration**

**High-Level Architecture**

Bootstrap (MainScene)
   └── VContainer Scope
        └── GameInstance
             ├── AssetService (Addressables Loader)
             ├── ModelService
             |    └── GameState
             |          └── GridModel 
             ├── InputService
             ├── UIRoot
             └── GridViewController (Controller)
                   ├── GridWindow (View)
                   └── MatchableView (View)
                    
**Test Framework**

SmokeTests (PlayMode)

PlayModeTestBase
   ├── ViewControllerTestBase
        └── GridViewControllerTests      
   └── ServiceTestBase
        └── AssetServiceTests

EditModeTestBase
   └── ModelTestBase
        └── GridModelTests
        
             
