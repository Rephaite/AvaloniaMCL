using AvaloniaMCL.Managers;
using AvaloniaMCL.Models;
using ReactiveUI;

namespace AvaloniaMCL.ViewModels
{
	public class ViewModelBase : ReactiveObject
	{
		public Config Config => ConfigManager.Config;
	}
}