using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
namespace SugarFrame.Node
{
    public class GameObjectPathMoveAction : BaseAction
    {
        [Header("GameObjectPathMoveAction")]
        public GameObject MoveTarget;

        [LabelText("移动时间")] public float timer = 1;
        [LabelText("路线点父物品")] public Transform targetPos;
        [LabelText("路线类型")] public MoveType moveType;
        [LabelText("等待移动完成")] public bool waitFinish = true;
        public Color lineColor = Color.red;
        public enum MoveType { Linear, Bezier }


        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            init();

            StartCoroutine(StartMove(emitTrigger));

            if (!waitFinish)
                RunOver(emitTrigger);
        }

        //物品开始沿着childPos点移动
        IEnumerator StartMove(BaseTrigger emitTrigger)
        {
            yield return null;
            if (childPos.Count < 2)
            {
                if (waitFinish)
                    RunOver(emitTrigger);
                yield break;
            }
            float edgeTime = timer / (childPos.Count - 1); //在两点之间移动的时间 = 总的时间/边数

            MoveTarget.transform.position = childPos[0]; //将移动物品放在初始点

            for (int i = 1; i < childPos.Count; i++)
            {
                MoveTarget.transform.DOMove(childPos[i], edgeTime);
                yield return new WaitForSeconds(edgeTime);
                MoveTarget.transform.position = childPos[i];
                //Debug.Log(MoveTarget.name + "移动中 " + i + "/" + (childPos.Count - 1));
            }

            if (waitFinish)
                RunOver(emitTrigger);
        }

        [SerializeField] private List<Vector3> childPos = new List<Vector3>();
        /// <summary>
        /// 生成childPos轨迹点
        /// </summary>
        private void init()
        {
            childPos.Clear();

            if (moveType == MoveType.Linear) //线性轨迹的解析
            {
                for (int i = 0; i < targetPos.childCount; i++)
                {
                    childPos.Add(targetPos.GetChild(i).position);
                }
            }
            if (moveType == MoveType.Bezier) //贝塞尔曲线轨迹的解析
            {
                var pos = new List<Transform>(targetPos.GetComponentsInChildren<Transform>());
                childPos = BezierMath.GetBezierPoint(pos, 100);
            }

        }
        //在编辑窗口中画曲线，游戏内不显示
        void OnDrawGizmos()
        {
            if (targetPos == null)
                return;

            init();
            Gizmos.color = lineColor;
            for (int i = 0; i < childPos.Count - 1; i++)
            {
                Gizmos.DrawLine(childPos[i], childPos[i + 1]);
            }
        }

    }
    //贝塞尔曲线数学公式
    public class BezierMath
    {

        /// <summary>
        /// 返回pointNum个由pointPos决定的贝塞尔曲线点
        /// </summary>
        /// <param name="pointPos"></param>
        /// <param name="pointNum"></param>
        /// <returns></returns>
        public static List<Vector3> GetBezierPoint(List<Transform> pointPos, int pointNum = 100)
        {
            List<Vector3> ret = new List<Vector3>();

            for (float i = 0; i <= pointNum; i++)
            {
                ret.Add(Bezier(i / pointNum, pointPos));
            }

            return ret;
        }


        /// <summary>
        /// 二次贝塞尔 0<t<1 ,p0起点，p2终点
        /// </summary>
        public static Vector3 Bezier_2(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }
        public static void Bezier_2ref(ref Vector3 outValue, Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            outValue = (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }
        /// <summary>
        /// 三次贝塞尔, 0<t<1 ,p0起点，p3终点
        /// </summary>
        public static Vector3 Bezier_3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
        public static void Bezier_3ref(ref Vector3 outValue, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            outValue = (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
        // n阶贝塞尔，递归实现
        public static Vector3 Bezier(float t, List<Vector3> p)
        {
            if (p.Count < 2)
                return p[0];
            List<Vector3> newp = new List<Vector3>();
            for (int i = 0; i < p.Count - 1; i++)
            {
                //Debug.DrawLine(p[i], p[i + 1]);
                Vector3 p0p1 = (1 - t) * p[i] + t * p[i + 1];
                newp.Add(p0p1);
            }
            return Bezier(t, newp);
        }
        // transform转换为vector3，在调用参数为List<Vector3>的Bezier函数
        public static Vector3 Bezier(float t, List<Transform> p)
        {
            if (p.Count < 2)
                return p[0].position;
            List<Vector3> newp = new List<Vector3>();
            for (int i = 0; i < p.Count; i++)
            {
                newp.Add(p[i].position);
            }
            return Bezier(t, newp);
        }
        public static Vector3 Bezier(float t, GameObject exp)
        {

            if (exp.transform.childCount < 2)
                return exp.transform.position;
            List<Transform> p = new List<Transform>(exp.GetComponentsInChildren<Transform>());
            List<Vector3> newp = new List<Vector3>();
            for (int i = 0; i < p.Count; i++)
            {
                newp.Add(p[i].position);
            }
            return Bezier(t, newp);
        }
    }

}
