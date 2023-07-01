using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AvaloniaMCL.Managers
{
	public static class RandomManager
	{
		public static Bitmap RandomCover()
		{
			IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
			Uri uri = new Uri($"avares://AvaloniaMCL/Assets/cover{new Random().Next(1, 5)}.png");
			return new Bitmap(assets.Open(uri));
		}
	}
}