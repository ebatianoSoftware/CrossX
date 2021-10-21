using CrossX.Framework.Input.Gamepad;

namespace CrossX.WindowsForms.Input
{

    internal class OpenTkGamePads : IGamePads
    {
        private readonly OpenTkGamePadInfo[] gamepadInfos = new OpenTkGamePadInfo[4];

        public OpenTkGamePads()
        {
            for(var idx =0; idx < gamepadInfos.Length; ++idx)
            {
                gamepadInfos[idx] = new OpenTkGamePadInfo(idx);
            }
        }

        public GamepadState GetState(int index) => gamepadInfos[index].GetState();

        public void Update()
        {
            for (var idx = 0; idx < gamepadInfos.Length; ++idx)
            {
                gamepadInfos[idx].Update();
            }
        }
    }
}
