*The current page is translated by a machine, information may be missing or inaccurate. If you encounter any problems or have questions, please submit an ISSUE.*

## Introduction
> Provides a script packaging feature package (including compression, encryption, signature verification, etc.);  
> It is recommended to use this tool only after publishing. In the Editor environment, you can use [FileLoader](../../projects/Assets/XOR/Runtime/Src/Loader.cs). Please refer to the [example](../../projects/Assets/Samples/Starter.cs) for usage.

## Definition
> [`C#`] Inherits [XOR.ScriptPacker](../../projects/Assets/XOR/Runtime/Src/ScriptPacker/ScriptPacker.cs) → None  
<details>
<summary>Interface Details</summary>

| Method  | Description  |
| ------------ | ------------ |
| `static Dictionary<string, string> Scan(string, string[])` | Scans all JS files in the specified directory and returns a `Dictionary<relative path, script content>` |
| `static Dictionary<string, string> ScanModule(string, string[], string[])` | Used to scan script files in the specified directory's `node_modules module` and returns a `Dictionary<relative path, script content>` |
| `static byte[] Pack(Dictionary<string, string>, params object[])` | Packs the `Dictionary<relative path, script content>` into binary data, allowing for compression, encryption, and signing operations |
| `static Dictionary<string, string> Unpack(byte[], params object[])` | The inverse operation of Pack, performs decompression, decryption, signature verification, etc., as needed |
</details>
