using CrossX.Abstractions.IO;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossX.WindowsForms.Services
{
    internal class SelectFileServiceWinForms : ISelectFileService
    {
        public Task<string> GetOpenFilePath(string message, string filters, Environment.SpecialFolder initialFolder = Environment.SpecialFolder.MyComputer)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = message,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = filters,
                FilterIndex = 0,
                RestoreDirectory = true,
                InitialDirectory = Environment.GetFolderPath(initialFolder)
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return Task.FromResult(ofd.FileName);
            }
            return Task.FromResult((string)null);
        }

        public Task<string> GetSaveFilePath(string message, string filters, Environment.SpecialFolder initialFolder = Environment.SpecialFolder.MyComputer)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = message,
                Filter = filters,
                CheckPathExists = true,
                FilterIndex = 0,
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(initialFolder),
                RestoreDirectory = true,
                AutoUpgradeEnabled = true,
                AddExtension = true,
                ValidateNames = true
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return Task.FromResult(sfd.FileName);
            }
            return Task.FromResult((string)null);
        }

        public Task<string> SelectFolder(string message, Environment.SpecialFolder initialFolder = Environment.SpecialFolder.MyComputer)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog
            {
                RootFolder = initialFolder,
                AutoUpgradeEnabled = true,
                ShowNewFolderButton = true,
                Description = message,
                UseDescriptionForTitle = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return Task.FromResult(ofd.SelectedPath);
            }

            return Task.FromResult((string)null);
        }
    }
}
