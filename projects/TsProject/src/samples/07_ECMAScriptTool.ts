import { System } from "csharp";
import { Application, SystemInfo } from "csharp.UnityEngine";
import { $generic } from "puerts";

console.log(Application.dataPath);
console.log(SystemInfo.deviceUniqueIdentifier);

let List_String = $generic(System.Collections.Generic.List$1, System.String) as {
    new(): System.Collections.Generic.List$1<string>
};
let list = new List_String();
list.Add("key1");
list.Add("key2");
list.Add("key3");
for (let i = 0; i < list.Count; i++) {
    console.log(list.get_Item(i));
}