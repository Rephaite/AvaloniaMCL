using AvaloniaMCL.Core;
using ReactiveUI;

namespace AvaloniaMCL.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public int PhysicalMemory
		{
			get => Launcher.PhysicalMemory;
			set => this.RaiseAndSetIfChanged(ref Launcher.PhysicalMemory, value);
		}
	}
}