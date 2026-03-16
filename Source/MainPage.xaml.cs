using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Bracketonomy
{
	// The main page for tournament results and interacting with the simulation.
	public sealed partial class MainPage : Page
	{
		private AppWindow AdjustWeightsWindow;

		private readonly TeamData Data = new TeamData();
		public TournamentResults Results = new TournamentResults();

		public MainPage()
		{
			InitializeComponent();

			// Attempt to set a preferred size for ourselves.
			ApplicationView.PreferredLaunchViewSize = new Size(1250, 750);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
		}

		private void QuitClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Exit();
		}

		private async void OpenTeamDataClick(object sender, RoutedEventArgs e)
		{
			FileOpenPicker picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			picker.FileTypeFilter.Add(".csv");

			StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				StorageApplicationPermissions.FutureAccessList.Add(file);
				await Data.ReadFile(file);

				if (Data.Teams.Count > 0)
				{
					ApplicationView.GetForCurrentView().Title = file.DisplayName;
					Results.Populate(Data);
					SimulateTournament();
				}
			}
			else
			{
				// Do something else?
			}
		}

		private async void AdjustWeightsClick(object sender, RoutedEventArgs e)
		{
			// Only allow one adjust weights page at a time.
			if (AdjustWeightsWindow == null)
			{
				AdjustWeightsWindow = await AppWindow.TryCreateAsync();

				Frame adjustWeightsFrame = new Frame();
				adjustWeightsFrame.Navigate(typeof(AdjustWeightsPage), this);
				ElementCompositionPreview.SetAppWindowContent(AdjustWeightsWindow, adjustWeightsFrame);
				
				// Finish setting up the window.
				AdjustWeightsWindow.RequestSize(new Size(500, 1185));
				AdjustWeightsWindow.Title = "Adjust Weights";
				AdjustWeightsWindow.Closed += delegate
				{
					adjustWeightsFrame.Content = null;
					AdjustWeightsWindow = null;
				};
			}

			await AdjustWeightsWindow.TryShowAsync();
		}

		public void SimulateTournament()
		{
			if (Results.Results.Count > 0)
			{
				Results.SimulateTournament(Data);
			}
		}

		private async void AboutClick(object sender, RoutedEventArgs e)
		{
			AppWindow aboutPageWindow = await AppWindow.TryCreateAsync();

			Frame aboutPageFrame = new Frame();
			aboutPageFrame.Navigate(typeof(AboutPage));
			ElementCompositionPreview.SetAppWindowContent(aboutPageWindow, aboutPageFrame);

			// Finish setting up the window.
			aboutPageWindow.RequestSize(new Size(500, 300));
			aboutPageWindow.Title = "About";
			aboutPageWindow.Closed += delegate
			{
				aboutPageFrame.Content = null;
				aboutPageWindow = null;
			};

			await aboutPageWindow.TryShowAsync();
		}
	}
}
