using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class SliderComponent : Singleton<SliderComponent>
{
    #region Serializable Fields

    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI SliderText;

    #endregion


    #region Private Fields

    private int Value = 0;

    #endregion

    private void Start()
    {
        DOVirtual.DelayedCall(0.25f, () =>
        {
            SliderInit();
            SliderTextUpdated();
        });
    }

    public async void SliderInit()
    {
        _slider.minValue = 0;
        _slider.maxValue = LevelComponent.Instance.GetWaves()[LevelComponent.Instance.GetCurrentWaveIndex()].Enemys
            .Count;
        _slider.value = 0;
    }

    public void SliderUpdated()
    {
        _slider.DOValue(Value, 0.25f);
    }

    public int GetSliderValue()
    {
        return Value;
    }

    public void SetSliderValue(int value)
    {
        Value += value;
    }

    public void SliderValueReset()
    {
        Value = 0;
    }

    public void SliderTextUpdated()
    {
        SliderText.text = string.Format("WAVE {0}/{1}", LevelComponent.Instance.GetCurrentWaveIndex() + 1,
            LevelComponent.Instance.GetWaves().Count);
    }
}