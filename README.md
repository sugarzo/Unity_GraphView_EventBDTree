# Unity_GraphView_EventBDTree

本框架使用了GraphView和UI Toolkit，实现了类似行为树插件的可视化节点操作，方便长期项目中的游戏逻辑编写。

该框架比较轻量级，这篇文档用来介绍框架如何使用介绍如何使用，不涉及程序相关。后半部分针对程序讲解如何拓展脚本和新节点。

框架内置了Odin

节点图效果展示样例：

![image](https://user-images.githubusercontent.com/74815734/215321102-235c8c6f-8a28-4ee4-9571-c0c5e8e64c62.png)

### 作者账号和框架介绍文章

CSDN：https://blog.csdn.net/m0_51776409/article/details/127876213

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

### 条件节点：
一种特殊的事件节点，有两个输出端口，都只能单连接。当条件满足时流向true，不满足时流向false
![image](https://user-images.githubusercontent.com/74815734/208722148-9baea6d4-7928-478c-885f-f8612c519128.png)

### 序列节点：
一种特殊的事件节点，有一个支持多连接的输出端口（也是目前框架里唯一一个支持输出端口多连接的节点），可以整合多个流向的事件。该节点的执行逻辑有些不同，只有在所有流向的逻辑都执行完成时，才会返回【执行完成】给对应的触发器
![image](https://user-images.githubusercontent.com/74815734/208722206-3d2facc9-9007-4347-a994-059a3d751ecc.png)

## 节点扩展
以下是程序篇部分，可以通过创建新的脚本新增新的节点，在顶部窗口打开【FlowChart】

设置好文件名和保存脚本路径，点击Create即可，注意不要重名了。
![image](https://user-images.githubusercontent.com/74815734/208722314-5b19e52e-6ac9-4799-b3a0-57b5b769c088.png)

![image](https://user-images.githubusercontent.com/74815734/208722333-ec8e3b9a-abf1-41ea-b9aa-61cc386eb556.png)

所有节点都是state : MonoBehaviour的基类，所以都享有Unity GameObject的生命周期，可以被destroy和setActive。

```csharp
public enum EState
{
    [LabelText("未执行")]
    None,
    [LabelText("正在进入")]
    Enter,
    [LabelText("正在执行")]
    Running,
    [LabelText("正在退出")]
    Exit,
    [LabelText("执行完成")]
    Finish,
}
public interface IStateEvent
{
    void Execute();
    void OnEnter();
    void OnRunning();
    void OnExit();
}

//所有节点的基类
public abstract class NodeState : MonoBehaviour
{
#if UNITY_EDITOR
    [HideInInspector]
    public Vector2 nodePos;
#endif
    //流向下一节点的流
    [HideInInspector]
    public MonoState nextFlow;
}

public abstract class MonoState : NodeState, IStateEvent
{

}
```

参考UMG图如下（啊懒得画了，在这里文字描述一下继承关系）

NodeState : MonoBehaviour

MonoState : NodeState, IStateEvent

BaseTrigger：MonoState

BaseAction：MonoState

BaseBranch：BaseAction

BaseSeqence：BaseAction

### 触发器节点：

命名空间为SugarFrame.Node，模板中有两个内置注册事件和注销事件的函数，分别会在触Enable和DisEnable中执行，请保证最好注册注销事件需要对应。不然就会出现Trigger已经被摧毁了却依然在监听事件，导致Null错误。

```csharp
using UnityEngine;

namespace SugarFrame.Node
{
    public class #TTT# : BaseTrigger
    {
        //Called on Enable
        public override void RegisterSaveTypeEvent()
        {
            //EventManager.StartListening("",Execute);
        }

        //Called on DisEnable
        public override void DeleteSaveTypeEvent()
        {
            //EventManager.StopListening("",Execute);
        }
    }
}
```

自定义Trigger的核心在于何时调用Execute函数，当Execute执行时，代表Trigger触发。

例如ButtonTrigger的写法如下，当UI按钮被按下时触发事件：

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SugarFrame.Node
{
    public class ButtonTrigger : BaseTrigger
    {
        public List<Button> buttons;

        //Called on Enable
        public override void RegisterSaveTypeEvent()
        {
            foreach (var btn in buttons)
                btn.onClick.AddListener(Execute);
        }

        //Called on DisEnable
        public override void DeleteSaveTypeEvent()
        {
            foreach (var btn in buttons)
                btn.onClick.RemoveListener(Execute);
        }
    }
}
```

### 事件节点：

命名空间为SugarFrame.Node，只需要重写RunningLogic()，在逻辑执行完成时调用RunOver(emitTrigger)即可

请保证RunOver一定要被执行且一次逻辑中只被执行一次

```csharp
using UnityEngine;

namespace SugarFrame.Node
{
    public class #TTT# : BaseAction
    {
        [Header("#TTT#")]
        public string content;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            //Write Logic

            RunOver(emitTrigger);
        }
    }

}
```

如果是非实时逻辑（比如异步加载，需要等待），可以将RunOver传入对应委托，或者用协程挂起即可。下面是IntervalAction节点的参考写法：

```csharp
using UnityEngine;

using System;
using System.Collections;
using UnityEngine;

namespace SugarFrame.Node
{
    public class IntervalAction : BaseAction
    {
        [Header("等待x秒后执行下一个")]
        public float timer = 1f;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            StartCoroutine(WaitTime(()=>RunOver(emitTrigger)));
        }

        IEnumerator WaitTime(Action _event)
        {
            if(timer <= 0)
            {
                _event?.Invoke();
                yield break;
            }
            yield return new WaitForSeconds(timer);
            _event?.Invoke();
        }
    }

}
```

### 条件节点：
命名空间为SugarFrame.Node，只需要重写bool IfResult()的函数即可判断流向

```csharp
using UnityEngine;

namespace SugarFrame.Node
{
    public class #TTT# : BaseBranch
    {
        [Header("#TTT#")]
        public string content;

        public override bool IfResult()
        {
            return true;  
        }
    }

}
```

### 序列节点：

暂未提供拓展接口。

# V1.1 窗口UI更新

现在可以直接在顶部菜单打开空的FlowChart窗口了

![image](https://user-images.githubusercontent.com/74815734/215321145-8852daf7-76ee-401b-8ff8-154ca4e905ae.png)

可以在FlowChart窗口中直接切换场景中的图，并且可以直接使用【New按钮】创建新的节点图了

在游戏运行时，节点图可以依据该节点的运行状态会呈现不同颜色了，绿色代表已执行，蓝色为正在执行。该功能主要为Runtime的Debug服务

![image](https://user-images.githubusercontent.com/74815734/215321179-b8a2c0ab-cdcc-402a-bfa0-042be97c2d78.png)

# V1.2 新增节点的分包管理

新增了节点的包管理，每个节点都属于不同的包。不同的包的节点脚本在项目资源文件中分开放置。

![image](https://user-images.githubusercontent.com/74815734/215321242-92de535c-80b5-4102-a599-2779e17a0545.png)

如果想拓展新的节点包，在NodePackageType.cs中找到对应的枚举类型新增即可。

此外，新增了节点注释特性，用来标记在节点类的class上，使用这个特性来给节点分包和附加说明。

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false,Inherited =true)]
public class NodeNoteAttribute : System.Attribute
{
    public string note;
    public NodePackageType packageType;

    public NodeNoteAttribute(string _note = "",NodePackageType _packageType = NodePackageType.Base)
    {
        note = _note;
        packageType = _packageType;
    }
}
```

[NodeNote]有两个属性，一个是节点的说明（中文），一个是节点的隶属包。若不添加该特性则默认为基础包（Base）且没有中文注释。

![image](https://user-images.githubusercontent.com/74815734/215321317-987d6a43-1c49-49c4-adcd-6cbc82107dca.png)

![image](https://user-images.githubusercontent.com/74815734/215321319-f3daa0a2-3152-4838-820d-356c4b69bb29.png)

对应的节点创建窗口已更改，现在新创建的节点都会自带该特性。如果创建了新的包类型，记得修改下面的Package Paths，选择该包脚本的创建目录。

![image](https://user-images.githubusercontent.com/74815734/215321308-612ee8a8-5aac-4551-8d2d-635c6e178f4f.png)

