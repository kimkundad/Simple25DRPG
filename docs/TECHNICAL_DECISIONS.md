# Technical Decisions

## Project

Simple25DRPG

## Purpose

This document records important technical and architectural decisions.

It explains why each choice was made, which alternatives were considered, and when a decision should be reviewed.

The goal is to prevent repeated debates, inconsistent implementations, and accidental architectural changes by developers or AI assistants.

---

# Decision Status

Each decision uses one of the following statuses:

- Proposed
- Accepted
- Superseded
- Deprecated
- Rejected

---

# TD-001: Use Unity 6 with Universal Render Pipeline

## Status

Accepted

## Decision

Use Unity 6.5 or the current project-approved Unity 6 version with Universal Render Pipeline.

## Context

The game targets Android and uses stylized 2.5D graphics.

The rendering solution must support:

- Mobile optimization
- Stylized materials
- Lighting and shadows
- Post-processing
- Broad Unity support

## Reasons

- URP is designed for scalable rendering across mobile and desktop.
- It provides better control over mobile performance than HDRP.
- It supports the planned visual style.
- It is suitable for 3D environments viewed through a 2.5D camera.
- It is officially maintained as part of Unity.

## Alternatives Considered

### Built-in Render Pipeline

Rejected because it is less suitable for a new long-term Unity project and offers a less modern rendering workflow.

### High Definition Render Pipeline

Rejected because it targets high-end hardware and is unnecessarily expensive for the Android-first goal.

## Consequences

- Materials and shaders must be URP compatible.
- Imported assets using Built-in shaders may require conversion.
- Rendering features must be tested on target Android devices.

## Review Trigger

Review only if project platform requirements change significantly.

---

# TD-002: Android Is the Primary Target Platform

## Status

Accepted

## Decision

Design and test the project as mobile-first, with Android ARM64 as the primary build target.

## Context

The game is intended to run primarily on Android devices.

Desktop play mode exists for development convenience.

## Reasons

- Mobile constraints affect UI, performance, input, memory, and rendering.
- Designing mobile-first prevents expensive optimization work later.
- Android testing can begin during the prototype stage.

## Consequences

- Avoid desktop-only APIs.
- Touch controls must be considered from the beginning.
- Performance must be tested on real devices.
- UI must support different screen sizes and aspect ratios.
- Texture, mesh, shadow, and post-processing budgets must remain controlled.

## Review Trigger

Review when adding iOS, PC, or console as an official target.

---

# TD-003: Use Unity Input System

## Status

Accepted

## Decision

Use the Unity Input System package for all player input.

Do not use the legacy Input Manager.

## Context

The game must support development keyboard input and future Android touch controls.

## Reasons

- It supports multiple device types through one action-based system.
- Input Actions can map keyboard, gamepad, joystick, and touch controls.
- It separates player intentions from physical keys.
- It is appropriate for future local multiplayer or control rebinding.

## Alternatives Considered

### Legacy Input Manager

Rejected because it is device-oriented, less flexible, and not appropriate for the project's long-term control architecture.

### Custom Input Polling

Rejected because it would recreate features already provided by the Unity Input System.

## Consequences

- Gameplay controllers receive input values rather than read devices directly.
- Input Actions must be included in version control.
- Mobile UI controls must write into the same gameplay-facing abstraction.

## Review Trigger

No expected review unless Unity replaces the Input System.

---

# TD-004: Separate Input from Gameplay

## Status

Accepted

## Decision

Input components collect commands, while gameplay components execute behavior.

## Context

Player movement and combat must work with keyboard, virtual joystick, gamepad, remote commands, or AI input.

## Reasons

- Prevents movement logic from depending on hardware.
- Makes systems easier to test.
- Supports future multiplayer architecture.
- Allows mobile controls without rewriting movement.

## Consequences

The following is prohibited inside movement and combat controllers:

```csharp
Keyboard.current
Input.GetAxis
Input.GetKey
```

Input should instead be provided through a dedicated reader, interface, or command source.

## Review Trigger

Review when formal command buffering or networking is introduced.

---

# TD-005: Use CharacterController for Player Movement

## Status

Accepted

## Decision

Use Unity's CharacterController for player locomotion during the prototype and offline RPG phases.

## Context

The player requires responsive action-RPG movement on mostly grounded 3D environments.

## Reasons

- Provides direct and predictable movement.
- Works well for responsive player-controlled characters.
- Avoids unwanted physical reactions.
- Simplifies slope and collision handling.
- Suitable for camera-relative movement.

## Alternatives Considered

### Rigidbody

Rejected for initial player movement because physics-driven motion may introduce sliding, force tuning, jitter, and less predictable control.

Rigidbody remains acceptable for physical objects, projectiles, ragdolls, and gameplay objects that genuinely require physics.

### Transform Translation

Rejected because it does not provide appropriate collision handling.

### NavMeshAgent

Rejected for the local player because it is better suited to pathfinding-driven agents than direct joystick movement.

## Consequences

- Gravity must be applied manually.
- CharacterController.Move must be called by the movement controller.
- Physics forces do not automatically affect the player.
- Knockback requires a deliberate implementation.

## Review Trigger

Review if the game later requires physics-heavy player locomotion.

---

# TD-006: Use a Perspective 2.5D Camera

## Status

Accepted

## Decision

Use a perspective camera with a fixed elevated angle and smooth target following.

## Context

The desired style is inspired by Tree of Savior, Ragnarok Online, and isometric action RPGs.

## Reasons

- Perspective gives depth while maintaining a 2.5D presentation.
- It works naturally with 3D environments and characters.
- It supports camera-relative movement.
- It allows future zoom, shake, and cinematic behavior.

## Alternatives Considered

### Orthographic Camera

Not selected initially because it produces a flatter presentation and different visual scale behavior.

It may still be evaluated for specific maps or art-direction tests.

## Consequences

- Camera angle and field of view affect perceived asset scale.
- Camera-relative vectors must be flattened onto the ground plane.
- Environment design must account for occlusion.

## Review Trigger

Review after the first representative environment and character art are available.

---

# TD-007: Start Without Cinemachine

## Status

Accepted

## Decision

Implement a small custom camera-follow component for the first prototype.

Do not add Cinemachine until its features are required.

## Context

The initial camera needs only:

- Target follow
- Fixed offset
- Smoothing

## Reasons

- Keeps the first implementation easy to understand.
- Avoids adding package complexity for a simple requirement.
- Makes the movement-camera relationship explicit for learning purposes.

## Alternatives Considered

### Cinemachine from the Beginning

Deferred rather than rejected.

Cinemachine is suitable when the project requires:

- Camera blending
- Confiner volumes
- Advanced damping
- Multiple virtual cameras
- Target groups
- Complex cinematic behavior

## Consequences

- The custom camera component must remain small.
- It should be replaceable later without changing player movement.
- Do not recreate advanced Cinemachine functionality manually.

## Review Trigger

Review when multiple camera modes or camera collision are required.

---

# TD-008: Use Component-Based Gameplay Architecture

## Status

Accepted

## Decision

Compose gameplay objects from focused MonoBehaviour components.

Examples:

```text
PlayerInputReader
PlayerMovementController
PlayerCombatController
PlayerAnimationController
Health
```

## Context

Player and enemy behavior will grow over time.

A single large script would become difficult to maintain.

## Reasons

- Encourages one responsibility per component.
- Allows features to be enabled, disabled, or replaced.
- Matches Unity's composition model.
- Reduces large interconnected classes.

## Alternatives Considered

### One PlayerController Class

Rejected for long-term use because movement, input, combat, animation, and health would become tightly coupled.

### Deep Inheritance Hierarchy

Rejected because it tends to create fragile base classes and unclear behavior.

## Consequences

- Dependencies between sibling components must be explicit.
- Avoid splitting trivial behavior into meaningless micro-components.
- Components that always operate together may reference each other through serialized fields or cached components.

## Review Trigger

Review if component count becomes difficult to manage.

---

# TD-009: Prefer Inspector Composition Over a Dependency Injection Framework

## Status

Accepted

## Decision

Use serialized references, `GetComponent` caching during initialization, and scene composition for the prototype.

Do not add a dependency injection framework yet.

## Context

The project is currently small and developed in Unity scenes and prefabs.

## Reasons

- Inspector wiring is visible and beginner-friendly.
- It avoids introducing a framework before a real need exists.
- It fits the scale of the initial prototype.

## Alternatives Considered

### Dependency Injection Framework

Deferred.

A framework may become useful when:

- Service lifetimes become complicated
- Automated tests require extensive composition
- Many scenes share complex dependency graphs
- Runtime module loading is introduced

## Consequences

- Required references must be validated.
- Missing Inspector references should produce clear errors.
- Repeated global lookups are prohibited.

## Review Trigger

Review when composition becomes repetitive or difficult across many scenes.

---

# TD-010: Avoid Global Mutable State and Unnecessary Singletons

## Status

Accepted

## Decision

Do not use Singletons or static mutable fields as the default method of system access.

## Context

Singletons are convenient initially but create hidden dependencies and lifecycle problems as projects grow.

## Reasons

- Explicit dependencies are easier to understand.
- Scene transitions become safer.
- Tests become easier.
- Future multiplayer may contain multiple player entities.

## Acceptable Uses

A persistent application-level service may use a controlled single-instance lifetime when clearly justified.

Examples may eventually include:

- Save service
- Audio service
- Scene flow service

The decision must be documented when such a service is introduced.

## Consequences

- Systems receive references explicitly.
- Avoid patterns such as `Player.Instance`.
- Avoid static health, inventory, or gameplay state.

## Review Trigger

Review individually for each proposed global service.

---

# TD-011: Use ScriptableObjects for Reusable Game Definitions

## Status

Accepted

## Decision

Use ScriptableObjects for reusable configuration and content definitions.

Examples:

- Item definitions
- Skill definitions
- Enemy definitions
- Movement settings
- Character base stats

## Context

Game designers and developers need editable data that can be shared across multiple runtime instances.

## Reasons

- Data is editable in the Unity Inspector.
- Assets can be referenced by prefabs.
- Shared data avoids duplication.
- Runtime behavior remains separate from content definitions.

## Consequences

- ScriptableObject assets must not accidentally store per-instance runtime state.
- Runtime values such as current HP belong to scene instances or runtime models.
- Data migrations must be considered when schemas change.

## Review Trigger

Review when external data pipelines or server-driven content are introduced.

---

# TD-012: Use Shared Health and Damage Contracts

## Status

Accepted

## Decision

Player and enemies should use a shared health and damage abstraction where their behavior overlaps.

## Context

Many entities can receive damage and die.

Duplicating health logic would create inconsistent behavior.

## Reasons

- One implementation can enforce health limits and death rules.
- Combat systems can target an interface rather than enemy-specific classes.
- UI and effects can subscribe to common events.

## Consequences

- Player-specific death handling remains outside the generic health class.
- Enemy rewards and loot remain outside the generic health class.
- The shared component must remain focused.

## Review Trigger

Review when shields, elemental damage, or advanced status systems are introduced.

---

# TD-013: Use Events for State Notifications, Not Every Interaction

## Status

Accepted

## Decision

Use events for meaningful state changes with potentially multiple listeners.

Use direct method calls for clear one-to-one commands.

## Suitable Events

- Health changed
- Character died
- Item collected
- Quest progressed
- Attack completed

## Suitable Direct Calls

- Movement controller receives movement vector
- Combat controller requests damage
- Camera follows a target

## Reasons

- Events reduce coupling between state owners and presentation.
- Direct calls preserve readability for explicit collaboration.
- Avoiding an event-only architecture prevents hidden execution flow.

## Consequences

- Event subscriptions must be removed correctly.
- Event ownership must be clear.
- Do not create a global event bus during the prototype.

## Review Trigger

Review if cross-scene event communication becomes necessary.

---

# TD-014: Keep UI Separate from Gameplay Logic

## Status

Accepted

## Decision

UI components present state and collect UI commands.

They do not own core combat, health, inventory, or progression rules.

## Context

The game will support both mobile UI and development controls.

## Reasons

- Gameplay remains reusable without a specific screen.
- UI can be redesigned without rewriting game rules.
- Automated tests become easier.
- Multiple UI layouts can consume the same state.

## Consequences

- Health publishes changes; health bars display them.
- Attack buttons request an attack; they do not calculate damage.
- Virtual joysticks provide movement values; they do not move transforms.

## Review Trigger

No expected review.

---

# TD-015: Optimize for 60 FPS but Measure Before Complex Optimization

## Status

Accepted

## Decision

Target 60 FPS on supported Android devices while avoiding premature complex optimization.

## Baseline Rules

- Cache component references.
- Avoid object searches in Update.
- Avoid LINQ in per-frame gameplay.
- Avoid avoidable per-frame allocations.
- Pool frequently spawned objects.
- Profile representative devices.

## Context

Mobile hardware has CPU, GPU, thermal, and memory constraints.

## Reasons

- Basic performance discipline prevents common issues.
- Profiling is more reliable than assumptions.
- Premature optimization can damage readability.

## Consequences

- Performance-sensitive systems must be measured.
- Visual quality tiers may be needed later.
- The minimum supported device must eventually be defined.

## Review Trigger

Review after first Android profiling results.

---

# TD-016: Do Not Implement Networking During the Prototype

## Status

Accepted

## Decision

Build the first milestone as an offline playable prototype.

Preserve reasonable separation between input, simulation, and presentation without implementing networking abstractions prematurely.

## Context

Networking would increase complexity before the core gameplay is validated.

## Reasons

- Movement and combat must be fun before online synchronization.
- Networking affects architecture, testing, hosting, security, and persistence.
- Early networking would slow iteration substantially.

## Consequences

- No networking package is selected yet.
- No prediction or reconciliation is implemented.
- Avoid local-input dependencies inside gameplay.
- Avoid code that assumes only one player can ever exist.

## Review Trigger

Review after the offline combat and progression loop is validated.

---

# TD-017: Do Not Add Assembly Definitions Initially

## Status

Accepted

## Decision

Keep runtime scripts in Unity's default assembly during the earliest prototype.

Add Assembly Definition files only when a clear benefit appears.

## Context

Assembly definitions improve modularity and compile times but add configuration overhead.

## Reasons

- The initial codebase is small.
- Early assembly boundaries may be arbitrary.
- Simplicity is more valuable during the first systems.

## Consequences

- Editor-only code must be placed under an `Editor` folder.
- Assembly boundaries will be reviewed as the codebase grows.
- Cyclic dependencies must still be avoided.

## Review Trigger

Review when:

- Compile times become disruptive
- Automated tests are added
- Editor tooling grows
- Modules gain stable boundaries

---

# TD-018: Do Not Use the Resources Folder by Default

## Status

Accepted

## Decision

Avoid placing new assets in `Assets/Resources` unless runtime string-based loading is specifically required and documented.

## Context

The folder currently exists in the planned project structure, but unrestricted use can cause dependency and memory-management problems.

## Reasons

- All Resources assets may be included in builds.
- String-based loading reduces reference safety.
- Unloading and ownership can become unclear.
- Addressables are more suitable for large content delivery later.

## Alternatives

- Inspector references
- Prefab references
- ScriptableObject references
- Addressables in a future phase

## Consequences

Any use of `Resources.Load` requires a documented reason.

## Review Trigger

Review when dynamic content loading becomes part of the roadmap.

---

# TD-019: Delay Addressables Until Content Scale Requires Them

## Status

Accepted

## Decision

Do not configure Addressables during the basic playable prototype.

## Context

Addressables are useful for large projects, remote content, memory control, and live updates, but add workflow complexity.

## Reasons

- The first milestone has a small number of assets.
- Direct references are easier to debug.
- Asset delivery requirements are not yet known.

## Consequences

- Avoid architecture that assumes all content is permanently loaded.
- Keep item and enemy definitions reference-based and migration-friendly.
- Introduce Addressables before large-scale world or live-content production.

## Review Trigger

Review when:

- Multiple maps contain significant content
- Downloadable content is planned
- Remote catalogs are required
- Memory management requires explicit loading

---

# TD-020: Use Git and Exclude Generated Unity Files

## Status

Accepted

## Decision

Version control the project source and configuration while excluding generated Unity artifacts.

## Include

- Assets
- Packages
- ProjectSettings
- Documentation
- Source files
- Meta files

## Exclude

- Library
- Logs
- Temp
- Obj
- Build output
- User-specific IDE data where appropriate

## Context

Unity regenerates several large folders locally.

## Reasons

- Keeps the repository small.
- Prevents machine-specific conflicts.
- Preserves Unity GUID relationships through `.meta` files.

## Consequences

- Every asset's `.meta` file must be committed.
- ProjectSettings and package manifests must be committed.
- Library must never be treated as source.

## Review Trigger

No expected review.

---

# TD-021: AI Assistants Must Follow Project Documentation

## Status

Accepted

## Decision

AI coding assistants must read and follow:

- `AGENTS.md`
- `README.md`
- `CONTRIBUTING.md`
- `TODO.md`
- `docs/ARCHITECTURE.md`
- `docs/SYSTEM_ARCHITECTURE.md`
- `docs/CODING_STANDARDS.md`
- `docs/TECHNICAL_DECISIONS.md`

before making significant changes.

## Required Workflow

1. Analyze the current implementation.
2. Explain the proposed solution.
3. List files to create or modify.
4. Identify risks or assumptions.
5. Wait for approval when required by `AGENTS.md`.
6. Apply the smallest coherent change.
7. Verify compilation and relevant behavior.
8. Update documentation when architecture changes.

## Reasons

- Reduces inconsistent code generation.
- Prevents unnecessary rewrites.
- Keeps AI output aligned with project goals.

## Consequences

- Documentation must remain accurate.
- AI-generated code receives the same review as human-written code.
- AI suggestions are not automatically considered correct.

## Review Trigger

Review when the team's AI-assisted workflow changes.

---

# TD-022: Documentation Must Match the Current Project State

## Status

Accepted

## Decision

Documentation represents intended architecture and current status, not merely aspirational features.

Future features must be marked clearly as planned.

## Reasons

- Prevents Codex and developers from assuming unimplemented systems exist.
- Reduces accidental dependencies on future plans.
- Makes onboarding more reliable.

## Consequences

When a feature is added:

- Update `TODO.md`
- Update `CHANGELOG.md`
- Update architecture documents when system boundaries change
- Record significant new technical decisions here

## Review Trigger

Review at every completed milestone.

---

# Decision Proposal Template

Use the following template for future decisions:

```md
# TD-XXX: Decision Title

## Status

Proposed

## Decision

Describe the selected approach.

## Context

Describe the problem and constraints.

## Reasons

- Reason one
- Reason two

## Alternatives Considered

### Alternative A

Explain why it was not selected.

## Consequences

Describe positive and negative consequences.

## Review Trigger

Describe when this decision should be reconsidered.
```

---

# Change Policy

Do not silently contradict an accepted technical decision.

When a decision must change:

1. Add a new decision.
2. Mark the previous decision as `Superseded`.
3. Link the previous and replacement decisions.
4. Update affected architecture and coding documents.
5. Record the change in `CHANGELOG.md`.

Technical decisions should evolve deliberately rather than accidentally.