# TapMatchGame

## Overview

A casual matching game where players tap pairs of matching objects on a grid. The project is structured around modular Services, ViewControllers and Models, with clear separation between game logic, state management and view presentation. Core systems are organized using dependency injection using VContainer. The game is structured around asynchronous execution using UniTask.

Configurations are stored in ScriptableObjects **GridConfiguration** and **MatchableColorConfiguration**

## High-Level Architecture

### Game Architecture

- Bootstrap (MainScene)
  - VContainer Scope
    - GameInstance
      - AssetService (Addressables Loader)
      - ModelService
        - GameState
          - GridModel
      - InputService
      - UIRoot
      - GridViewController (Controller)
        - GridWindow (View)
        - MatchableView (View)
                    
### Test Framework

- SmokeTests (PlayMode)

- PlayModeTestBase
  - ViewControllerTestBase
    - GridViewControllerTests
  - ServiceTestBase
    - AssetServiceTests

- EditModeTestBase
  - ModelTestBase
    - GridModelTests


## Central Classes

### GameInstance
Game entry point. Initializes services and calls for first ViewController to Instantiate itself.

### AssetService
Handles loading and unloading of Addressables.

### ModelService
Stores all game models, currently GridModel in GameState. Takes in Actions to Execute logic on the Models. Provides access to readonly versions of Models to Views through interface.

### ViewController
Base class for handling view prefabs stored in Addressables. Uses AssetService to Instantiate the View into UIRoot when called to do so. Can hide and show itself and inheriting classes have access to OnEvent methods to run logic when Show, Hide or Instantiate are called.

### GridViewController
Handles GridWindow view and MatchableView logic. Instantiates the Grid and Matchables through an ObjectPool and stores their references. Is subscribed to MatchableViews OnPointerDown event in order to run game logic. OnTap creates a TapMatch Action and sends it to ModelService to be handled. The class then uses the returning result data to animate the View in accordance with the changed Model state.

### UIRoot
Provides RectTransform for the UI parent for the game.

### InputService
Blocks UI input through disabling Graphic raycaster. Can be accessed by Views through IInputService which can provide a scoped InputBlock object. When called the object blocks input through a callback and enables it back OnDispose.
             
