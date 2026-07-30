# AGENTS.md

# Simple25DRPG

## Project Overview

This project is a production-quality mobile-first 2.5D RPG built with Unity 6 URP.

Primary inspiration:

- Tree of Savior
- Ragnarok Online
- Diablo (camera style)

Current goal:

Create an offline playable prototype that can later evolve into a full MMORPG.

Target platform:

- Android
- ARM64
- Unity 6.5+
- Universal Render Pipeline (URP)

Language:

- C#

---

# AI Responsibilities

Before making changes:

- Analyze the existing project.
- Explain the proposed implementation.
- Wait for approval before modifying files.
- Keep changes as small as possible.
- Avoid unnecessary refactoring.

Never rewrite working systems unless requested.

---

# Development Philosophy

Priorities:

1. Readability
2. Maintainability
3. Extensibility
4. Mobile Performance

Code should be understandable by developers who are still learning Unity.

Avoid clever code.

Prefer explicit code over magic.

---

# Coding Standards

- One class per file.
- One responsibility per class.
- Follow SOLID.
- Prefer composition over inheritance.
- Avoid Singleton unless absolutely necessary.
- Avoid static state.
- Keep methods short.
- Use meaningful names.
- Use XML documentation comments for public APIs.
- Use SerializeField instead of public fields whenever possible.

---

# Folder Structure

Assets/

    Animations/

    Art/

    Audio/

    Materials/

    Models/

    Prefabs/

    Resources/

    Scenes/

    Scripts/

        Camera/

        Common/

        Enemy/

        Managers/

        Player/

        UI/

    UI/

Always place new files in the correct folder.

Do not create duplicate systems.

---

# Unity Rules

Use:

- Unity Input System

Do NOT use:

- Legacy Input Manager

Movement must use:

- CharacterController

Avoid:

- Rigidbody-based player movement unless explicitly requested.

---

# Input Architecture

Input must never directly control gameplay.

Preferred flow:

PlayerInput

↓

Movement

↓

Animation

↓

Combat

The movement system should receive input values rather than read keyboard state directly.

This allows future support for:

- Keyboard
- Android Virtual Joystick
- Gamepad
- Touch Controls

without changing gameplay code.

---

# Camera

Camera requirements:

- Perspective
- 2.5D angle
- Smooth follow
- Camera-relative movement
- Configurable offset

Future compatibility:

- Cinemachine
- Camera shake
- Zoom

---

# Player Architecture

Player/

PlayerInput.cs

PlayerMovementController.cs

PlayerAnimationController.cs

PlayerCombatController.cs

PlayerHealth.cs

Player.cs

Each class should have only one responsibility.

---

# Enemy Architecture

Enemy/

EnemyAI.cs

EnemyMovement.cs

EnemyAnimation.cs

EnemyHealth.cs

EnemyCombat.cs

Future AI should support:

- Idle
- Patrol
- Chase
- Attack
- Return Home

---

# Combat

Current version:

- One attack button
- Melee attack
- Cooldown
- Animation
- Hit Detection

Future:

- Combo
- Skills
- Magic
- Auto Attack

Design for extension.

---

# UI

Current UI:

- HP Bar
- Attack Button

Future UI:

- Inventory
- Equipment
- Skills
- Quest
- NPC Dialogue
- Chat

Keep UI independent from gameplay logic.

---

# Performance Rules

Target:

60 FPS on Android.

Avoid:

- GameObject.Find() inside Update()
- FindObjectOfType() inside gameplay loops
- Allocations every frame
- LINQ inside Update()

Cache references.

Reuse objects.

Prefer Object Pooling.

---

# ScriptableObjects

Use ScriptableObjects for:

- Items
- Skills
- Character Stats
- Enemy Data
- Equipment
- Configurations

Avoid hardcoded values.

---

# Inspector

Inspector should be clean.

Use:

- Header
- Tooltip
- Space

Expose only necessary fields.

---

# Comments

Explain WHY.

Avoid comments that only repeat code.

Good:

// Cache the camera transform once to avoid repeated lookups.

Bad:

// Move player.

---

# Naming

Classes:

PascalCase

Methods:

PascalCase

Private fields:

_serializedField

Local variables:

camelCase

Constants:

UPPER_CASE

---

# Mobile

Everything must support Android.

Avoid desktop-only APIs.

Design input abstraction from day one.

---

# Networking

Do NOT implement networking yet.

However:

Avoid tightly coupling systems.

Player logic should not depend on local input.

Future support:

- Dedicated Server
- Client Prediction
- Synchronization

---

# Assets

Never modify imported assets directly.

Create Prefabs.

Create Materials separately.

Avoid editing package files.

---

# Git

Small commits.

One feature per commit.

Examples:

feat(player): implement movement

feat(camera): smooth follow

fix(player): grounded detection

---

# AI Behaviour

Before writing code:

1. Explain the plan.

2. List files to create.

3. Explain architecture.

4. Wait for approval.

Only then modify files.

Never generate unnecessary complexity.

When in doubt:

Choose the simpler architecture.