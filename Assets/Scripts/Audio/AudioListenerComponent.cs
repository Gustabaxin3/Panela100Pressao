using AUDIO;
using UnityEngine;

public class AudioListenerComponent : MonoBehaviour {
    private void OnEnable() {
        AudioEvents.OnPlayerJump += PlayPlayerJumpSound;
        AudioEvents.OnBackgroundMusic += PlayBackgroundMusic;
        AudioEvents.OnPlayerInsideZipline += PlayZiplineEnter;
        AudioEvents.OnPlayerOutZipline += PlayZiplineExit;
        AudioEvents.OnPlayerRope += PlayRopeSound;

        AudioEvents.OnUIButton += PlayUIButtonSound;
        AudioEvents.OnPlayerFootstep += PlayPlayerFootstepSound;
        AudioEvents.OnObjectDragging += PlayObjectDraggingSound;
        AudioEvents.OnZipline += PlayZiplineSound;

        AudioEvents.OnRespectCaptain += PlayRespectCaptainSound;
        AudioEvents.OnPushGrunt += PlayPushGruntSound;

        AudioEvents.OnJumpGroundExit += PlayJumpGroundExitSound;
        AudioEvents.OnJumpVoice += PlayJumpVoiceSound;
        AudioEvents.OnJumpLand += PlayJumpLandSound;
    }

    private void OnDisable() {
        AudioEvents.OnPlayerJump -= PlayPlayerJumpSound;
        AudioEvents.OnBackgroundMusic -= PlayBackgroundMusic;
        AudioEvents.OnPlayerInsideZipline -= PlayZiplineEnter;
        AudioEvents.OnPlayerOutZipline -= PlayZiplineExit;
        AudioEvents.OnPlayerRope -= PlayRopeSound;

        AudioEvents.OnUIButton -= PlayUIButtonSound;
        AudioEvents.OnPlayerFootstep -= PlayPlayerFootstepSound;
        AudioEvents.OnObjectDragging -= PlayObjectDraggingSound;
        AudioEvents.OnZipline -= PlayZiplineSound;

        AudioEvents.OnRespectCaptain -= PlayRespectCaptainSound;
        AudioEvents.OnPushGrunt -= PlayPushGruntSound;

        AudioEvents.OnJumpGroundExit -= PlayJumpGroundExitSound;
        AudioEvents.OnJumpVoice -= PlayJumpVoiceSound;
        AudioEvents.OnJumpLand -= PlayJumpLandSound;
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

    private void PlayBackgroundMusic() => AudioManager.Instance.PlayTrack("Audio/Musica/Background");
    private void PlayZiplineEnter() => AudioManager.Instance.PlaySoundEffect("Audio/Tirolesa/Entrada");
    private void PlayZiplineExit() => AudioManager.Instance.PlaySoundEffect("Audio/Tirolesa/Saida");
    private void PlayRopeSound() => AudioManager.Instance.PlaySoundEffect("Audio/Corda/Pegar");
    private void PlayUIButtonSound() => AudioManager.Instance.PlaySoundEffect("Audio/UI/Botao");
    private void PlayPlayerFootstepSound() => AudioManager.Instance.PlaySoundEffect("Audio/Passos/Passo");
    private void PlayObjectDraggingSound() => AudioManager.Instance.PlaySoundEffect("Audio/Arrastar/Objeto");
    private void PlayZiplineSound() => AudioManager.Instance.PlaySoundEffect("Audio/Tirolesa/Andando");
    private void PlayRespectCaptainSound() => AudioManager.Instance.PlaySoundEffect("Audio/Capitao/Respeitar");
    private void PlayPushGruntSound() => AudioManager.Instance.PlaySoundEffect("Audio/Grunhido/Empurrar");
    private void PlayJumpGroundExitSound() => AudioManager.Instance.PlaySoundEffect("Audio/Pulo/SairChao");
    private void PlayJumpVoiceSound() => AudioManager.Instance.PlaySoundEffect("Audio/Pulo/Grunhido");
    private void PlayJumpLandSound() => AudioManager.Instance.PlaySoundEffect("Audio/Pulo/Pouso");
}
