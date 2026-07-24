# Coding Standards - Hero of Eternia

This document defines the project-wide coding conventions, directory guidelines, and version control standards for *Hero of Eternia*.

---

## 1. Naming Conventions
*   **Classes & Structs:** PascalCase (e.g., `PlayerController`, `SaveManager`).
*   **Interfaces:** PascalCase prefixed with 'I' (e.g., `ISaveable`, `IDamageable`).
*   **Methods:** PascalCase (e.g., `TakeDamage`, `SaveGame`).
*   **Variables (Private/Protected):** camelCase with a leading underscore (e.g., `_currentHealth`, `_saveSlot`).
*   **Variables (Public/Local):** camelCase (e.g., `targetSpeed`, `delta`).
*   **Constants & Enums:** PascalCase (e.g., `MaxHealthLimit`, `PresetType.Ultra`).

---

## 2. Directory and Namespace Rules
*   Every C# class must belong to a namespace matching its file path folder (e.g., class `PlayerController` in `Scripts/Entities/` resides in namespace `HeroOfEternia.Entities`).
*   File names must match the single public class declared within them.

---

## 3. Class Organization
Structure C# classes in the following order:
1.  Private backing fields / constants.
2.  Public properties.
3.  Godot lifecycle callbacks (`_Ready`, `_Process`, `_PhysicsProcess`).
4.  Public methods.
5.  Private helper methods.
6.  Event handler callbacks.

---

## 4. Comments and Documentation Standards
*   Use XML standard docstrings (`///`) on all public APIs, methods, and interface definitions.
*   Write clear, inline comments explaining *why* a complex algorithm is chosen, rather than *what* the syntax is doing.

---

## 5. Error Handling and Logging
*   **Defensive Checks:** Validate function parameters on entry (e.g., check for nulls, check indices range).
*   **Try-Catch Blocks:** Enclose file read/write (SQLite) operations, asset preloads, and system initialization steps.
*   **Logging:** Use Godot's built-in print commands wrapped in custom loggers (`Logger.Info`, `Logger.Warn`, `Logger.Error`) to allow log stripping in release builds.

---

## 6. Testing Standards
*   Write JUnit-equivalent C# unit tests using Godot's test framework (like GUT - Godot Unit Test, or standard NUnit/XUnit).
*   All mathematical utilities, database serializers, and state machines require 100% automated test coverage.

---

## 7. Version Control & Git Flow
*   **Branching Strategy:**
    *   `main`: Stable release branch (conforms to complete APK tags).
    *   `dev`: Active consolidation branch.
    *   `feature/p<num>-<name>`: Isolated feature branches for each phase (e.g., `feature/p2-init-godot`).
*   **Commit Message Conventions:**
    *   `feat: <description>` for new architectures/features.
    *   `fix: <description>` for bug repairs.
    *   `docs: <description>` for manual documentation edits.
