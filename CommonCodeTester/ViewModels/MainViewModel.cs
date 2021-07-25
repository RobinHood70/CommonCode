namespace RobinHood70.CommonCodeTester.ViewModels
{
	using System.Windows.Input;
	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Command;
	using RobinHood70.CommonCodeTester.Models;

	public class MainViewModel : ViewModelBase
	{
		#region Constructors
		public MainViewModel() => TestRunner.Initialize();
		#endregion

		#region Public Static Properties
		public static ICommand OpenFourCC => new RelayCommand(TestRunner.OpenFourCC);

		public static ICommand TestReaderCommand => new RelayCommand(TestRunner.RunTests);
		#endregion
	}
}