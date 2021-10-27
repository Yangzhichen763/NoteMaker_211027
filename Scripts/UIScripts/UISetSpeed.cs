using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class UISetSpeed : MonoBehaviour
{
    [SerializeField] private InputField input;
    [SerializeField] private UIBoxController mainController, minimapController;
    [SerializeField] private float speedMulti = 119.63f / 15;

    [SerializeField] private CanvasGroup recordCanvas;

    private int speed = 120;
    private bool play = false;

    private float mainCalcudSpeed, miniCalcudSpeed, nowLoadTime;


    public int Speed => speed;

    public void Start()
    {
        input ??= GetComponent<InputField>();

        input.onEndEdit.AddListener((string inputStr) =>
        {
            speed = int.Parse(inputStr);
            SetSpeed();
        });


        SetSpeed();

        recordCanvas.gameObject.SetActive(false);
    }
    private void SetSpeed()
    {
        mainCalcudSpeed = speed * speedMulti;
        miniCalcudSpeed = mainCalcudSpeed
        * (minimapController.Range.w - minimapController.Range.y)
        / (mainController.Range.w - mainController.Range.y);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Play();
    }
    public void FixedUpdate()
    {
        if (play)
        {
            mainController.Move(mainCalcudSpeed * Time.fixedDeltaTime);
            minimapController.Move(miniCalcudSpeed * Time.fixedDeltaTime);

            recordCanvas.alpha = Mathf.Sin((Time.timeSinceLevelLoad - nowLoadTime) * 4) * 0.4f + 0.6f;
        }
    }

    public void Play()
    {
        play = !play;

        bool recordActive = recordCanvas.gameObject.activeInHierarchy;
        if (recordActive ^ play)
            recordCanvas.gameObject.SetActive(!recordActive);


        nowLoadTime = Time.timeSinceLevelLoad;
    }
    public void MoveToBeginning()
    {
        StartCoroutine(MoveToBeginningIEnum());
    }
    private IEnumerator MoveToBeginningIEnum()
    {
        WaitForFixedUpdate second = new WaitForFixedUpdate();

        bool mainMove;
        do
        {
            mainMove = mainController.Move(-mainController.RollSpeed * 0.1f);
            minimapController.Move(-minimapController.RollSpeed * 0.1f);

            yield return second;
        } while (mainMove && !Input.anyKeyDown);
    }
}
