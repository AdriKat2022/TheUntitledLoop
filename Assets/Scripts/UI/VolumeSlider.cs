using AdriKat.Toolkit.Attributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private string sliderName;
    [SerializeField] private string audioMixerParameter;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Icons")]
    [SerializeField] bool useVolumeIcons;
    [ShowIf(nameof(useVolumeIcons))]
    [SerializeField] private Image volumeIcon;
    [ShowIf(nameof(useVolumeIcons))]
    [SerializeField] private Sprite highVolumeIcon;
    [ShowIf(nameof(useVolumeIcons))]
    [SerializeField] private Sprite mediumVolumeIcon;
    [ShowIf(nameof(useVolumeIcons))]
    [SerializeField] private Sprite lowVolumeIcon;
    [ShowIf(nameof(useVolumeIcons))]
    [SerializeField] private Sprite noVolumeIcon;

    [Header("Other References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI nameTextMesh;
    [SerializeField] private TextMeshProUGUI valueTextMesh;

    private bool volumeActive = true;
    private float previousValue;

    private void Start()
    {
        if (!audioMixer.GetFloat(audioMixerParameter, out float mixerValue))
        {
            Debug.LogError(audioMixerParameter + " is not a public parameter of the audioMixer " + audioMixer.ToString());
        }

        nameTextMesh.text = sliderName;

        // Get saved parameters in PlayerPrefs
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(audioMixerParameter, 1f));
        volumeActive = PlayerPrefs.GetInt(audioMixerParameter + "_active", 1) == 1;
        previousValue = slider.value;

        UpdateMixerValue();

        slider.onValueChanged.AddListener(SliderUpdate);
    }

    public void SliderUpdate(float sliderValue)
    {
        // Save into PlayerPrefs
        if (sliderValue > previousValue)
        {
            volumeActive = true;
            PlayerPrefs.SetInt(audioMixerParameter + "_active", 1);
        }
        PlayerPrefs.SetFloat(audioMixerParameter, sliderValue);
        PlayerPrefs.Save();
        previousValue = sliderValue;

        UpdateMixerValue();
    }

    public void SwitchVolume()
    {
        volumeActive = !volumeActive;

        // Save into PlayerPrefs
        PlayerPrefs.SetInt(audioMixerParameter + "_active", volumeActive ? 1 : 0);
        PlayerPrefs.Save();

        UpdateMixerValue();
    }

    #region Update Objects
    private void UpdateMixerValue()
    {
        if (volumeActive)
        {
            audioMixer.SetFloat(audioMixerParameter, 20f * Mathf.Log10(slider.value));
        }
        else
        {
            audioMixer.SetFloat(audioMixerParameter, 20f * Mathf.Log10(slider.minValue));
        }

        if (valueTextMesh != null)
        {
            valueTextMesh.text = ((int)(100 * slider.value)).ToString();
        }

        UpdateSliderIcon();
    }

    private void UpdateSliderValue()
    {
        audioMixer.GetFloat(audioMixerParameter, out float mixerValue);

        mixerValue = Mathf.Pow(10, (mixerValue / 20f));

        slider.SetValueWithoutNotify(mixerValue);
        valueTextMesh.text = ((int)(100 * slider.value)).ToString();

        UpdateSliderIcon();
    }

    private void UpdateSliderIcon()
    {
        if (slider == null || !useVolumeIcons) return;

        if (volumeIcon != null)
        {
            Debug.LogWarning("No volume icon referenced.", gameObject);
            return;
        }

        if (volumeActive && slider.value > slider.minValue)
        {
            if (slider.value < 0.33)
            {
                volumeIcon.sprite = lowVolumeIcon;
            }
            else if (slider.value > 0.66)
            {
                volumeIcon.sprite = highVolumeIcon;
            }
            else
            {
                volumeIcon.sprite = mediumVolumeIcon;
            }
        }
        else
        {
            volumeIcon.sprite = noVolumeIcon;
        }
    }
    #endregion Update Objects

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Selection.activeGameObject != gameObject) return;

        if (nameTextMesh != null)
        {
            nameTextMesh.text = sliderName;
        }

        if (audioMixer == null)
        {

            bool test = audioMixer.GetFloat(audioMixerParameter, out float mixerValue);
            if (test)
            {
                Debug.Log("Parameter value : " + mixerValue);
            }
        }

        UpdateSliderIcon();
    }
#endif

    #region Utility
    [ButtonAction("Get References In Children")]
    private void FindReferencesInChildren()
    {
        slider = GetComponentInChildren<Slider>();

        if (useVolumeIcons)
        {
            volumeIcon = GetComponentInChildren<Image>();
        }

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length == 2)
        {
            nameTextMesh = texts[0];
            valueTextMesh = texts[1];
        }
        else if (texts.Length == 1)
        {
            nameTextMesh = texts[0];
        }
    }

    #endregion
}
