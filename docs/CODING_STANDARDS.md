# Coding Standards

## General

- Follow SOLID.
- Keep code readable.
- One responsibility per class.
- One class per file.
- Prefer composition over inheritance.

---

# Naming

Classes

PascalCase

Methods

PascalCase

Properties

PascalCase

Private Fields

_serializedField

Local Variables

camelCase

Constants

UPPER_CASE

---

# Inspector

Use

[SerializeField]

instead of

public

Add

Header

Tooltip

Space

where appropriate.

---

# Unity

Use

- Unity Input System
- CharacterController
- ScriptableObject
- URP

Avoid

- Legacy Input
- GameObject.Find()
- FindObjectOfType()
- Runtime allocations
- LINQ inside Update()

---

# Performance

Target

60 FPS

Cache components.

Reuse objects.

Use Object Pooling.

Avoid GC spikes.

---

# Architecture

Player

Input

↓

Movement

↓

Animation

↓

Combat

Enemy

AI

↓

Movement

↓

Combat

↓

Animation

UI communicates with gameplay through events or interfaces whenever practical.

---

# Script Rules

Maximum responsibilities:

One.

Methods should remain short.

Avoid giant classes.

Prefer multiple focused components.

---

# Comments

Explain WHY.

Avoid obvious comments.

Example

Good

Cache camera transform once to avoid repeated lookups.

Bad

Move player.

---

# ScriptableObjects

Use for

- Items
- Skills
- Enemy Data
- Character Data
- Configurations

Avoid hardcoded values.

---

# AI Rules

Before writing code

1. Explain implementation.
2. Show architecture.
3. List files.
4. Wait for approval.

Never rewrite working systems without permission.

Always preserve backward compatibility where possible.

---

# Testing

Every feature should

Compile successfully.

Produce no Console errors.

Be configurable in Inspector.

Support Android.

Follow project architecture.