using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    // �����豸��Ļ����ͱ�
    public static float developWidth = 2560f, developHeight = 1440f;
    public static float developRate = developHeight / developWidth;

    // �û��豸��Ļ����ͱ�
    public static float userWidth = Screen.width, userHeight = Screen.height;
    public static float userRate = userHeight / userWidth;

    // main camera �� screen �� rect ��һ���ߵı�
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

    /// <summary>ÿ�γ���ת������һ��</summary>
    private void FindCamera()
    {
        mainCamera ??= Camera.main;
        FitCamera(mainCamera);

        uiCamera ??= GameObject.Find("UICamera").GetComponent<Camera>();
        FitCamera(uiCamera);
    }

    /// <summary>�������</summary>
    /// <param name="camera">Ҫ����������</param>
    /// <returns>����ɹ��򷵻� true</returns>
    public bool FitCamera(Camera camera)
    {
        if (camera == null) return false;

        if (developRate < userRate)
            camera.rect = new Rect(0, (1 - cameraRectHeightRate) * 0.5f, 1, cameraRectHeightRate);
        else
            camera.rect = new Rect((1 - cameraRectWidthRate) * 0.5f, 0, cameraRectWidthRate, 1);

        return true;
    }

    /// <summary>���������(Camera.main)����</summary>
    public bool FitMainCamera() => FitCamera(mainCamera);

    /// <summary>�������û�������(UICamera)����</summary>
    public bool FitUICamera() => FitCamera(uiCamera);

    /// <summary> �������ڱߺ��������ƽ������� </summary>
    /// <param name="position"></param>
    /// <returns>������������ͶӰ����Ļ�ϵ�λ�ã����ص� position.z = 0</returns>
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
