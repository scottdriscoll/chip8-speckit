# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Execution Flow (main)
```
1. Load plan.md from feature directory
   → If not found: ERROR "No implementation plan found"
   → Extract: subsystems touched (CPU, memory, rendering, audio, platform)
2. Load optional design documents:
   → data-model.md: Extract state/register changes → model tasks
   → contracts/: Each opcode/device entry → regression test task
   → research.md: Extract performance/observability decisions → setup tasks
3. Generate tasks by category:
   → Setup: project init, .NET WASM config, tooling
   → Tests: opcode/unit, ROM conformance, Web UI harness
   → Core: CPU, memory, timers, display, audio updates
   → Integration: WebAssembly plumbing, input, instrumentation
   → Polish: performance validation, docs, manual QA
4. Apply task rules:
   → Different files = mark [P] for parallel
   → Same file = sequential (no [P])
   → Tests before implementation (TDD, Principle III)
5. Number tasks sequentially (T001, T002...)
6. Generate dependency graph
7. Create parallel execution examples
8. Validate task completeness:
   → All contracts have regression tests?
   → All subsystems touched have implementation coverage?
   → Performance/observability updates captured?
9. Return: SUCCESS (tasks ready for execution)
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions
- C# core lives under `src/Core/`
- Platform/WebAssembly adapters under `src/Platform/Web/`
- Rendering/audio helpers under `src/Rendering/` and `src/Audio/`
- Tests under `tests/Unit/`, `tests/Integration/`, `tests/Rom/`

## Phase 3.1: Setup
- [ ] T001 Ensure `.csproj` and solution references include new core/module paths per plan
- [ ] T002 Configure `.dotnet-wasm` build or equivalent workflow for browser preview
- [ ] T003 [P] Update `wasm/wwwroot/index.html` scaffolding (CSS/JS hooks) if required by feature

## Phase 3.2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE 3.3
**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**
- [ ] T004 [P] Opcode regression test in `tests/Unit/Opcodes/Test_[OPCODE].cs`
- [ ] T005 [P] ROM conformance scenario in `tests/Rom/Test_[ROM_NAME].cs`
- [ ] T006 [P] Integration test covering browser flow in `tests/Integration/Test_[Feature].cs`
- [ ] T007 Instrumentation test ensuring telemetry expectations in `tests/Integration/Test_Instrumentation.cs`

## Phase 3.3: Core Implementation (ONLY after tests are failing)
- [ ] T008 Implement opcode logic in `src/Core/Cpu/Opcodes/[Opcode].cs`
- [ ] T009 Update memory/timer handling in `src/Core/Timing/TimerService.cs`
- [ ] T010 Refresh display pipeline in `src/Rendering/DisplayRenderer.cs`
- [ ] T011 Extend input mapper in `src/Core/Input/InputMapper.cs`
- [ ] T012 Wire telemetry counters in `src/Core/Diagnostics/FrameMetrics.cs`
- [ ] T013 Update save-state persistence in `src/Core/Persistence/SaveStateStore.cs`

## Phase 3.4: Integration
- [ ] T014 Connect WebAssembly host bindings in `src/Platform/Web/Interop/RuntimeHost.cs`
- [ ] T015 Hook audio beeper into WebAudio bridge in `src/Audio/WebAudioBeeper.cs`
- [ ] T016 Ensure async ROM loading & error surfaces in `src/Platform/Web/Services/RomLoader.cs`
- [ ] T017 Update browser UI bindings in `wasm/wwwroot/js/app.js`

## Phase 3.5: Polish
- [ ] T018 [P] Performance validation script (60 fps, CPU <50%) in `tests/Integration/Performance/Test_FrameBudget.cs`
- [ ] T019 [P] Documentation update in `docs/feature-[###-feature-name].md` (include observability notes)
- [ ] T020 Telemetry dashboard widgets in `wasm/wwwroot/js/telemetry.js`
- [ ] T021 Manual QA checklist run recorded in `docs/manual-testing.md`
- [ ] T022 Cleanup dead code & ensure analyzers pass

## Dependencies
- Tests (T004-T007) MUST precede implementation (T008-T013)
- Implementation tasks block integration (T014-T017)
- Integration proves before polish (T018-T022)
- Telemetry updates (T012, T017, T020) coordinate to avoid conflicts

## Parallel Example
```
# Launch regression tests together once plan.md ready:
Task: "Opcode regression test in tests/Unit/Opcodes/Test_[OPCODE].cs"
Task: "ROM conformance scenario in tests/Rom/Test_[ROM_NAME].cs"
Task: "Integration test covering browser flow in tests/Integration/Test_[Feature].cs"
Task: "Instrumentation test ensuring telemetry expectations in tests/Integration/Test_Instrumentation.cs"
```

## Notes
- [P] tasks = different files, no dependencies
- Verify tests fail before implementing
- Record performance metrics (fps, CPU, timer drift) when completing T018
- Commit after each task with reference to TID

## Task Generation Rules
*Applied during main() execution*

1. **From Contracts**:
   - Each opcode/device contract → regression test task [P]
   - Each display/audio/input change → implementation task
   
2. **From Data Model / Emulator State**:
   - Each register/state mutation → core implementation + unit test tasks
   - New persistence or tooling requirements → supporting tasks (e.g., save-state, instrumentation)
   
3. **From User Stories**:
   - Each player scenario → integration test [P]
   - Quickstart scenarios → manual QA tasks

4. **Ordering**:
   - Setup → Tests → Core → Integration → Polish
   - Deterministic execution and performance validation captured before completion

## Validation Checklist
*GATE: Checked by main() before returning*

- [ ] Every opcode/device change has a failing regression test
- [ ] All integration stories covered by tests
- [ ] Telemetry/performance tasks added when Principle V impacted
- [ ] Parallel tasks modify independent files
- [ ] Each task specifies exact file path
- [ ] Manual QA/polish steps present for player validation
