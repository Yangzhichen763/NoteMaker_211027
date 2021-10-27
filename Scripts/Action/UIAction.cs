using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIAction : MonoBehaviour
{
    public enum ActType : byte
    {
        Ease,
        Lerp,
    }

    public static UIAction manager;

    private List<VectorRecorder> moveRecorders;
    private List<VectorRecorder> scaleRecorders;
    private List<AlphaRecorder> alphaRecorders;

    public void Awake()
    {
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            this.enabled = false;
            return;
        }

        moveRecorders = new List<VectorRecorder>();
        alphaRecorders = new List<AlphaRecorder>();
        scaleRecorders = new List<VectorRecorder>();
    }
    public void Start()
    {

    }
    public void FixedUpdate()
    {
        CheckAndBuild<VectorRecorder, Transform, Vector3>(moveRecorders);

        for (int i = 0; i < scaleRecorders.Count; ++i)
        {
            VectorRecorder recorder = scaleRecorders[i];

            switch (recorder.GetActType)
            {
                case ActType.Ease:
                    if (!recorder.ScaleEase())
                    {
                        scaleRecorders.RemoveAt(i);
                        --i;
                    }
                    break;
                case ActType.Lerp:
                    if (!recorder.ScaleLerp())
                    {
                        scaleRecorders.RemoveAt(i);
                        --i;
                    }
                    break;
            }
        }

        CheckAndBuild<AlphaRecorder, CanvasGroup, float>(alphaRecorders);
    }


    /// <summary> 按给定曲线速度移动 </summary>
    /// <param name="transform"></param>
    /// <param name="startPosition"></param>
    /// <param name="finalPosition"></param>
    /// <param name="duration"></param>
    /// <param name="minRange"></param>
    /// <param name="easeFunction"></param>
    /// <param name="finalAction"></param>
    public void MoveEase(Transform transform, Vector3 startPosition, Vector3 finalPosition,
        float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        RemoveSame<VectorRecorder, Transform, Vector3>(transform, moveRecorders);

        VectorRecorder newMoveRecorder = new VectorRecorder(transform, startPosition, finalPosition,
            duration, minRange, easeFunction, finalAction);
        newMoveRecorder.SetType(ActType.Ease);

        moveRecorders.Add(newMoveRecorder);
    }
    public void MoveLerp(Transform transform, Vector3 finalPosition,
           float lerp_t, float minRange, Action finalAction)
    {
        RemoveSame<VectorRecorder, Transform, Vector3>(transform, moveRecorders);

        VectorRecorder newMoveRecorder = new VectorRecorder(transform, finalPosition,
            lerp_t, minRange, finalAction);
        newMoveRecorder.SetType(ActType.Lerp);

        moveRecorders.Add(newMoveRecorder);
    }
    public void MoveToward(Transform transform, Vector3 direction,
        float lerp_t, float minRange, Action finalAction)
    {
        MoveLerp(transform, transform.position + direction, lerp_t, minRange, finalAction);
    }

    public void ScaleEase(Transform transform, Vector3 startScale, Vector3 finalScale,
        float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        RemoveSame<VectorRecorder, Transform, Vector3>(transform, scaleRecorders);

        VectorRecorder newMoveRecorder = new VectorRecorder(transform, startScale, finalScale,
            duration, minRange, easeFunction, finalAction);
        newMoveRecorder.SetType(ActType.Ease);

        scaleRecorders.Add(newMoveRecorder);
    }

    public void AlphaEase(CanvasGroup canvas, float startAlpha, float finalAlpha,
        float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        RemoveSame<AlphaRecorder, CanvasGroup, float>(canvas, alphaRecorders);

        AlphaRecorder newAlphaRecorder = new AlphaRecorder(canvas, startAlpha, finalAlpha,
            duration, minRange, easeFunction, finalAction);
        newAlphaRecorder.SetType(ActType.Ease);

        alphaRecorders.Add(newAlphaRecorder);
    }
    public void AlphaLerp(CanvasGroup canvas, float finalAlpha,
        float lerp_t, float minRange, Action finalAction)
    {
        RemoveSame<AlphaRecorder, CanvasGroup, float>(canvas, alphaRecorders);

        AlphaRecorder newAlphaRecorder = new AlphaRecorder(canvas, finalAlpha,
            lerp_t, minRange, finalAction);
        newAlphaRecorder.SetType(ActType.Lerp);

        alphaRecorders.Add(newAlphaRecorder);
    }
    private void RemoveSame<T, Core, Tool>(Core compareCore, List<T> tList)
        where T : Recorder<Core, Tool> where Core : UnityEngine.Object
    {
        for (int i = 0; i < tList.Count; ++i)
        {
            T recorder = tList[i];
            if (recorder.GetCore == compareCore)
            {
                tList.RemoveAt(i);
                --i;
            }
        }
    }
    private void CheckAndBuild<T, Core, Tool>(List<T> recorders)
        where T: Recorder<Core, Tool> where Core : UnityEngine.Object
    {
        for (int i = 0; i < recorders.Count; ++i)
        {
            T recorder = recorders[i];

            switch (recorder.GetActType)
            {
                case ActType.Ease:
                    if (!recorder.EaseTo())
                    {
                        recorders.RemoveAt(i);
                        --i;
                    }
                    break;
                case ActType.Lerp:
                    if (!recorder.Lerp())
                    {
                        recorders.RemoveAt(i);
                        --i;
                    }
                    break;
            }
        }
    }

    public interface IAct<Core, Tool> where Core : UnityEngine.Object
    {
        public bool EaseTo();
        public bool Lerp();
    }
    private class Recorder<Core, Tool> : IAct<Core, Tool> where Core : UnityEngine.Object
    {
        protected ActType m_actType;
        public ActType GetActType => m_actType;


        protected float t = 0;

        protected float m_functionT;
        protected float m_minRangeSqr;

        protected Func<float, float> m_easeFunction;
        protected Action m_finalAction;

        protected Tool m_start, m_end;


        protected Core m_core;

        public Core GetCore => m_core;

        public Recorder(Core core, Tool start, Tool end,
            float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
        {
            this.m_core = core;
            this.m_start = start;
            this.m_end = end;

            this.m_easeFunction = easeFunction;
            this.m_finalAction = finalAction;

            m_functionT = 1 / duration;
            m_minRangeSqr = minRange * minRange;
        }
        public Recorder(Core core, Tool end,
          float lerp_t, float minRange, Action finalAction)
        {
            this.m_core = core;
            this.m_end = end;

            this.m_finalAction = finalAction;

            m_functionT = lerp_t;
            m_minRangeSqr = minRange * minRange;
        }
        public Recorder() { }

        /// <summary> 设置移动方式 </summary>
        /// <param name="moveType"></param>
        public void SetType(ActType moveType) => m_actType = moveType;
        ///<summary> 直达目的地 </summary>
        /// <returns> 如果已经到了目的大小，则返回 false </returns>
        public virtual bool EaseTo() => false;
        ///<summary> 按指数直达目的地 </summary>
        /// <returns> 如果已经到了目的大小，则返回 false </returns>
        public virtual bool Lerp() => false;
        ///<summary> 往一个方向移动 </summary>
        /// <returns> 如果已经到了目的大小，则返回 false </returns>
        public virtual bool EaseToward() => false;
    }
    private class VectorRecorder : Recorder<Transform, Vector3>, IAct<Transform, Vector3>
    {
        public VectorRecorder(Transform transform, Vector3 startPosition, Vector3 finalPosition,
            float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
            : base(transform, startPosition, finalPosition, duration, minRange, easeFunction, finalAction) { }
        public VectorRecorder(Transform transform, Vector3 finalPosition,
           float lerp_t, float minRange, Action finalAction)
            : base(transform, finalPosition, lerp_t, minRange, finalAction) { }

        public void TimePass() => t += Time.deltaTime * m_functionT;

        /// <summary> 移动 </summary>
        /// <returns> 返回是否正在移动，如果已经到达了终点，则返回 false </returns>
        public override bool EaseTo()
        {
            m_core.position = VectorEase(out bool returnBool);
            return returnBool;
        }
        /// <summary> 缩放 </summary>
        /// <returns> 返回是否正在缩放，如果已经到了目的大小，则返回 false </returns>
        public bool ScaleEase()
        {
            Vector3 scale = VectorEase(out bool returnBool);
            if (returnBool)
                m_core.localScale = scale;
            return returnBool;
        }

        private Vector3 VectorEase(out bool returnBool)
        {
            Vector3 deltaVec = m_end - m_start;
            Vector3 finalVec = m_start + deltaVec * m_easeFunction(this.t);

            t += UITimeManager.fixedDeltaTime * m_functionT;

            if (t >= 1)
            {
                finalVec = m_end;

                m_finalAction?.Invoke();
                returnBool = false;
            }
            else
                returnBool = true;

            return finalVec;
        }
        /// <summary> 移动 </summary>
        /// <returns> 返回是否正在移动，如果已经到达了终点，则返回 false </returns>
        public override bool Lerp()
        {
            m_core.position = VectorLerp(m_core.position, out bool returnBool);
            return returnBool;
        }
        /// <summary> 缩放 </summary>
        /// <returns> 返回是否正在缩放，如果已经到了目的大小，则返回 false </returns>
        public bool ScaleLerp()
        {
            m_core.localScale = VectorLerp(m_core.localScale, out bool returnBool);
            return returnBool;
        }
        private Vector3 VectorLerp(Vector3 inputVec, out bool returnBool)
        {
            Vector3 transVec = inputVec;
            inputVec = Vector3.Lerp(transVec, m_end, m_functionT);
            if (Vector3.SqrMagnitude(transVec - m_end) < m_minRangeSqr)
            {
                inputVec = m_end;

                m_finalAction?.Invoke();
                returnBool = false;
            }
            else returnBool = true;

            return inputVec;
        }
    }
    private class AlphaRecorder : Recorder<CanvasGroup, float>, IAct<CanvasGroup, float>
    {
        public AlphaRecorder(CanvasGroup canvas, float startAlpha, float finalAlpha,
            float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
            : base(canvas, startAlpha, finalAlpha, duration, minRange, easeFunction, finalAction) { }

        public AlphaRecorder(CanvasGroup canvas, float finalAlpha, float lerp_t, float minRange, Action finalAction)
            : base(canvas, finalAlpha, lerp_t, minRange, finalAction) { }

        public override bool EaseTo()
        {
            m_core.alpha = FloatEase(out bool returnBool);
            return returnBool;
        }
        private float FloatEase(out bool returnBool)
        {
            float deltaVec = m_end - m_start;
            float finalVec = m_start + deltaVec * m_easeFunction(this.t);

            t += UITimeManager.fixedDeltaTime * m_functionT;

            if (t >= 1)
            {
                m_finalAction?.Invoke();

                finalVec = m_end;
                returnBool = false;
            }
            else
                returnBool = true;

            return finalVec;
        }
        public override bool Lerp()
        {
            m_core.alpha = FloatLerp(m_core.alpha, out bool returnBool);
            return returnBool;
        }
        private float FloatLerp(float inputFloat, out bool returnBool)
        {
            float tmpFloat = inputFloat;
            inputFloat = Mathf.Lerp(tmpFloat, m_end, m_functionT);
            float delta = tmpFloat - m_end;
            if (delta * delta < m_minRangeSqr)
            {
                m_finalAction?.Invoke();

                inputFloat = m_end;
                returnBool = false;
            }
            else returnBool = true;

            return inputFloat;
        }
    }
}

public static class UIActionTool
{
    public static void MoveEase(this Transform transform, Vector3 startPosition, Vector3 finalPosition,
        float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        UIAction.manager.MoveEase(transform, startPosition, finalPosition,
            duration, minRange, easeFunction, finalAction);
    }
    public static void MoveLerp(this Transform transform, Vector3 finalPosition,
           float lerp_t, float minRange, Action finalAction)
    {
        UIAction.manager.MoveLerp(transform, finalPosition, lerp_t, minRange, finalAction);
    }
    public static void MoveToward(this Transform transform, Vector3 direction,
        float lerp_t, float minRange, Action finalAction)
    {
        UIAction.manager.MoveToward(transform, direction, lerp_t, minRange, finalAction);
    }


    public static void ScaleEase(this Transform transform, Vector3 startScale, Vector3 finalScale,
        float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        UIAction.manager.ScaleEase(transform, startScale, finalScale,
            duration, minRange, easeFunction, finalAction);
    }

    public static void MoveEase(this GameObject gameObject, Vector3 startPosition, Vector3 finalPosition,
       float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        UIAction.manager.MoveEase(gameObject.transform, startPosition, finalPosition,
            duration, minRange, easeFunction, finalAction);
    }
    public static void MoveLerp(this GameObject gameObject, Vector3 finalPosition,
           float lerp_t, float minRange, Action finalAction)
    {
        UIAction.manager.MoveLerp(gameObject.transform, finalPosition, lerp_t, minRange, finalAction);
    }
    public static void MoveToward(this GameObject gameObject, Vector3 direction,
        float lerp_t, float minRange, Action finalAction)
    {
        UIAction.manager.MoveToward(gameObject.transform, direction, lerp_t, minRange, finalAction);
    }

    public static void AlphaLerp(this CanvasGroup canvas, float finalAlpha,
        float lerp_t, float minRange, Action finalAction)
    {
        UIAction.manager.AlphaLerp(canvas, finalAlpha, lerp_t, minRange, finalAction);
    }
    public static void AlphaEase(this CanvasGroup canvas, float startAlpha, float finalAlpha,
        float duration, float minRange, Func<float, float> easeFunction, Action finalAction)
    {
        UIAction.manager.AlphaEase(canvas, startAlpha, finalAlpha, duration, minRange, easeFunction, finalAction);
    }
}
