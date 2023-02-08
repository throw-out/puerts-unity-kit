## 介绍
> 提供脚本打包功能包(含压缩丶加密丶验签等);  
> 建议此工具只在发布后使用, Editor环境下可使用[FileLoader](../projects/Assets/XOR/Runtime/Src/Loader.cs#L121), 请查看使用[示例](../projects/Assets/Samples/Starter.cs).

## 定义
> 继承 [XOR.ScriptPacker](../projects/Assets/XOR/Runtime/Src/ScriptPacker/ScriptPacker.cs) → 无

## 方法
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
| Scan | 用于扫描指定目录下的脚本文件, 并返回`Dictionary<相对路径, 脚本内容>` |
| ScanModule | 用于扫描指定目录下`node_modules模块`中的脚本文件, 并返回`Dictionary<相对路径, 脚本内容>` |
| Pack | 将`Dictionary<相对路径, 脚本内容>`打包为二进制数据, 此时可进行压缩丶加密丶加签等操作 |
| Unpack | Pack的逆操作, 按需进行解压丶解密丶验签等操作 |
</details>