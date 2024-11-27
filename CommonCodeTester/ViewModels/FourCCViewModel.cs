namespace RobinHood70.CommonCodeTester.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using RobinHood70.CommonCode;

public class FourCCViewModel : ObservableRecipient
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

			this.SetProperty(ref this.inputText, value, nameof(this.inputText));
		}
	}

	public string? OutputText
	{
		get => this.outputText;
		set => this.SetProperty(ref this.outputText, value, nameof(this.outputText));
	}
}