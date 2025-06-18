using System;
namespace AUDIO {
    public static class AudioEvents {
        public static Action OnPlayerJump;
        public static Action OnBackgroundMusic;
        public static Action OnPlayerInsideZipline;
        public static Action OnPlayerOutZipline;
        public static Action OnPlayerRope;

        public static Action OnUIButton;
        public static Action OnPlayerFootstep;
        public static Action OnObjectDragging;
        public static Action OnZipline;

        public static Action OnRespectCaptain;
        public static Action OnPushGrunt; 

        public static Action OnJumpGroundExit;
        public static Action OnJumpVoice;
        public static Action OnJumpLand;
    }
}