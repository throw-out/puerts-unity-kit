## 介绍
> 提供跨线程&跨Puerts.JsEnv实例交互(基于C#类型传递数据);

## 已知缺陷
> 在C#脚本编译或进入Play模式时将会进行AppDomain Unload操作, 此时AppDomain将强制**跨线程**调用JsEnv.Dispose从而导致Unity Crash!
>
> 解决方案如下:
> - 在`Player Settings` - `Other Settings` - `Scripting Define Symbols`中添加`THREAD_SAFE`定义;
>
>ThreadWorker本身是线程安全的, 目前只有AppDomain Unload的情况是例外, 它只会在Editor环境下出现.

## 定义
> 继承: [XOR.ThreadWorker](../projects/Assets/XOR/Runtime/Src/Thread/ThreadWorker.cs) → 无

## 成员
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
</details>

## 方法
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
</details>

## 示例演示
TODO
