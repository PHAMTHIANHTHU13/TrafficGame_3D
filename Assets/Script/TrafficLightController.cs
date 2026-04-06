using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public enum LightState { Red, Yellow, Green }
    public LightState currentState;          // chỉnh lại trạng thái đèn đỏ xanh vàng đỏ
    public Renderer redLight;
    public Renderer yellowLight;
    public Renderer greenLight;

    public float greenTime = 3f;
    public float redTime = 10f;
    public float yellowTime = 7f;

    void Start()
    {
        StartCoroutine(LightRoutine());
    }

    IEnumerator LightRoutine()
    {
        while (true)
        {


            // đèn xanh
            SetLight(LightState.Green);
            yield return new WaitForSeconds(greenTime);

            // đèn đỏ
            SetLight(LightState.Red);
            yield return new WaitForSeconds(redTime);

            // đèn vàng
            SetLight(LightState.Yellow);
            yield return new WaitForSeconds(yellowTime);


        }
    }

    void SetLight(LightState state)
    {
        currentState = state;

        // tắt hết
        redLight.enabled = false;
        yellowLight.enabled = false;
        greenLight.enabled = false;

        // bật cái cần
        if (state == LightState.Red)
            redLight.enabled = true;

        else if (state == LightState.Yellow)
            yellowLight.enabled = true;

        else if (state == LightState.Green)
            greenLight.enabled = true;
    }

    public bool IsRed()
    {
        return currentState == LightState.Red;
    }

    public bool IsYellow()
    {
        return currentState == LightState.Yellow;
    }
}
