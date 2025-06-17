using AUDIO;
using UnityEngine;

public class AudioListenerComponent : MonoBehaviour {
    private void OnEnable() {
        AudioEvents.OnPlayerJump += PlayPlayerJumpSound;
        AudioEvents.OnBackgroundMusic += PlayBackgroundMusic;
        AudioEvents.OnPlayerInsideZipline += PlayZiplineEnter;
        AudioEvents.OnPlayerOutZipline += PlayZiplineExit;
        AudioEvents.OnPlayerRope += PlayRopeSound;
    }

    private void OnDisable() {
        AudioEvents.OnPlayerJump -= PlayPlayerJumpSound;
        AudioEvents.OnBackgroundMusic -= PlayBackgroundMusic;
        AudioEvents.OnPlayerInsideZipline -= PlayZiplineEnter;
        AudioEvents.OnPlayerOutZipline -= PlayZiplineExit;
        AudioEvents.OnPlayerRope -= PlayRopeSound;
    }

    private void PlayPlayerJumpSound() {
        string[] jumpSounds = {
            "Audio/Pulo/SoldadoPulo01",
            "Audio/Pulo/SoldadoPulo02",
            "Audio/Pulo/SoldadoPulo03",
            "Audio/Pulo/SoldadoPulo04"
        };

        int index = Random.Range(0, jumpSounds.Length);

        AudioManager.Instance.PlaySoundEffect(jumpSounds[index]);
    }

    private void PlayBackgroundMusic() {
        AudioManager.Instance.PlayTrack("Audio/Musica/Background");
    }

    private void PlayZiplineEnter() {
        AudioManager.Instance.PlaySoundEffect("Audio/Tirolesa/Entrada");
    }

    private void PlayZiplineExit() {
        AudioManager.Instance.PlaySoundEffect("Audio/Tirolesa/Saida");
    }

    private void PlayRopeSound() {
        AudioManager.Instance.PlaySoundEffect("Audio/Corda/Pegar");
    }
}
