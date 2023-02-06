> - xor d.ts声明目录: [projects/Assets/XOR/Typing](../projects/Assets/XOR/Typing)
> - ts示例目录: [projects/TsProject/src/samples](../projects/TsProject/src/samples)

## 已知缺陷
> 当触发AppDomain Unload时(`通常出现在Editor C#编译或进入Play模式`), 此时如果TheradWorker正在运行计算中, 将导致Crash.

> 解决方案如下:
> - 在`Player Settings` - `Other Settings` - `Scripting Define Symbols`中添加`THREAD_SAFE`定义, 因只出现在Editor环境下才会出现, 在打包时删除可避免宏带来的性能损失:

## 定义
[XOR.ThreadWorker](../projects/Assets/XOR/Runtime/Src/Thread/ThreadWorker.cs)

## 成员
| 名称  | 描述  |
| ------------ | ------------ |

## 方法
| 名称  | 描述  |
| ------------ | ------------ |

## 示例演示
TODO