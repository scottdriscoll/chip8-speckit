# Feature Specification: [FEATURE NAME]

**Feature Branch**: `[###-feature-name]`  
**Created**: [DATE]  
**Status**: Draft  
**Input**: User description: "$ARGUMENTS"

## Execution Flow (main)
```
1. Parse user description from Input
   → If empty: ERROR "No feature description provided"
2. Extract key concepts from description
   → Identify: actors, actions, data, constraints
3. For each unclear aspect:
   → Mark with [NEEDS CLARIFICATION: specific question]
4. Fill User Scenarios & Testing section
   → If no clear user flow: ERROR "Cannot determine user scenarios"
5. Generate Functional Requirements
   → Each requirement must be testable
   → Mark ambiguous requirements
6. Identify Key Entities (if data involved)
7. Run Review Checklist
   → If any [NEEDS CLARIFICATION]: WARN "Spec has uncertainties"
   → If implementation details found: ERROR "Remove tech details"
8. Return: SUCCESS (spec ready for planning)
```

---

## ⚡ Quick Guidelines
- ✅ Focus on WHAT users need and WHY
- ❌ Avoid HOW to implement (no tech stack, APIs, code structure)
- 👥 Written for business stakeholders, not developers
- 🎯 Capture emulator expectations: ROM types, timing accuracy, input/audio experience

### Section Requirements
- **Mandatory sections**: Must be completed for every feature
- **Optional sections**: Include only when relevant to the feature
- When a section doesn't apply, remove it entirely (don't leave as "N/A")

### For AI Generation
When creating this spec from a user prompt:
1. **Mark all ambiguities**: Use [NEEDS CLARIFICATION: specific question] for any assumption you'd need to make
2. **Don't guess**: If the prompt doesn't specify something (e.g., "login system" without auth method), mark it
3. **Think like a tester**: Every vague requirement should fail the "testable and unambiguous" checklist item
4. **Common underspecified areas**:
   - Supported ROM formats and known test suites
   - Display resolution, color treatment, and scaling behavior  
   - Input mapping (keyboard, touch, gamepad) and remapping rules
   - Audio expectations (on/off toggle, latency tolerance)
   - Performance targets (60 fps, 60 Hz timers) and fallback plans
   - Save-state/storage requirements (local storage, cloud sync)

---

## User Scenarios & Testing *(mandatory)*

### Primary User Story
[Describe the main user journey in plain language]

### Acceptance Scenarios
1. **Given** [initial state], **When** [action], **Then** [expected outcome]
2. **Given** [initial state], **When** [action], **Then** [expected outcome]

### Edge Cases
- What happens when [boundary condition]?
- How does system handle [error scenario]?

## Requirements *(mandatory)*

### Functional Requirements
- **FR-001**: Emulator MUST load CHIP-8 ROMs from local upload and curated library sources.
- **FR-002**: Emulator MUST render the 64×32 display (and Super-CHIP variants when relevant) with configurable scaling in-browser.
- **FR-003**: Emulator MUST enforce 60 Hz timers and deterministic execution across runs given identical input sequences.
- **FR-004**: Users MUST be able to configure keypad/input mappings for keyboard, touch, or gamepad.
- **FR-005**: Emulator MUST surface performance and observability data (frame time, opcode throughput, audio status) per Principle V.

*Example of marking unclear requirements:*
- **FR-006**: Emulator MUST generate audio using [NEEDS CLARIFICATION: WebAudio beep behavior not specified - duration, frequency?]
- **FR-007**: Emulator MUST persist save-states to [NEEDS CLARIFICATION: storage target not specified - IndexedDB, file download?]

### Key Entities *(include if feature involves data)*
- **EmulatorState**: Registers (V0–VF), I register, program counter, stack, delay/sound timers.
- **DisplayBuffer**: 64×32 (or Super-CHIP resolution) pixel state, scaling attributes, collision flags.

---

## Review & Acceptance Checklist
*GATE: Automated checks run during main() execution*

### Content Quality
- [ ] No implementation details (languages, frameworks, APIs)
- [ ] Focused on user value and business needs
- [ ] Written for non-technical stakeholders
- [ ] All mandatory sections completed

### Requirement Completeness
- [ ] No [NEEDS CLARIFICATION] markers remain
- [ ] Requirements are testable and unambiguous  
- [ ] Success criteria are measurable
- [ ] Scope is clearly bounded
- [ ] Dependencies and assumptions identified

---

## Execution Status
*Updated by main() during processing*

- [ ] User description parsed
- [ ] Key concepts extracted
- [ ] Ambiguities marked
- [ ] User scenarios defined
- [ ] Requirements generated
- [ ] Entities identified
- [ ] Review checklist passed

---
