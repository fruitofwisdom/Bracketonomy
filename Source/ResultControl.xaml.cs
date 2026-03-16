using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// This custom control allows for a uniform presentation of results that look like a normal
// TextBox, but behave much more like a fancy TextBlock. The User Control item template is
// documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Bracketonomy
{
	public sealed partial class ResultControl : UserControl
	{
		public static readonly DependencyProperty ResultProperty =
			DependencyProperty.Register("Result", typeof(Result), typeof(ResultControl),
			new PropertyMetadata(null, new PropertyChangedCallback(OnResultChanged)));
		public Result Result
		{
			get => (Result)GetValue(ResultProperty);
			set => SetValue(ResultProperty, value);
		}

		public ResultControl()
		{
			this.InitializeComponent();
		}

		private static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResultControl control = d as ResultControl;
			TextBlock seedTextBlock = control.FindName("SeedText") as TextBlock;
			TextBlock teamTextBlock = control.FindName("TeamText") as TextBlock;
			TextBlock scoreTextBlock = control.FindName("ScoreText") as TextBlock;

			// If no team is in the results, then no results have been calculated yet.
			if (control.Result.Team == null)
			{
				seedTextBlock.Text = "";
				teamTextBlock.Text = "";
				scoreTextBlock.Text = "";
			}
			else
			{
				seedTextBlock.Text = control.Result.Seed.ToString();
				teamTextBlock.Text = control.Result.Team;
				// The actual championship result has no score.
				if (control.Result.Score == 0)
				{
					scoreTextBlock.Text = "";
				}
				else
				{
					scoreTextBlock.Text = control.Result.Score.ToString();
				}
				if (control.Result.Winner)
				{
					seedTextBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
					teamTextBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
					scoreTextBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
				}
				else
				{
					seedTextBlock.FontWeight = Windows.UI.Text.FontWeights.Normal;
					teamTextBlock.FontWeight = Windows.UI.Text.FontWeights.Normal;
					scoreTextBlock.FontWeight = Windows.UI.Text.FontWeights.Normal;
				}
			}
		}
	}
}
