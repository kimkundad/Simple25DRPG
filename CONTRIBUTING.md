# Contributing Guide

Thank you for contributing to Simple25DRPG.

This project follows a clean architecture and mobile-first design.

Please follow these guidelines before making changes.

---

# Workflow

1. Create a new feature branch.

2. Keep commits small.

3. One feature per pull request.

4. Explain architectural decisions.

5. Test inside Unity before committing.

---

# Coding Standards

- Follow SOLID principles.
- One class per file.
- One responsibility per class.
- Prefer composition over inheritance.
- Avoid Singleton unless necessary.
- Use SerializeField instead of public fields.
- Use XML documentation for public APIs.
- Keep Update() lightweight.
- Avoid runtime allocations.

---

# Folder Rules

Place files in the correct folder.

Example

Assets/
    Scripts/
        Player/
        Enemy/
        Camera/
        UI/
        Managers/
        Common/

Do not create duplicate systems.

---

# Unity Rules

Use

- Unity Input System
- CharacterController
- URP

Avoid

- Legacy Input
- Rigidbody movement for player
- FindObjectOfType in gameplay
- GameObject.Find inside Update

---

# AI Rules

Before modifying code

1. Explain the implementation.
2. List files that will be modified.
3. Wait for approval.
4. Then apply changes.

---

# Commit Convention

Examples

feat(player): add movement

feat(enemy): basic AI

feat(camera): smooth follow

fix(player): grounded detection

refactor(input): split input reader

docs: update README

---

# Code Review Checklist

Before committing

- Project compiles
- No Console errors
- Mobile compatible
- Inspector is clean
- XML comments added
- Performance considered