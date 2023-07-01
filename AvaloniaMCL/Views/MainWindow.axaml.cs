using System.Linq;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using AvaloniaMCL.Core;
using AvaloniaMCL.Managers;
using AvaloniaMCL.ViewModels;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using ReactiveUI;
using Version = AvaloniaMCL.Models.Version;

namespace AvaloniaMCL.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public MainWindow()
		{
			System.Net.ServicePointManager.DefaultConnectionLimit = 512;

			InitializeComponent();

			this.WhenActivated(d => d(ViewModel!.ShowSettings.RegisterHandler(ShowSettingsAsync)));
			this.WhenActivated(d => d(ViewModel!.ShowAccount.RegisterHandler(ShowAccountAsync)));

			Cover.Source = RandomManager.RandomCover();
			try { VersionList.SelectedItem = Launcher.VersionList.Where(x => x.Id == ConfigManager.Config.LastVersionUsed).First(); }
			catch { VersionList.SelectedItem = null; }
		}

		private async void PlayClicked(object? sender, RoutedEventArgs e)
		{
			if (VersionList.SelectedItem == null) return;

			if (!Launcher.IsLoggedIn) Username.IsEnabled = false;
			VersionList.IsEnabled = false;
			PlayButton.IsEnabled = false;

			Version selectedVersion = (Version)VersionList.SelectedItem;
			try
			{
				try
				{
					await Launcher.CmLauncher.CheckAndDownloadAsync(await Launcher.CmLauncher.GetVersionAsync(selectedVersion.Id));
					VersionManager.UpdateVersion(selectedVersion);
				}

				catch { }

				ProgressBar.Maximum = 1;
				ProgressBar.Value = 1;
				ProgressBar.ProgressTextFormat = "Launching...";

				if (!Launcher.CheckUsername(Username.Text)) return;

				MVersion startVersion = VersionManager.MVersionList.GetVersion(selectedVersion.Id);
				MLaunchOption launchOptions = new() { Path = Launcher.CmLauncher.MinecraftPath, StartVersion = startVersion, ServerPort = 25565, ScreenWidth = 856, ScreenHeight = 482, MaximumRamMb = ConfigManager.Config.Ram };
				if (Launcher.IsLoggedIn) launchOptions.Session = Launcher.Session;
				else launchOptions.Session = MSession.GetOfflineSession(Username.Text);

				ConfigManager.Config.Username = Username.Text;
				ConfigManager.Config.LastVersionUsed = selectedVersion.Id;
				ConfigManager.SaveConfig();

				var minecraft = await Launcher.CmLauncher.CreateProcessAsync(launchOptions);
				minecraft.Start();

				ProgressBar.ProgressTextFormat = "Done!";
			}

			catch
			{
				ProgressBar.Background = Brush.Parse("#8a1a1a");
				ProgressBar.ProgressTextFormat = $"Couldn't launch {selectedVersion.Id}!";
			}

			await Task.Delay(3000);

			if (!Launcher.IsLoggedIn) Username.IsEnabled = true;
			VersionList.IsEnabled = true;
			PlayButton.IsEnabled = true;
			ProgressBar.Background = Brush.Parse("#141414");
			ProgressBar.ProgressTextFormat = "";
			ProgressBar.Value = 0;
		}

		private async Task ShowSettingsAsync(InteractionContext<SettingsViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new SettingsWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);
		}

		private async Task ShowAccountAsync(InteractionContext<AccountViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new AccountWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);
		}
	}
}