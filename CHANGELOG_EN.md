# 2024/12/11

### Changes
- Optimization: Rewritten the TsBehaviour invocation logic to reduce the number of interactions between TsBehaviour instantiation and C# calls, and optimize the number of Delegate instances.
- Optimization: Customizable TsBehaviour lifecycle functions have been introduced to reduce the number of Component instances in the project.
- New Feature: Added TsBehaviour test cases.

# Migrating TsBehaviour from Previous Versions
> This version rewrites the C# code. Below is a partial migration guide, and so on:

- XOR.UpdateProxy → XOR.Behaviour.Default.UpdateBehaviour
- XOR.LateUpdateProxy → XOR.Behaviour.Default.LateUpdateBehaviour
- XOR.FixedUpdateProxy → XOR.Behaviour.Default.FixedUpdateBehaviour
- XOR.OnDisableProxy → XOR.Behaviour.Default.OnDisableBehaviour
- XOR.OnEnableProxy → XOR.Behaviour.Default.OnEnableBehaviour
- XOR.OnGUIProxy → XOR.Behaviour.Default.OnGUIBehaviour
- ...