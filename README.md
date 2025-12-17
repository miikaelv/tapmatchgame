# TapMatchGame

## Overview

A casual matching game where players tap pairs of matching objects on a grid. The project is structured around modular Services, ViewControllers and Models, with clear separation between game logic, state management and view presentation. Core systems are organized using dependency injection using VContainer. The game is structured around asynchronous execution using UniTask.

Configurations are stored in ScriptableObjects **GridConfiguration** and **MatchableColorConfiguration**

### Notes
I was too focused initially on the 7 days part of the assignment and only noticed halfway through that there was a time estimate of 8 hours. Time to make the project exceeded that since my scope was bit too big for it, but managed to finish in something around 16 hours total time coding. Very sorry about out that. Started out testing more, but cut down later to save time. Though the TestBases should make it much easy to add the remaining test cases. 

A big trade-off in the architecture is that it is made for a more deterministic game like traditional tap match, not a physics based one like Dream Blast. That would require a different approach since the physics calculation determines what can be matched. Unless you write your own physics engine in C# without UnityEngine you could then only validate things like amount of which color tiles and level requirements, but not match logic itself on server-side.

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

- AcceptanceTestBase
  - GridAcceptanceTests
 
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


## AI Usage
Used AI to help find bugs. I paste in code that throws or does not work as I intended and AI tries to help with finding the bug. Sometimes helps, sometimes doesn’t. 

Also used AI sometimes to brainstorm what approach to take before writing method / algorithm. I would then write the code out myself, combining ideas the AI and myself had. i.e. ApplyGravity method in GridModel. 

Often I like for AI to find me links to reference code / documentation online that a human has written to avoid weird hallucinations, such as with help using VContainer.
