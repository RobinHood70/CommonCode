namespace RobinHood70.CommonCodeTester.ViewModels
{
	using System.Diagnostics;
	using RobinHood70.CommonCode;
	using RobinHood70.CommonCodeTester.Models;

	public class FourCCViewModel : Notifier
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

				Debug.WriteLine(this.OutputText);
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
