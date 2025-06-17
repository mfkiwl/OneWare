using Avalonia.Controls;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace OneWare.Settings.ViewModels.SettingTypes
{
    public class ListBoxSettingViewModel : TitledSettingViewModel
    {
        private readonly IWindowService _windowService;
        private int _selectedIndex;

        public ListBoxSettingViewModel(ListBoxSetting setting, IWindowService windowService) : base(setting)
        {
            Setting = setting;
            _windowService = windowService;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public new ListBoxSetting Setting { get; }

        public async Task AddNewAsync(Control owner)
        {
            var result = await _windowService.ShowInputAsync(
                "Add",
                "Add a new value to the list",
                MessageBoxIcon.Info,
                null,
                TopLevel.GetTopLevel(owner) as Window);

            if (!string.IsNullOrWhiteSpace(result))
                Setting.Items.Add(result);
        }

        public async Task EditSelectedAsync(Control owner)
        {
            if (SelectedIndex < 0 || SelectedIndex >= Setting.Items.Count)
                return;

            var current = Setting.Items[SelectedIndex];
            var result = await _windowService.ShowInputAsync(
                "Edit",
                "Edit the value",
                MessageBoxIcon.Info,
                current,
                TopLevel.GetTopLevel(owner) as Window);

            if (!string.IsNullOrWhiteSpace(result))
                Setting.Items[SelectedIndex] = result;
        }

        public void RemoveSelected()
        {
            if (SelectedIndex < 0 || SelectedIndex >= Setting.Items.Count)
                return;

            Setting.Items.RemoveAt(SelectedIndex);
        }
    }
}
