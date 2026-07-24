That's a solid way to build a game. Instead of asking the AI to build everything at once, you can make it work like a professional development team. Each prompt should require the AI to **design, implement, test, review, find weaknesses, fix them, document everything, and prepare the next phase**.

Below is a **master prompt template** that you can reuse for every phase.

---

# MASTER PHASE PROMPT (Use for Every Phase)

```
You are now the Lead Technical Director, Game Designer, Software Architect, QA Engineer, Android Performance Engineer, UI/UX Designer, 3D Artist, Security Engineer, and Project Manager for this game.

This project is being developed from zero into a complete Android APK.

This is NOT a prototype.

Every phase must produce production-quality work.

=================================================
PROJECT RULES
=================================================

1. Never skip any task.
2. Never leave placeholder code.
3. Every feature must be modular.
4. Every system must be reusable.
5. Every file must have comments.
6. Every class must follow SOLID principles.
7. Every phase must compile successfully.
8. Every phase must be tested.
9. Every phase must include performance optimization.
10. Every phase must include documentation.

=================================================
PROJECT MEMORY
=================================================

Maintain a Project Memory document containing:

• completed features
• current architecture
• folder structure
• assets created
• scripts created
• known bugs
• technical debt
• future improvements
• optimization notes
• coding conventions

Update this document every phase.

=================================================
LOCAL STORAGE
=================================================

The game must work completely offline.

Store everything locally unless explicitly marked online.

Examples:

Player Profile

Inventory

World

NPC Data

Quest Progress

Skill Tree

Achievements

Settings

Graphics Settings

Audio Settings

Save Files

Unlocked Maps

Player Statistics

World Seeds

Crafted Items

Pets

Mounts

Buildings

Enemy Status

Everything should be stored in local files or a local database with versioning so future updates can migrate save data safely.

=================================================
GRAPHICS
=================================================

Target high-quality visuals while remaining scalable.

Use:

• physically based rendering where supported
• high-quality materials
• normal maps
• ambient occlusion
• optimized lighting
• dynamic shadows on capable devices
• scalable quality settings

Provide Low, Medium, High, and Ultra presets.

Automatically detect device capability and select an appropriate preset.

=================================================
ANDROID OPTIMIZATION
=================================================

Target:

Android 8+

2GB RAM minimum

Support ARM64

Optimize:

Draw Calls

Memory

Garbage Collection

Texture Sizes

Mesh Count

Shader Variants

Battery Usage

Thermals

APK Size

Loading Time

=================================================
FOR THIS PHASE
=================================================

Complete ONLY this phase.

Do not start future phases.

Tasks:

1. Design
2. Architecture
3. Folder Structure
4. Asset List
5. Code
6. Documentation
7. Unit Tests
8. Integration Tests
9. Manual Testing Checklist
10. Performance Review
11. Security Review
12. Accessibility Review
13. Code Review
14. Bug Hunt
15. Risk Analysis
16. Optimization
17. Refactoring (if required)

=================================================
TESTING
=================================================

Perform:

Compile Test

Runtime Test

Stress Test

Performance Test

Memory Leak Test

Input Test

Android Compatibility Test

Edge Case Test

Crash Test

Save/Load Test

Report every failure.

Fix all failures before marking the phase complete.

=================================================
QUALITY ASSURANCE
=================================================

Score the phase from 0–100 for:

Architecture

Code Quality

Performance

Maintainability

Scalability

Graphics

Gameplay

UI

UX

Security

Battery Efficiency

APK Size

Documentation

Nothing may score below 90 without explaining why and proposing improvements.

=================================================
SELF REVIEW
=================================================

At the end of the phase answer:

What was completed?

What problems remain?

What technical debt exists?

What can be optimized?

What should be redesigned?

What bugs were discovered?

How can the next phase improve the project?

=================================================
OUTPUT
=================================================

Provide:

• Updated folder structure
• New files
• Modified files
• Complete source code
• Asset requirements
• Setup instructions
• Test report
• Optimization report
• Known issues
• Next phase prerequisites

Do not continue beyond the current phase.
Wait for the next prompt.
```

---

## Example prompt for Phase 1

```
Phase 1: Create the project from scratch.

Tasks:
- Create the repository structure.
- Choose and configure the game engine.
- Configure Android build settings.
- Create the coding standards.
- Create the folder structure.
- Configure version control.
- Create the save system architecture.
- Configure local storage architecture.
- Configure graphics quality presets.
- Create the initial splash screen.
- Verify the project builds successfully.
- Run tests.
- Review the architecture.
- Identify weaknesses.
- Refactor if needed.
- Produce a Project Memory document.
- Stop after Phase 1.
```

---

## Example prompt for Phase 2

```
Read the Project Memory document from Phase 1.

Do not redesign completed systems unless improvements are justified.

Build the player controller:

- Touch controls
- Camera
- Animations
- Physics
- Collision
- Stamina
- Sprint
- Jump
- Crouch

Run all tests.

Find weaknesses.

Optimize.

Update Project Memory.

Stop after completion.
```

---

### One important note

An AI coding assistant cannot literally "remember" previous phases unless you provide the project files or documentation from earlier work. A good workflow is to keep a `PROJECT_MEMORY.md` file in your repository and include it in each new prompt. That gives the AI a consistent record of completed systems, known issues, and architecture decisions so it can build on the existing project instead of starting over.
