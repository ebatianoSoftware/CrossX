using System;
using System.Threading.Tasks;

namespace CrossX.Abstractions.IO
{
    public interface ISelectFileService
    {
        Task<string> GetOpenFilePath(string message, string filters, Environment.SpecialFolder initialFolder = Environment.SpecialFolder.MyComputer);
        Task<string> GetSaveFilePath(string message, string filters, Environment.SpecialFolder initialFolder = Environment.SpecialFolder.MyComputer);

        Task<string> SelectFolder(string message, Environment.SpecialFolder initialFolder = Environment.SpecialFolder.MyComputer);
    }
}
