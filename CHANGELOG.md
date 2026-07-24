# Changelog - Hero of Eternia

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0] - 2026-07-24

### Added
*   **Project Foundation & Architectural Vision**: Selected **Godot 4.x (C#)** as the core engine.
*   **Folder Structure Blueprint**: Formulated directory trees mapping Assets, Prefabs, Scripts, and documentation.
*   **Decoupled Script Design**: Documented manager script classes (GameManager, SaveManager, InputManager, etc.) coordinated by an EventBus.
*   **Local Save Strategy**: Outlined SQLite save slot patterns with version check headers to support progress migrations.
*   **Scalable Graphics Quality Layout**: Formulated hardware presets (Low, Medium, High, Ultra) adjusting draw parameters.
*   **Security Strategy**: Outlined save validation signatures and obfuscation layers.
*   **Workspace Agent Bindings**: Configured [.agents/AGENTS.md](file:///c:/AAA/.agents/AGENTS.md) to permanently enforce AI-first asset production.
