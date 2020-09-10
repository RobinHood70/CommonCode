namespace RobinHood70.CommonCodeTester.ViewModels
{
	using GalaSoft.MvvmLight;
	using RobinHood70.CommonCode;

	public class FourCCViewModel : ViewModelBase
	{
		private string? inputText;
		private string? outputText;

		public string? InputText
		{
			get => this.inputText;
			set
			{
				if (value?.Length == 4)
				{
					this.OutputText = FourCC.HexString(value);
				}

				this.Set(ref this.inputText, value, nameof(this.inputText));
			}
		}

		public string? OutputText
		{
			get => this.outputText;
			set => this.Set(ref this.outputText, value, nameof(this.outputText));
		}
	}
}
