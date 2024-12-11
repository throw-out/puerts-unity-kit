# 2024/12/11

### 变更

- 优化: 重写TsBehaviour调用逻辑, 减少TsBehaviour实例化时与C#的交互次数, 并优化Delegate实例数量
- 优化: 可自定义组合TsBehaviour生命周期函数, 用于减少项目中Component组件实例数量
- 新增: 添加TsBehaviour测试用例


### 从旧版本迁移TsBehaviour

> 本次重写了C#代码, 以下为迁移指南(部分), 以此类推:

- XOR.UpdateProxy -> XOR.Behaviour.Default.UpdateBehaviour
- XOR.LateUpdateProxy -> XOR.Behaviour.Default.LateUpdateBehaviour
- XOR.FixedUpdateProxy -> XOR.Behaviour.Default.FixedUpdateBehaviour
- XOR.OnDisableProxy -> XOR.Behaviour.Default.OnDisableBehaviour
- XOR.OnEnableProxy -> XOR.Behaviour.Default.OnEnableBehaviour
- XOR.OnGUIProxy -> XOR.Behaviour.Default.OnGUIBehaviour
- ......