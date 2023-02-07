> - xor d.ts声明目录: [projects/Assets/XOR/Typing](../projects/Assets/XOR/Typing)
> - ts示例目录: [projects/TsProject/src/samples](../projects/TsProject/src/samples)

## 已知缺陷
> 在C#脚本编译或进入Play模式时将会进行AppDomain Unload操作, 此时AppDomain将强制**跨线程**调用JsEnv.Dispose从而导致Unity Crash!

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