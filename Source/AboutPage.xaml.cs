using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace Bracketonomy
{
	// The About page displays various pieces of information about Bracketonomy.
	public sealed partial class AboutPage : Page
	{
		public AboutPage()
		{
			InitializeComponent();

			if (FindName("NameTextBlock") is TextBlock nameTextBlock)
			{
				nameTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;
			}
			Package package = Package.Current;
			if (FindName("VersionTextBlock") is TextBlock versionTextBlock)
			{
				versionTextBlock.Text = "Version " + package.Id.Version.Major + "." + package.Id.Version.Minor;
			}
			if (FindName("PublisherHyperlinkButton") is HyperlinkButton publisherHyperlinkButton)
			{
				PublisherHyperlinkButton.Content = package.PublisherDisplayName;
				publisherHyperlinkButton.NavigateUri =
					new System.Uri(package.PublisherDisplayName, System.UriKind.Absolute);
			}
		}
	}
}
