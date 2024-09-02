if (CS.UnityEngine.Application.isEditor) {
    //这里调用了Assets/Samples/Editor/ConsoleRedirect模块, 删除ConsoleRedirect模块后需要一并删除此代码块
    //The code calls the ConsoleRedirect module. If you remove the ConsoleRedirect module, you also need to delete the corresponding code block.
    require("puerts/console-track");
    require("puerts/puerts-source-map-support");
}