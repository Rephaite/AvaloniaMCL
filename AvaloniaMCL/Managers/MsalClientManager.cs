using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaMCL.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.MsalClient;
using Microsoft.Identity.Client;

namespace AvaloniaMCL.Managers
{
	public static class MsalClientManager
    {
        private static string _clientID = ""; // Klucz! 

        private static IPublicClientApplication _msalApp;

		public static async Task<MSession?> Login()
		{
			_msalApp = await MsalMinecraftLoginHelper.BuildApplicationWithCache(_clientID);
			Launcher.LoginHandler = new LoginHandlerBuilder().WithCachePath($"{Launcher.MinecraftPath}/AvaloniaMCL_session.json").ForJavaEdition().WithMsalOAuth(_msalApp, factory => factory.CreateInteractiveApi()).Build();

			try
			{
				var session = await Launcher.LoginHandler.LoginFromCache();
				return session.GameSession;
			}

			catch
			{
				try
				{
					var session = await Launcher.LoginHandler.LoginFromOAuth();
					return session.GameSession;
				}

				catch
				{
					return null;
				}
			}
		}

		public static async void Logout()
		{
			var accounts = await _msalApp.GetAccountsAsync();
			foreach (var account in accounts) await _msalApp.RemoveAsync(account);
			try { await Launcher.LoginHandler.ClearCache(); } catch { Console.WriteLine("Couldn't clear cache!"); }
			try { File.Delete($"{Launcher.MinecraftPath}/AvaloniaMCL_session.json"); } catch { Console.WriteLine("Couldn't delete cache file!"); }
		}
	}
}