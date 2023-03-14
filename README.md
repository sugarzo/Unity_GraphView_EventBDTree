# Unity_GraphView_EventBDTree

本框架使用了GraphView和UI Toolkit(内置了Odin)，实现了事件的的可视化节点操作，方便长期项目中的游戏逻辑编写。该框架比较轻量级，这篇文档用来介绍框架如何使用，前半部分是程序策划篇，只介绍如何使用比较简单，不需要代码知识也能理解。后半部分针对程序讲解如何拓展脚本和新节点。

节点图效果展示样例：

![image](https://user-images.githubusercontent.com/74815734/215321102-235c8c6f-8a28-4ee4-9571-c0c5e8e64c62.png)

### 作者账号和框架介绍文章

CSDN：https://blog.csdn.net/m0_51776409/article/details/127876213

## 如何搭建逻辑
现在有两种可以创建FlowChart物品的方式。

第一种是在Unity的Hierarchy窗口中，右键新建物品菜单中，可以看到一个FlowChart的选项，点击新建，场景便会多一个FlowChart游戏物品。选中FlowChart，点击上面挂载的脚本“Open”，就可以打开节点面板了。随后的后续编辑也只需要通过这个按钮进入即可。

Note文本框是用来写注释的，不影响实际运行效果。

![image](https://user-images.githubusercontent.com/74815734/208720792-984bff88-4a86-4b3c-8bda-6125fbad0806.png)
![image](https://user-images.githubusercontent.com/74815734/208720811-d99cc1fe-6514-4af6-93c4-6cf83eaea162.png)

第二种是在顶部菜单中，找到FlowChart即可直接打开窗口。在节点窗口顶部点击New即可在场景中直接创建FlowChart物品。

![image](https://user-images.githubusercontent.com/74815734/224997195-be116d07-6e4c-4c44-81e9-e191d5ef6b31.png)

进入节点图窗口后会的布局如下：

![image](https://user-images.githubusercontent.com/74815734/224997287-77cd2210-6132-472a-a606-acb5913bd5a7.png)

如果是新建的Flowchart物品，目前节点区域是什么都没有的。编辑面板主要分为两部分：左边的Inspector面板用于显示和编辑点击节点的属性，右边的图用来显示节点（目前是空的）
在右边的节点图区域右键，点击Create Node，可以看到可以创建的节点列表：

![image](https://user-images.githubusercontent.com/74815734/208721167-ccf1747b-787d-4756-b228-b1dfc58c43de.png)
![image](https://user-images.githubusercontent.com/74815734/208721179-4c759be5-b797-4681-8db0-44189bd19a2a.png)

节点主要分为四大类：触发器节点、行为（事件）节点、分支节点、序列节点。

其中，触发器和行为节点是主要的节点：触发器节点决定什么时候触发这个事件（例如玩家按下某个按键，游戏中某个状态改变到指定值，游戏内某个事件发生都可以算是一个触发时机），行为节点决定了进入该节点后程序需要执行什么逻辑。具体主要节点的功能见文档末尾的节点附录。

例如，我想执行一段逻辑：当进入游戏场景的时候开始播放游戏音乐BGM，这时候已经知道程序已经提前写好了对应播放音乐的Action节点和Trigger节点。

新建一个Trigger和AudioAction，在创建节点列表中找到对应节点，点击对应节点可以在左边Inspector面板中设置对应属性值，连接逻辑如下：

![image](https://user-images.githubusercontent.com/74815734/208721225-6a84da8b-1134-4580-a234-eff83f4d3f97.png)
![image](https://user-images.githubusercontent.com/74815734/208721234-dc7fabcb-185e-48e3-80fc-3e527307d000.png)

运行游戏，可以看到逻辑被正常运行。接着，我想加上当玩家按下ESC键时，游戏退出，这其中的逻辑如下（可以接着在原来的FlowChart中继续搭建逻辑）

![image](https://user-images.githubusercontent.com/74815734/208721276-4c392fc1-3611-49cb-b2fc-053f7d2a9d4d.png)
![image](https://user-images.githubusercontent.com/74815734/208721294-287228ae-34de-4861-9f75-4afaf5a85918.png)

在搭建时，节点也可以通过复制和粘贴创建。但是跨图粘贴目前是不可以的。一个图里可以放很多节点。但最好还是根据需求分开模块，一个游戏物品只能搭载一个FlowChart对应一张图，当逻辑多时就多搭建几张图，比如点击事件放一张图，状态事件放一张图等。

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

## 节点颜色与调试

在游戏运行时，也可以打开顶部菜单打开节点窗口，观察节点颜色从而查看该节点是否正常运行。

灰色：未执行

蓝色：正在执行

绿色：已执行完毕

![image](https://user-images.githubusercontent.com/74815734/224998268-b6aceb5b-8cb1-4f60-81b1-f8cd078d81a6.png)

可以通过修改节点菜单中的节点注释给节点改名。

![image](https://user-images.githubusercontent.com/74815734/224998327-387eb21a-14b2-4252-b104-0244cde71ea1.png)

# 节点扩展

接着是程序篇部分，这里讲解如何新建和扩展节点。

## 如何创建新节点

在顶部菜单中，点击FlowChart/FlowChart节点配置即可打开节点的创建和配置窗口

![image](https://user-images.githubusercontent.com/74815734/224998498-6afa5a49-344b-4ab6-bb29-d2d160441ecb.png)

每个节点的新建其实就是创建了一个cs脚本。新建新节点需要确定三个属性：

节点名称：就是脚本类名

节点类型：提供了触发器/条件/事件类型。这里不同类型的区别已在上文中介绍。

节点隶属包/节点注释(可中文)名：这个对运行时没有任何影响，只是在创建节点菜单中分类改名方便管理。

![image](https://user-images.githubusercontent.com/74815734/224998599-b9e0f329-d87c-4ff2-892d-45c91a006b10.png)
![image](https://user-images.githubusercontent.com/74815734/224998580-c764d762-f76a-4fbb-ae81-67e0d4fa01ec.png)

节点包名称和节点注释其实是通过特性Attribute标记上去的。如果后面需要更改已创建的脚本的类型可以进入脚本界面直接修改。

![image](https://user-images.githubusercontent.com/74815734/224998696-68ff9ad3-481b-4b15-818b-c31ac84144bc.png)

决定好上面这些后就可以点击Create按钮新建脚本了。

![image](https://user-images.githubusercontent.com/74815734/224998731-554dced8-19d7-4690-8836-e3ddd07c785c.png)

考虑到不同节点用处不同，所以提出了节点分包。如果节点依赖于游戏中一个独立的模块，应该单独划分一个新类型包。

在节点配置中的【节点分类包和路径】中，可以新建/删除不同类型的脚本包。在这里需要设置不同的脚本创建路径。设置好后在创建节点窗口中就可以使用了。

![image](https://user-images.githubusercontent.com/74815734/224998791-c4ee0e7e-d623-49b3-a6a9-12895f310d48.png)

## 节点的程序逻辑编写

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

参考的继承关系如下。

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

暂未提供拓展接口。不过可以通过继承BaseSequence脚本手动设置不同流向。这里展示一下Sequence的节点写法。

```csharp
namespace SugarFrame.Node
{
    public abstract class BaseSequence : BaseAction
    {

        [HideInInspector]
        public List<MonoState> nextflows = new List<MonoState>();
        [Header("每个行为之间是否等待x秒,输入-1时等待1帧")]
        public float waitTimeEachAction = 0;
        [ReadOnly]
        public int runningAction = 0;

        public override void OnEnter()
        {           
            base.OnEnter();
        }


        /// <summary>
        /// 向下执行所有节点
        /// </summary>
        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            if (nextflows != null && nextflows.Count > 0)
            {
                runningAction = nextflows.Count;
                StartCoroutine(StartActions(emitTrigger));
            }
            else
            {
                //Sequence节点输出为空，直接切换到结束状态
                RunOver(emitTrigger);
            }
        }

        private IEnumerator StartActions(BaseTrigger emitTrigger)
        {
            DataCache cache = new DataCache();
            cache.count = nextflows.Count;
            cache.trigger = emitTrigger;

            //继续所有节点
            foreach (var nextFlow in nextflows)
            {
                //依赖注入,当所有Action执行完成时回调Trigger
                if (nextFlow is BaseAction action)
                    action.OnExitEvent += delegate ()
                    {
                        cache.count--;
                        if(cache.count == 0)
                        {
                            cache.trigger?.OnExit();
                        }
                    };

                if (nextFlow is BaseAction nextAction)
                    nextAction.Execute();
                else
                    nextFlow.Execute();

                if (waitTimeEachAction > 0)
                    yield return new WaitForSeconds(waitTimeEachAction);
                if (waitTimeEachAction == -1)
                    yield return null;
            }
            yield return null;
        }


        private class DataCache
        {
            public BaseTrigger trigger;
            public int count;
        }
    }
}
```

# 节点附录(该github版本只展示UnityBase包)

## Base基础包

该目录下的节点为Unity默认功能，可以在无前置条件下调用。

### Action功能列表

IntervalAction
执行到该事件时，暂停x秒，随后再执行接下来的事件。

UnityEventAction
public出去了一个UnityEvent

DebugLogAction
Debug一段信息

GameObjectAction
配置场景中GameObject的，选择将它们SetActive或者Destroy

GameObjectMoveAction
将场景中的GameObject移动到另一个位置

GameObjectPathMoveAction
将场景中的GameObject沿着指定路径移动到另一个位置，需要配置路径点的位置，支持直线移动和贝塞尔曲线移动。路径已通过Gizmo.Draw画线，可在Scene中查看

ShaderControlAction
设置一个材质中Shader中的参数

SpriteMixAction
混合两个sprite的color，调整color.a的大小1-0或者0-1。支持UIImage或者Sprite2D

TriggerEventAction
可以执行/注册/注销一个Trigger的事件

AnimatorAction
控制Animator的动画播放

### Trigger功能列表

ButtonTrigger
指定一个Button，在onClick事件中触发

InputAnyTrigger
按下键盘上的指定按钮时触发

Trigger
可以指定GameOjbect的某一个生命周期（Awake,Update等），与该生命周期同步触发


