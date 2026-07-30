# System Architecture

## Project

Simple25DRPG

## Purpose

This document defines the high-level architecture, system boundaries, and communication rules for the project.

The architecture must remain simple enough for the current offline prototype while allowing future expansion into an online RPG or MMORPG.

---

## Architecture Goals

The project architecture should provide:

- Clear separation of responsibilities
- Low coupling between systems
- Replaceable input sources
- Testable gameplay logic
- Mobile-friendly performance
- Inspector-driven configuration
- Future multiplayer compatibility
- Minimal unnecessary abstraction

Prefer the simplest implementation that satisfies the current milestone without blocking future development.

---

## High-Level System Map

```text
Input Sources
    |
    v
Player Input
    |
    v
Player Gameplay
    |
    +-------------------+
    |                   |
    v                   v
Movement             Combat
    |                   |
    v                   v
Animation          Damage System
                        |
                        v
                    Health System
                        |
                        v
                  Death / Rewards

Gameplay Systems
    |
    +------> UI Presentation
    |
    +------> Audio and Visual Effects
    |
    +------> Save System
    |
    +------> Future Networking
```

---

## Core Design Principle

Gameplay systems must not depend directly on specific hardware input.

For example:

```text
Keyboard
Virtual Joystick
Gamepad
Touch Input
    |
    v
Input Abstraction
    |
    v
Player Movement
```

The movement controller receives normalized movement values.

It must not directly check keyboard keys, joystick UI objects, or mobile touch state.

---

# System Boundaries

## Input System

### Responsibility

The input system collects player intentions and exposes them to gameplay systems.

Examples:

- Movement direction
- Attack pressed
- Skill pressed
- Interaction pressed
- Menu requested

### Input Sources

Current:

- Keyboard for development

Planned:

- Android virtual joystick
- Mobile action buttons
- Gamepad

### Rules

- Use Unity Input System only.
- Do not use the legacy Input Manager.
- Input classes must not contain movement or combat logic.
- Gameplay components must not depend on a specific device.
- Input values should be exposed through interfaces, properties, or events.

### Recommended Flow

```text
Unity Input System
        |
        v
PlayerInputReader
        |
        +------> PlayerMovementController
        |
        +------> PlayerCombatController
```

---

## Player System

The Player system is composed of small components attached to the player GameObject.

```text
Player
├── CharacterController
├── PlayerInputReader
├── PlayerMovementController
├── PlayerAnimationController
├── PlayerCombatController
├── Health
└── PlayerFacade
```

Not every component must be created immediately.

Only create components required by the current milestone.

---

## Player Input Reader

### Responsibility

- Read Unity Input System actions
- Store the latest movement input
- Raise action events
- Normalize input where appropriate

### Must Not

- Move the CharacterController
- Trigger damage directly
- Control animations directly
- Search for other scene objects every frame

---

## Player Movement Controller

### Responsibility

- Receive movement input
- Convert input into camera-relative movement
- Apply movement through CharacterController
- Rotate the character toward movement direction
- Apply gravity where required

### Dependencies

- CharacterController
- Input provider
- Camera reference or camera direction provider
- Movement settings

### Must Not

- Read keyboard keys directly
- Handle attack logic
- Update UI
- Play unrelated sound effects
- Contain enemy logic

### Data Flow

```text
Movement Input
      |
      v
Camera-Relative Conversion
      |
      v
Movement Calculation
      |
      v
CharacterController.Move()
      |
      v
Animation Parameters
```

---

## Player Combat Controller

### Responsibility

- Receive attack commands
- Validate cooldowns
- Start attacks
- Perform hit detection
- Apply damage through the damage system

### Future Responsibilities

- Combos
- Skills
- Attack speed
- Animation events
- Target validation
- Network command forwarding

### Must Not

- Directly modify enemy private state
- Know enemy UI details
- Read mobile button objects directly
- Contain item inventory logic

---

## Player Animation Controller

### Responsibility

Translate gameplay state into Animator parameters.

Examples:

- Movement speed
- Is moving
- Is attacking
- Is dead
- Hit reaction

### Rules

- Gameplay logic must not depend on exact animation clip names.
- Animator parameter names should be centralized or serialized.
- Animation events may request gameplay actions, but core validation remains in gameplay code.

---

## Player Facade

A PlayerFacade may be introduced when external systems need a single access point to the player.

Possible responsibilities:

- Expose player health
- Expose interaction entry points
- Expose player transform
- Coordinate player-wide enable or disable state

Do not add a facade until multiple external systems genuinely require it.

---

# Camera System

## Responsibility

- Follow the target
- Maintain the 2.5D perspective
- Smooth camera motion
- Provide camera-relative orientation for movement

### Current Requirements

- Perspective projection
- Fixed isometric-style angle
- Configurable offset
- Smooth follow
- Stable movement on mobile

### Future Features

- Zoom
- Camera shake
- Cinematic targets
- Obstacle handling
- Boss framing
- Cinemachine integration

### Rules

- Player movement must not move the camera directly.
- The camera follows a target independently.
- Movement may use the camera's flattened forward and right vectors.
- Camera code should remain independent from combat and health.

### Camera-Relative Movement

Vertical camera tilt must not affect ground movement.

```text
cameraForward.y = 0
cameraRight.y = 0
normalize both vectors
```

Then:

```text
worldDirection =
    cameraForward * inputY +
    cameraRight * inputX
```

---

# Enemy System

```text
Enemy
├── EnemyBrain
├── EnemyMovement
├── EnemyCombat
├── EnemyAnimationController
├── Health
└── LootDropper
```

Only implement required components during the prototype.

---

## Enemy Brain

### Responsibility

Select the enemy's current behavior.

Prototype states:

- Idle
- Chase
- Attack
- Dead

Future states:

- Patrol
- Return Home
- Flee
- Stunned
- Cast Skill

### Rules

- The brain selects intent.
- Movement performs movement.
- Combat performs attacks.
- Health controls alive or dead state.
- Animation reflects the state.

Avoid placing all enemy behavior in one large class.

---

## Enemy Movement

### Responsibility

- Move toward a destination
- Rotate toward movement direction
- Stop at attack range
- Respect movement speed and movement restrictions

Future movement may use:

- NavMesh
- Custom steering
- Server-authoritative positions

Do not assume a networking solution at the prototype stage.

---

## Enemy Combat

### Responsibility

- Validate target
- Check attack range
- Apply cooldown
- Request damage
- Trigger attack animation

The combat component must use the shared damage abstraction.

---

# Health and Damage

## Shared Health Component

Player and enemies should use a common health model where practical.

Responsibilities:

- Store current health
- Store maximum health
- Receive damage
- Receive healing
- Raise health changed events
- Raise death event once

### Rules

- Health cannot fall below zero.
- Health cannot exceed maximum health.
- Death must be idempotent.
- UI should subscribe to health events.
- Combat should not directly change UI.

### Suggested Flow

```text
Attacker
   |
   v
Damage Request
   |
   v
IDamageable.TakeDamage()
   |
   v
Health Updated
   |
   +------> Death Event
   |
   +------> UI Update Event
   |
   +------> Effects
```

---

## Damage Data

A damage request may later contain:

- Source
- Target
- Base damage
- Damage type
- Critical result
- Skill identifier
- Hit position
- Knockback value

For the prototype, start with only the fields currently required.

Do not build a full combat formula system prematurely.

---

# UI System

## Responsibility

Display information and collect UI-specific commands.

Examples:

- HP bar
- Attack button
- Virtual joystick
- Skill buttons
- Inventory windows

### Rules

- UI must not contain core gameplay logic.
- UI may call public commands or publish events.
- Gameplay systems may publish state changes for UI.
- UI should not search for gameplay objects every frame.
- Scene references should be assigned through the Inspector or a controlled composition root.

### Example

```text
Health
   |
   v
HealthChanged Event
   |
   v
HealthBarPresenter
   |
   v
Slider / Image
```

The `Health` component must not know that a Slider exists.

---

# Configuration System

Use ScriptableObjects for reusable game data.

Examples:

- Player movement settings
- Enemy definitions
- Item definitions
- Skill definitions
- Character stats
- Audio settings

Use serialized MonoBehaviour fields for scene-specific references.

### Data Selection

Use a ScriptableObject when:

- Data is shared by multiple objects
- Designers need reusable assets
- Values represent game content
- Runtime systems should consume immutable configuration

Use serialized fields when:

- The value belongs to one scene instance
- The field references another scene object
- Creating an asset would add no practical value

Avoid hardcoded balancing values inside gameplay methods.

---

# Event Communication

Events are preferred when a system publishes a state change to multiple listeners.

Good examples:

- Health changed
- Character died
- Item collected
- Quest updated
- Attack started

Direct references are acceptable when one component clearly owns and collaborates with another.

Do not replace every method call with events.

Use events to reduce coupling, not to create hidden control flow.

---

# Managers

Global managers must be introduced cautiously.

Possible future managers:

- SceneFlowController
- AudioService
- SaveService
- PoolService

Avoid generic classes such as:

- GameManager
- MainManager
- GlobalManager

unless their responsibility is precisely defined.

### Rules

- Avoid unnecessary Singletons.
- Prefer scene composition and dependency injection through serialized references.
- Persistent services must have clear lifetime ownership.
- Do not use static mutable gameplay state.

---

# Scene Architecture

## Prototype Scene

```text
Main
├── Environment
│   └── Ground
├── Player
├── Enemies
├── Cameras
│   └── Main Camera
├── Lighting
│   ├── Directional Light
│   └── Global Volume
├── Systems
└── UI
    └── Canvas
```

Group scene objects by responsibility.

Avoid placing all objects at the scene root without organization.

---

# Composition Root

The scene acts as the initial composition root.

Responsibilities:

- Hold object references
- Connect input to gameplay
- Connect camera to target
- Connect UI to health
- Provide configuration assets

The prototype should rely mainly on Inspector assignment.

A dependency injection framework is not required.

---

# Save System

## Prototype

The initial save system may store:

- Player position
- Player level
- Player experience
- Inventory
- Settings

Do not implement saving until required by the roadmap.

### Rules

- Gameplay data should be converted into serializable save models.
- Do not serialize MonoBehaviour references.
- Save format should include a version number.
- Runtime state and save data must remain separate.

### Future

- Local JSON save
- Cloud synchronization
- Server-authoritative persistence

---

# Audio and Effects

Audio and visual effects listen to gameplay events.

Examples:

```text
Attack Started
    +------> Play attack sound
    +------> Spawn slash effect

Damage Received
    +------> Play hit sound
    +------> Spawn hit effect
    +------> Camera shake
```

Core combat must still function when effects are unavailable.

---

# Object Pooling

Use pooling for frequently created runtime objects.

Candidates:

- Damage numbers
- Projectiles
- Hit effects
- Loot effects
- Enemies in repeated spawn systems

Do not add pooling for objects that are rarely created.

---

# Future Networking Boundary

Networking is not part of the current prototype.

However, local gameplay must avoid unnecessary coupling to local input.

Future flow:

```text
Local Input
    |
    v
Player Command
    |
    v
Simulation
    |
    v
Presentation
```

A remote player should eventually be able to use the same movement and animation presentation without a local keyboard or joystick.

### Networking Preparation Rules

- Input is separate from movement.
- Gameplay state is separate from UI.
- Avoid global static player references.
- Avoid relying on frame-perfect local-only assumptions.
- Do not implement prediction or synchronization yet.

---

# Assembly Definitions

Assembly Definition files are not required during the first small prototype.

Introduce them when:

- Compile times become meaningful
- Runtime and editor code need separation
- Tests require isolated assemblies
- Package-like modules emerge

Do not add many assemblies prematurely.

---

# Dependency Direction

Preferred dependency direction:

```text
Presentation
    |
    v
Gameplay
    |
    v
Shared Contracts and Data
```

Examples:

- UI depends on health contracts.
- Health does not depend on UI.
- Input adapts devices into gameplay commands.
- Movement does not depend on keyboard APIs.
- Effects depend on gameplay events.
- Combat does not depend on specific effects.

---

# Initial Prototype Architecture

The first playable prototype should begin with:

```text
PlayerInputReader
PlayerMovementController
CameraFollow
```

Then add:

```text
Health
PlayerCombatController
EnemyBrain
EnemyMovement
EnemyCombat
HealthBarPresenter
```

Build each vertical feature completely before adding the next large system.

---

# Architecture Review Checklist

Before adding a new system, verify:

- Does the class have one clear responsibility?
- Is the system required for the current milestone?
- Can it work without desktop-only input?
- Is gameplay separated from UI?
- Are dependencies explicit?
- Are scene references cached?
- Is configuration exposed appropriately?
- Does it avoid unnecessary global state?
- Can it be extended without rewriting working systems?
- Is the implementation simpler than the alternatives?

---

# Source of Truth

When implementation and documentation disagree:

1. Confirm the current intended behavior.
2. Update the implementation.
3. Update this document.
4. Update the changelog where appropriate.

Architecture documentation must evolve with the project.