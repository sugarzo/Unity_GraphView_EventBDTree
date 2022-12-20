# Unity_GraphView_EventBDTree

本框架使用了GraphView和UI Toolkit，将以前的事件系统包装了进来，变成了可视化节点操作，方便长期项目中策划同学也能参与进来编写游戏逻辑。该框架比较轻量级，这篇文档用来介绍框架如何使用，前半部分是程序策划篇，只介绍如何使用比较简单，不需要代码知识也能理解。后半部分针对程序讲解如何拓展脚本和新节点。

节点图效果展示样例：

![image](https://user-images.githubusercontent.com/74815734/208720653-3ed63df7-f126-44e5-ae75-46601ffdd39c.png)

# 如何搭建逻辑
在Unity的Hierarchy窗口中，右键新建物品菜单中，可以看到一个FlowChart的选项，点击新建，场景便会多一个FlowChart游戏物品。
![image](https://user-images.githubusercontent.com/74815734/208720792-984bff88-4a86-4b3c-8bda-6125fbad0806.png)
![image](https://user-images.githubusercontent.com/74815734/208720811-d99cc1fe-6514-4af6-93c4-6cf83eaea162.png)
