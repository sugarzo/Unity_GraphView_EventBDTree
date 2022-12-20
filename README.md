# Unity_GraphView_EventBDTree

本框架使用了GraphView和UI Toolkit，实现了类似行为树插件的可视化节点操作，方便长期项目中策划同学也能参与进来编写游戏逻辑。该框架比较轻量级，这篇文档用来介绍框架如何使用，前半部分是程序策划篇，只介绍如何使用比较简单，不需要代码知识也能理解。后半部分针对程序讲解如何拓展脚本和新节点。（分支已内置Odin和DOTween插件）

节点图效果展示样例：

![image](https://user-images.githubusercontent.com/74815734/208720653-3ed63df7-f126-44e5-ae75-46601ffdd39c.png)

## 如何搭建逻辑
在Unity的Hierarchy窗口中，右键新建物品菜单中，可以看到一个FlowChart的选项，点击新建，场景便会多一个FlowChart游戏物品。
![image](https://user-images.githubusercontent.com/74815734/208720792-984bff88-4a86-4b3c-8bda-6125fbad0806.png)
![image](https://user-images.githubusercontent.com/74815734/208720811-d99cc1fe-6514-4af6-93c4-6cf83eaea162.png)

选中FlowChart，点击上面挂载的脚本“Open”，就可以打开节点面板了！随后的后续编辑也只需要通过这个按钮进入即可。

Note文本框是用来写注释的，不影响实际运行效果。

点击Open后会打开一个面板，目前上面什么都没有。编辑面板分为两部分：左边的Inspector面板用于显示和编辑点击节点的属性，右边的图用来显示节点（目前是空的）

![image](https://user-images.githubusercontent.com/74815734/208721139-352fa7b8-e229-4339-9836-9eb52701e699.png)

在右边的节点图区域右键，点击Create Node，可以看到可以创建的节点列表：

![image](https://user-images.githubusercontent.com/74815734/208721167-ccf1747b-787d-4756-b228-b1dfc58c43de.png)
![image](https://user-images.githubusercontent.com/74815734/208721179-4c759be5-b797-4681-8db0-44189bd19a2a.png)

节点主要分为四大类：触发器节点、行为（事件）节点、分支节点、序列节点。
其中，触发器和行为节点是主要的节点：触发器节点决定什么时候触发这个事件（例如玩家按下某个按键，游戏中某个状态改变到指定值，游戏内某个事件发生都可以算是一个触发时机），行为节点决定了进入该节点后程序需要执行什么逻辑。具体主要节点的功能见文档末尾的节点附录。
例如，我想执行一段逻辑：当进入游戏场景的时候开始播放游戏音乐BGM，这时候已经知道程序已经提前写好了对应播放音乐的Action节点和Trigger节点。
新建一个StatusTrigger和AudioAction，在创建节点列表中找到对应节点，点击对应节点可以在左边Inspector面板中设置对应属性值，连接逻辑如下：

![image](https://user-images.githubusercontent.com/74815734/208721225-6a84da8b-1134-4580-a234-eff83f4d3f97.png)
![image](https://user-images.githubusercontent.com/74815734/208721234-dc7fabcb-185e-48e3-80fc-3e527307d000.png)

运行游戏，可以看到逻辑被正常运行。接着，我想加上当玩家按下ESC键时，游戏退出，这其中的逻辑如下（可以接着在原来的FlowChart中继续搭建逻辑）

![image](https://user-images.githubusercontent.com/74815734/208721276-4c392fc1-3611-49cb-b2fc-053f7d2a9d4d.png)
![image](https://user-images.githubusercontent.com/74815734/208721294-287228ae-34de-4861-9f75-4afaf5a85918.png)

理论上一张图里只要不是几百个节点都不会卡（应该），不过最好还是根据需求分开模块，一个游戏物品只能搭载一个FlowChart对应一张图，当逻辑多时就多搭建几张图，比如点击事件放一张图，状态事件放一张图等。

通过简单的Trigger和Action的连接，就可以搭建游戏逻辑了！

## 样例场景

通过上面的简单例子，应该可以理解节点图的使用方式了。在框架中的Assets\SugarzoNode\Scene中，有一个样例场景，可以参考里面的逻辑。

## 节点类型介绍

上面的例子介绍了框架的简单使用，接着介绍四个类型节点的运行逻辑。

### 触发器节点：
触发器节点是决定事件什么时候触发的关键，当条件满足时，该触发器节点连接的下一节点逻辑会被触发，此时该触发器State也会进入【执行中】的状态。
触发器只有一个输出端口，输出端口只能单连接。

![image](https://user-images.githubusercontent.com/74815734/208721691-b7497b44-6567-45ee-a185-693181d5cc8d.png)
![image](https://user-images.githubusercontent.com/74815734/208721708-d58cd15d-4798-4c51-8c26-617688df80a0.png)

触发器节点共有属性：

State:表示该触发器的状态，当触发器被触发时，State进入【执行中】，直到触发器触发的逻辑流向的最后一个节点执行完成后，State才会进入【执行完毕】。当事件节点在后面形成环形逻辑时，则会造成这个Trigger永远不会进入【执行完毕】的状态，请注意这点。
![image](https://user-images.githubusercontent.com/74815734/208721733-ada8f232-aedf-4901-b74f-5ea60960a8c4.png)

Trigger有几个共同属性。
生命周期执行：与Unity的MonoBehaviour执行顺序一致。
CanExecuteOnRunning：当触发器状态位于【执行中】时，该触发器是否能被再次触发，默认值为false
RunOnlyOnce:该触发器是否只能执行一次，当该选项被勾中时，触发器执行完成后便会摧毁自己（当勾选该选项时，CanExecuteOnRunning需要为false）注意这里Trigger的状态并不会被存档，意味着当场景被重载时，如果这个Trigger在资源场景里，该Trigger依然会被重新创建。

### 事件节点：
事件节点：决定该事件执行的逻辑内容
触发器拥有一个输入端口，一个输出端口，输入端口可以多连接，输出端口只能单连接。

![image](https://user-images.githubusercontent.com/74815734/208721850-21aa4194-03c2-454c-8bb1-d08ac7923da4.png)

事件节点共有属性：
Wait1Frame:执行时等待一帧
运行特性：
当事件节点为最后一个时（即它的output端口没有连接任何其他端口），事件结束时，就会将触发该事件的触发器Trigger节点State设置成【执行完毕】

