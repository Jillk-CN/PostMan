## Proposal: Update config.yaml with Current Project Context

### What

Populate `openspec/config.yaml` with accurate project context reflecting the current state of the PostMan Unity game — its tech stack, implemented systems, architectural patterns, and coding conventions.

### Why

The config file is currently empty (no `context:` or `rules:` defined). Without this context, every future `openspec` artifact generation starts from scratch with no knowledge of the project's architecture, conventions, or domain. Filling it in ensures all future proposals, designs, and task lists are grounded in the actual codebase.

### Non-goals

- Changing any game code
- Adding new systems or features
- Modifying existing openspec change artifacts

### Approach

Audit the current `Assets/Scripts/` directory, read key source files, and write a comprehensive `context:` block into `config.yaml` covering:
- Engine and package versions
- Implemented systems and their locations
- Architectural patterns in use
- Coding conventions (language, naming, comments)
- Domain knowledge (game genre, player mechanics)
