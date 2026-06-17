using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public enum LightState { Red, Green, Yellow }
    private LightState currentState;          // chỉnh lại trạng thái đèn đỏ xanh vàng đỏ
    public Renderer redLight;
    public Renderer yellowLight;
    public Renderer greenLight;

    public float greenTime = 3f; //này là đèn vàng
    public float redTime = 10f;
    public float yellowTime = 7f; // này là đèn xanh 

    public float phaseOffset = 0f;

    private Coroutine lightRoutine; // lưu coroutine hiện tại

    void Start()
    {
        SetLight(LightState.Red);

        // Dừng bất kỳ coroutine nào đang chạy (phòng trường hợp script bị reset)
        if (lightRoutine != null)
        {
            StopCoroutine(lightRoutine);
        }

        StartCoroutine(DelayTraffic(phaseOffset));
    }

    IEnumerator DelayTraffic(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(LightRoutine());
    }


    IEnumerator LightRoutine()
    {
        while (true)
        {
            // đỏ
            SetLight(LightState.Red);
            yield return new WaitForSeconds(redTime);


            // vàng (là xanh)
            SetLight(LightState.Yellow);
            yield return new WaitForSeconds(yellowTime);

            // xanh (là vàng)
            SetLight(LightState.Green);
            yield return new WaitForSeconds(greenTime);


        }
    }

    void SetLight(LightState state)
    {
        currentState = state;

        if (redLight != null)
        {
            redLight.gameObject.SetActive(false);
        }
        if (yellowLight != null)
        {
            yellowLight.gameObject.SetActive(false);
        }
        if (greenLight != null)
        {
            greenLight.gameObject.SetActive(false);
        }


        switch (state)
        {
            case LightState.Red:
                if (redLight != null) redLight.gameObject.SetActive(true);
                break;
            case LightState.Yellow:
                if (yellowLight != null) yellowLight.gameObject.SetActive(true);
                break;
            case LightState.Green:
                if (greenLight != null) greenLight.gameObject.SetActive(true);
                break;
        }
    }

    public bool IsRed() // dùng cho npc
    {
        return currentState == LightState.Red;
    }

    public bool IsYellow() // dùng cho npc
    {
        return currentState == LightState.Yellow;
    }

    public bool IsGreen() // dùng cho npc
    {
        return currentState == LightState.Green;
    }

    

}