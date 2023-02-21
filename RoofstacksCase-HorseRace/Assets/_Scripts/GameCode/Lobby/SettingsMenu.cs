using GameCode.Data;
using GameCode.Hud;
using GameCode.Init;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class SettingsMenu : Menu
    {
        [SerializeField] private TMP_Text _musicValueText;
        [SerializeField] private TMP_Text _soundValueText;
        
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;
        
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;
        
        [SerializeField] private Button _returnButton;

        private AudioMixer _audioMixer;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            var config = GameManager.Instance.GameConfig;
            _audioMixer = config.AudioMixer;
            
            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    menuManager.OpenMenu(MenuType.Title);
                    SetPrefs();
                })
                .AddTo(disposable);

            _musicToggle
                .OnValueChangedAsObservable()
                .Subscribe(isOn =>
                {
                    _musicSlider.interactable = isOn;
                    float value = isOn ? _musicSlider.value : 0;
                    _musicValueText.SetText($"{(int)(value * 100)}");
                    SetMusicValue(value);
                })
                .AddTo(disposable);
            
            _soundToggle
                .OnValueChangedAsObservable()
                .Subscribe(isOn =>
                {
                    _soundSlider.interactable = isOn;
                    float value = isOn ? _soundSlider.value : 0;
                    _soundValueText.SetText($"{(int)(value * 100)}");
                    SetSoundValue(value);
                })
                .AddTo(disposable);

            _musicSlider
                .OnValueChangedAsObservable()
                .Subscribe(value =>
                {
                    value = _musicToggle.isOn ? value : 0;
                    _musicValueText.SetText($"{(int)(value * 100)}");
                    SetMusicValue(value);
                })
                .AddTo(disposable);
            
            _soundSlider
                .OnValueChangedAsObservable()
                .Subscribe(value =>
                {
                    value = _soundToggle.isOn ? value : 0;
                    _soundValueText.SetText($"{(int)(value * 100)}");
                    SetSoundValue(value);
                })
                .AddTo(disposable);
            
            GetPrefs();
        }

        private void SetMusicValue(float value)
        {
            _audioMixer.SetFloat("Music", value == 0 ? -80 : Mathf.Log10(value) * 20);
        }
        
        private void SetSoundValue(float value)
        {
            _audioMixer.SetFloat("Sound", value == 0 ? -80 : Mathf.Log10(value) * 20);
        }

        private void GetPrefs()
        {
            _musicSlider.value = PlayerPrefs.GetFloat("MUSIC_SLIDER", .15f);
            _soundSlider.value = PlayerPrefs.GetFloat("SOUND_SLIDER", .15f);
            
            var musicToggleBool = PlayerPrefs.GetInt("MUSIC_TOGGLE", 1) == 1;
            var soundToggleBool = PlayerPrefs.GetInt("SOUND_TOGGLE", 1) == 1;

            _musicToggle.isOn = musicToggleBool;
            _soundToggle.isOn = soundToggleBool;
            
            SetMusicValue(musicToggleBool ? _musicSlider.value : 0);
            SetSoundValue(soundToggleBool ? _soundSlider.value : 0);
        }
        
        private void SetPrefs()
        {
            var musicToggleInt = _musicToggle.isOn ? 1 : 0;
            var soundToggleInt = _soundToggle.isOn ? 1 : 0;
            PlayerPrefs.SetInt("MUSIC_TOGGLE", musicToggleInt);
            PlayerPrefs.SetInt("SOUND_TOGGLE", soundToggleInt);
            
            PlayerPrefs.SetFloat("MUSIC_SLIDER", _musicSlider.value);
            PlayerPrefs.SetFloat("SOUND_SLIDER", _soundSlider.value);
        }
    }
}
