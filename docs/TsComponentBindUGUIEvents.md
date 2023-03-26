## 支持的参数类型
- string
- number
- boolean
- UnityEngine.Object及其子类型

## 使用说明
以**UnityEngine.UI.Button** 为例:

1. 转为Wrapper组件  
![image](https://user-images.githubusercontent.com/45587825/227753184-a38abb02-8aa3-4c12-8b85-f269794b4d1c.png)
2. 添加一个Wrapper事件  
![image](https://user-images.githubusercontent.com/45587825/227753165-adbce705-7a5f-4302-be90-fce1934e2850.png)
3. 启动Services服务, 然后选择TsComponent组件上的一个方法  
![image](https://user-images.githubusercontent.com/45587825/227753269-bf4222fc-4e08-4385-b50e-973b4b173d1d.png)

## 演示
> 示例场景:[projects/Assets/Samples/06_UGUIEvents](../projects/Assets/Samples/06_UGUIEvents/)  
> 示例typescript代码: [projects/TsProject/src/samples/06_UGUIEvents.ts](../projects/TsProject/src/samples/06_UGUIEvents.ts)  

- 事件绑定  
![image](https://user-images.githubusercontent.com/45587825/227753354-fcaab716-9c26-4ad3-a20d-0fb3215f612f.png)
- 依次触发事件打印如下  
![image](https://user-images.githubusercontent.com/45587825/227753412-976d800a-3789-4039-8a63-c5335f5bf63e.png)