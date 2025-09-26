
# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context
**Language/Version**: C# (.NET 8 WebAssembly)  
**Primary Dependencies**: [e.g., .NET WASM workload, SkiaSharp, WebAudio bridge or NEEDS CLARIFICATION]  
**Storage**: [e.g., RAM buffer, IndexedDB persistence, browser local storage or N/A]  
**Testing**: [e.g., xUnit, NUnit, Playwright WASM harness or NEEDS CLARIFICATION]  
**Target Platform**: Browser (WebAssembly)
**Project Type**: Single C# core with Web adapters  
**Performance Goals**: Sustain 60 fps render loop and deterministic 60 Hz timers (adjust if feature-specific)  
**Constraints**: Deterministic execution, bounded memory (<1 MB state), no alternate runtimes unless amended  
**Scale/Scope**: [domain-specific, e.g., ROM count, UI depth, tooling additions or NEEDS CLARIFICATION]

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I – Instruction-Accurate Emulation**: Confirm opcode semantics, memory layout, and ROM expectations remain faithful; document deviations.
- **Principle II – Single C# Core, WebAssembly Delivery**: Ensure all new functionality lives in the shared C# core with thin WebAssembly adapters; flag any alternative runtimes.
- **Principle III – Test-First Coverage**: Identify failing tests to author before implementation (opcode, timer, UI harness).
- **Principle IV – Deterministic Timing & Input**: Validate timing assumptions (60 Hz) and centralized input mapping; plan for replay tooling when relevant.
- **Principle V – Performance & Observability**: Capture expected frame rates, CPU budgets, and telemetry/metering updates.
- Record mitigation steps for any exception in Complexity Tracking before proceeding.

## Project Structure

### Documentation (this feature)
```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->
```
src/
├── Core/
│   ├── Cpu/
│   ├── Memory/
│   ├── Devices/
│   └── Timing/
├── Platform/
│   └── Web/
├── Rendering/
└── Audio/

tests/
├── Unit/
├── Integration/
└── Rom/

wasm/
└── wwwroot/
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract emulator concerns from feature spec** → `data-model.md`:
   - CPU/memory/register impacts
   - Display/audio behavior
   - State transitions and serialization needs

2. **Generate opcode/device contracts** from functional requirements:
   - For each opcode change → update instruction matrix under `/contracts/`
   - For device interactions (display, keypad, timers) → define expected inputs/outputs

3. **Generate regression tests** from contracts:
   - One test file per opcode/device change with expected register/memory/display assertions
   - Include ROM-based tests when behavior spans multiple opcodes
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration scenario (e.g., ROM playback, debugging tools)
   - Quickstart test = instructions to validate in browser/WebAssembly build

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/bash/update-agent-context.sh codex`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (instruction matrix, data model, quickstart)
- Each opcode/device contract → regression test task [P]
- Each subsystem (CPU, memory, rendering, audio, input) → implementation task tied to C# core files
- Each user story → integration/UI harness test task
- Implementation tasks to make tests pass while respecting TDD

**Ordering Strategy**:
- TDD order: Tests before implementation 
- Dependency order: Models before services before UI
- Mark [P] for parallel execution (independent files)

**Estimated Output**: 25-30 numbered, ordered tasks in tasks.md

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [ ] Phase 0: Research complete (/plan command)
- [ ] Phase 1: Design complete (/plan command)
- [ ] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed (tests + 60 fps + deterministic replay)

**Gate Status**:
- [ ] Initial Constitution Check: PASS
- [ ] Post-Design Constitution Check: PASS
- [ ] All NEEDS CLARIFICATION resolved
- [ ] Complexity deviations documented
- [ ] Performance & observability impacts recorded (Principle V)

---
*Based on Constitution v1.0.0 - See `/memory/constitution.md`*
