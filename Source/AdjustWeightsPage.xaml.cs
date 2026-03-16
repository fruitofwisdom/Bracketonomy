using System;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace Bracketonomy
{
	// This page allows for the adjusting of weights by binding each slider to a public field in
	// the static Weights class. As the slider values are changed, the tournament is re-simulated.
	public sealed partial class AdjustWeightsPage : Page
	{
		private MainPage MainPage;

		private readonly Timer SimulateTimer;

		public AdjustWeightsPage()
		{
			InitializeComponent();

			// Create a Timer to simulate the tournament, but don't start it yet.
			SimulateTimer = new Timer(WeightChanged, null, 0, 0);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter is MainPage)
			{
				MainPage = e.Parameter as MainPage;
			}
			base.OnNavigatedTo(e);
		}

		private void SliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			// Trigger our timer to run half a second from the last slider change, since the
			// binding weight hasn't been updated yet.
			SimulateTimer.Change(500, 0);
		}

		async private void WeightChanged(object stateInfo)
		{
			// Simulate the tournament, but on the UI thread.
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				MainPage?.SimulateTournament();
			});
		}
	}

	// Convert a double to a string (and back) with the formatting we desire.
	public class WeightFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return string.Format("{0:N2}", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			// Return 0.0 if we couldn't parse the value.
			double.TryParse((string)value, out double weightAsDouble);
			return weightAsDouble;
		}
	}
}
