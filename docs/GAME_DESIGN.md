# Game Design Document

# Simple25DRPG

Version 0.1

---

# Overview

Simple25DRPG is a mobile-first 2.5D action RPG developed with Unity 6 URP.

The game draws inspiration from:

- Tree of Savior
- Ragnarok Online
- Diablo (camera perspective)

The objective is to build an expandable architecture that starts as an offline RPG and can evolve into a multiplayer MMORPG.

---

# Design Philosophy

The game should be:

- Easy to learn
- Responsive
- Lightweight
- Mobile friendly
- Easy to expand

Gameplay simplicity comes first.

---

# Core Gameplay Loop

Explore

↓

Fight Monsters

↓

Collect Loot

↓

Gain Experience

↓

Level Up

↓

Upgrade Equipment

↓

Unlock New Areas

Repeat

---

# Camera

Perspective Camera

Fixed Angle

Camera Follow Player

Smooth Movement

Future:

- Zoom
- Camera Shake
- Cinematic Camera

---

# Controls

Mobile

- Virtual Joystick
- Attack Button
- Skill Buttons (future)

Desktop (Development Only)

- WASD
- Mouse

Gameplay must never depend directly on keyboard input.

---

# Player

Current

- Movement
- Rotation
- Basic Attack
- HP

Future

- Skills
- Combo
- Equipment
- Buff
- Debuff
- Status Effects

---

# Enemy

Current

- Idle
- Chase
- Attack
- Die

Future

- Patrol
- Elite
- Boss
- Ranged
- Magic

---

# Combat

Version 1

- Single Attack
- Melee Damage
- Cooldown

Future

- Combo
- Critical Hit
- Skills
- AoE
- Knockback

---

# Character Progression

Player has

- Level
- Experience
- HP
- Attack
- Defense

Future

- STR
- AGI
- INT
- DEX
- VIT

---

# Inventory

Future Features

- Equipment
- Consumables
- Quest Items
- Materials

---

# Equipment

Weapon

Armor

Helmet

Boots

Accessory

Future

- Set Bonus
- Upgrade
- Enchant

---

# NPC

Future

- Shop
- Quest
- Storage
- Teleport

---

# Quest

Future

Main Quest

Side Quest

Daily Quest

---

# World

Future

Town

Dungeon

Field

Boss Arena

---

# Multiplayer

Planned

Party

Guild

Trade

Chat

PvE

PvP

---

# Save System

Prototype

Local Save

Future

Cloud Save

Server Save

---

# Long-Term Goal

Build a maintainable mobile MMORPG without rewriting core gameplay systems.