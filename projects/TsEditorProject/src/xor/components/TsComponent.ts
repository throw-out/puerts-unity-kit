import * as csharp from "csharp";
import { $typeof } from "puerts";

class TsComponentImpl extends XOR.TsBehaviour {

}

function register() {
    let _g = (global ?? globalThis ?? this);
    _g.XOR = _g.XOR || {};
    _g.XOR["TsComponent"] = TsComponentImpl;
}
register();