import * as csharp from "csharp";
import { $typeof } from "puerts";

class TsComponentImpl extends xor.TsBehaviour {

}

function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.TsComponent = TsComponentImpl;
}
register();