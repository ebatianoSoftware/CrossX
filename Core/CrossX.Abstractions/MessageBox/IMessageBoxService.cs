using System;
using System.Threading.Tasks;

namespace CrossX.Abstractions.MessageBox
{
    [Flags]
    public enum MessageBoxResult
    {
        OK = 0x01,
        Cancel = 0x02,
        Yes = 0x04,
        No = 0x08
    }

    public interface IMessageBoxService
    {
        Task<MessageBoxResult> ShowMessageBox(string message, string caption, MessageBoxResult extectedResults);
    }
}
