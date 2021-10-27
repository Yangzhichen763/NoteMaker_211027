using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoxController : MonoBehaviour
{
    [SerializeField] private RectTransform boxTrans;
    [HideInInspector] public Vector2Int boxSize;

    [SerializeField] private float moveSensi = 1, scaleSensi = 1, rollMoveSensi = 50;
    [SerializeField, Tooltip("左下右上的边界")] private Vector4 limitRange;

    private Vector2 scale = Vector2.one;
    private Vector2 lastMousePos;

    [Header("Editor 部分")]
    [SerializeField] private RectTransform showRangeTrans;
    [SerializeField] private RectTransform editRangeTrans;

    public RectTransform Panel => boxTrans;
    public Vector2 Scale => scale;
    public Vector4 Range => limitRange;
    public float RollSpeed => rollMoveSensi;
    public Vector2 Position => boxTrans.anchoredPosition;

#if UNITY_EDITOR
    public void OnValidate()
    {
        DisplayPrefeb(ref limitRange);
    }
#endif

    public void Start()
    {
        boxTrans ??= GetComponent<RectTransform>();
    }
    public void Update()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScroll != 0)
            InputScale(boxTrans, mouseScroll);
        InputMove(boxTrans, ref lastMousePos);
    }

    private void InputScale(RectTransform rectTrans, float mouseScroll)
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //左右缩放
            scale += mouseScroll * scaleSensi * Vector2.right * scale.x;

            AppendRect(limitRange, rectTrans, scale);
            LimitRect(limitRange, rectTrans, ref scale);
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            //上下缩放
        }
        else if (mouseScroll != 0)
        {
            //视角左右移动
            rectTrans.anchoredPosition += mouseScroll * Vector2.right * rollMoveSensi;

            AppendRect(limitRange, rectTrans, scale);
            LimitRect(limitRange, rectTrans, ref scale);
        }
    }
    private void InputMove(RectTransform rectTrans, ref Vector2 lastMousePos)
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePos = Input.mousePosition;
        }
        // 视角自由移动
        else if (Input.GetMouseButton(2))
        {
            // 鼠标位置移动
            Vector2 mousePos = Input.mousePosition;
            Vector2 dirVec = mousePos - lastMousePos;
            lastMousePos = mousePos;

            // 计算移动后的位置和检验
            rectTrans.anchoredPosition += dirVec * moveSensi;

            AppendRect(limitRange, rectTrans, scale);
            LimitRect(limitRange, rectTrans, ref scale);
        }
    }

    /// <summary> 移动 </summary>
    /// <param name="deltaMove"> 移动的距离 </param>
    /// <returns> 如果到达边界，则返回 false </returns>
    public bool Move(float deltaMove)
    {
        RectTransform rectTrans = boxTrans;
        rectTrans.anchoredPosition += deltaMove * Vector2.left;

        bool returnBool = AppendRect(limitRange, rectTrans, scale);
        LimitRect(limitRange, rectTrans, ref scale);

        return returnBool;
    }
    /// <summary> 限制 rect </summary>
    /// <param name="boundary"> 边界条件 </param>
    /// <param name="rectTrans"> 要被限制的 rectTrans </param>
    /// <param name="scale"> scale </param>
    private void LimitRect(Vector4 boundary, RectTransform rectTrans, ref Vector2 scale)
    {
        Rect rect = rectTrans.rect;
        Vector2 anchoredPosition = rectTrans.anchoredPosition;

        // 限制 Size
        float width = boundary.z - boundary.x, height = boundary.w - boundary.y;
        float scale_x = width / rect.width - scale.x, scale_y = height / rect.height - scale.y;
        Vector2 maxDelta = new Vector2(scale_x > 0 ? scale_x : 0, scale_y > 0 ? scale_y : 0);
        scale += maxDelta;

        // 限制 position
        float left = anchoredPosition.x, down = anchoredPosition.y,
            right = anchoredPosition.x + rect.width * scale.x, up = anchoredPosition.y + rect.height * scale.y;
        if (left > boundary.x) anchoredPosition.x = boundary.x;
        if (down > boundary.y) anchoredPosition.y = boundary.y;
        if (right < boundary.z) anchoredPosition.x += boundary.z - right;
        if (up < boundary.w) anchoredPosition.y += boundary.w - up;

        // 赋值
        rectTrans.anchoredPosition = anchoredPosition;
        rectTrans.localScale = scale;
    }
    /// <summary> 向右水平拓展 rect </summary>
    /// <param name="boundary"> 边界条件 </param>
    /// <param name="rectTrans"> 要被限制的 rectTrans </param>
    /// <param name="scale"> scale </param>
    /// <returns> 如果碰到左边边界，就返回 false </returns>
    private bool AppendRect(Vector4 boundary, RectTransform rectTrans, Vector2 scale)
    {
        bool append = true;

        // 拓展 rect
        Rect rect = rectTrans.rect;
        Vector2 anchoredPosition = rectTrans.anchoredPosition;
        float left = anchoredPosition.x, right = anchoredPosition.x + rect.width * scale.x;
        if (left > boundary.x)
        {
            anchoredPosition.x = boundary.x;
            append = false;
        }
        if (right < boundary.z)
        {
            anchoredPosition = new Vector2(left, boundary.z);
            rect.size = new Vector2((boundary.z - left) / scale.x, rect.height);
        }

        // 赋值
        rectTrans.anchoredPosition = anchoredPosition;
        rectTrans.sizeDelta = rect.size;


        return append;
    }

    private void DisplayPrefeb(ref Vector4 limitRange)
    {
        Vector2 pos = showRangeTrans.anchoredPosition
            + 0.5f * new Vector2(ScreenScaler.developWidth, ScreenScaler.developHeight)
            - showRangeTrans.pivot * showRangeTrans.sizeDelta;
        Vector2 size = showRangeTrans.rect.size;
        limitRange = new Vector4(pos.x, pos.y, pos.x + size.x, pos.y + size.y);

        if (editRangeTrans != null)
        {
            editRangeTrans.anchoredPosition = new Vector2(limitRange.x, limitRange.y);
            editRangeTrans.sizeDelta = size;
        }
    }
}
