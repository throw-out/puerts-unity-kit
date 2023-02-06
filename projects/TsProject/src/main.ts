import * as csharp from "csharp";

import "./lib/modules";

if (csharp.UnityEngine.Application.isEditor) {
    require("puerts/console-track");
    require("puerts/puerts-source-map-support");
}