---
name: Project-overview-with-ripgrep
---

# Project Overview With Ripgrep Commandline Tool
- Get a list of all C Sharp files in the project and there methods

# Project Architecture & Method Map

## Context Injection
The following command is used to generate a functional map of the project. When you see output formatted with line numbers and C# signatures, treat it as the **Active Project Map**.

```
rg -n -t cs '^\s*(public|protected|internal|private|static|async|class|interface|record).*\(.*\)' -g '!{bin,obj,node_modules,.git,.vs}/'
```

## Guidelines for Copilot
1. **Verify Before Coding**: Before suggesting a new method or a cross-file call, refer to the Project Map to see if a similar signature already exists.
2. **Namespace Awareness**: Pay attention to the file paths in the `rg` output to ensure you are suggesting logic in the correct architectural layer (e.g., Core vs. UI).
3. **Signature Adherence**: If a method is marked `async`, ensure all suggested calls use `await`.

## Output Constraints
- Do not suggest creating duplicate logic if a signature in the Map already performs that task.
- If a signature includes DocBlocks (///), use those descriptions to inform your logic.
