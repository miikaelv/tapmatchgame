using System;
using UnityEngine;
using UnityEngine.UI;

namespace TapMatch.UnityServices
{
    public interface IInputService
    {
        public InputBlock BlockInputInScope();
    }

    public class InputService : MonoBehaviour, IInputService
    {
        public GraphicRaycaster UIRootRaycaster;
        private bool InputBlocked;

        private void SetInputBlocking(bool enable)
        {
            UIRootRaycaster.enabled = !enable;
            InputBlocked = enable;
        }

        // TradeOff: Does not currently handle overlapping input blocks due to simple implementation
        // Blocks all UI Input within scope called
        public InputBlock BlockInputInScope()
        {
            return InputBlocked
                ? throw new Exception("InputBlocking is already set")
                : new InputBlock(SetInputBlocking);
        }
    }

    public sealed class InputBlock : IDisposable
    {
        private readonly Action<bool> BlockInput;

        public InputBlock(Action<bool> blockInput)
        {
            BlockInput = blockInput;
            BlockInput?.Invoke(true);
        }

        public void Dispose()
        {
            BlockInput?.Invoke(false);
        }
    }
}