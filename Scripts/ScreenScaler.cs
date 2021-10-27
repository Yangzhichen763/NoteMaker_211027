using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    // 开发设备屏幕长宽和比
    public static float developWidth = 2560f, developHeight = 1440f;
    public static float developRate = developHeight / developWidth;

    // 用户设备屏幕长宽和比
    public static float userWidth = Screen.width, userHeight = Screen.height;
    public static float userRate = userHeight / userWidth;

    // main camera 和 screen 的 rect 的一个边的比
    public static float cameraRectHeightRate = developRate / userRate;
    public static float cameraRectWidthRate = userRate / developRate;

    [SerializeField] private Camera mainCamera, uiCamera;

    public Camera UICamera => uiCamera;
    public Camera MainCamera => mainCamera;
    public static float GetRate => Mathf.Min(developRate, developHeight);

    public void Start()
    {
        FindCamera();
    }

    /// <summary>每次场景转换调用一次</summary>
    private void FindCamera()
    {
        mainCamera ??= Camera.main;
        FitCamera(mainCamera);

        uiCamera ??= GameObject.Find("UICamera").GetComponent<Camera>();
        FitCamera(uiCamera);
    }

    /// <summary>相机适配</summary>
    /// <param name="camera">要被适配的相机</param>
    /// <returns>适配成功则返回 true</returns>
    public bool FitCamera(Camera camera)
    {
        if (camera == null) return false;

        if (developRate < userRate)
            camera.rect = new Rect(0, (1 - cameraRectHeightRate) * 0.5f, 1, cameraRectHeightRate);
        else
            camera.rect = new Rect((1 - cameraRectWidthRate) * 0.5f, 0, cameraRectWidthRate, 1);

        return true;
    }

    /// <summary>场景主相机(Camera.main)适配</summary>
    public bool FitMainCamera() => FitCamera(mainCamera);

    /// <summary>场景主用户面板相机(UICamera)适配</summary>
    public bool FitUICamera() => FitCamera(uiCamera);

    /// <summary> 返回留黑边后的物体在平面的坐标 </summary>
    /// <param name="position"></param>
    /// <returns>物体世界坐标投影到屏幕上的位置，返回的 position.z = 0</returns>
    public static Vector3 WorldToScreenPoint(Vector3 position)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);

        if (developRate < userRate)
        {
            screenPos = new Vector3(
                screenPos.x * cameraRectWidthRate,
                (screenPos.y - (1 - cameraRectHeightRate) * 0.5f * userHeight) * cameraRectWidthRate,
                0);
        }
        else
        {
            screenPos = new Vector3(
                (screenPos.x - (1 - cameraRectWidthRate) * 0.5f * userWidth) * cameraRectHeightRate,
                screenPos.y * cameraRectHeightRate,
                0);
        }

        return screenPos;
    }
    public static Vector2 ToUserPoint(Vector2 screenPoint)
    {
        if (developRate < userRate)
        {
            screenPoint = new Vector2(
                screenPoint.x * cameraRectWidthRate,
                (screenPoint.y - (1 - cameraRectHeightRate) * 0.5f * userHeight) * cameraRectWidthRate);
        }
        else
        {
            screenPoint = new Vector2(
                (screenPoint.x - (1 - cameraRectWidthRate) * 0.5f * userWidth) * cameraRectHeightRate,
                screenPoint.y * cameraRectHeightRate);
        }

        return screenPoint;
    }
    public static Vector2 ToDevelopPoint(Vector2 screenPoint)
    {
        if (developRate < userRate)
        {
            screenPoint = new Vector2(
                screenPoint.x * cameraRectHeightRate,
                screenPoint.y * cameraRectHeightRate + (1 - cameraRectHeightRate) * 0.5f * userHeight);
        }
        else
        {
            screenPoint = new Vector2(
                screenPoint.x* cameraRectHeightRate - (1 - cameraRectWidthRate) * 0.5f * userWidth,
                screenPoint.y * cameraRectWidthRate);
        }

        return screenPoint;
    }
}
