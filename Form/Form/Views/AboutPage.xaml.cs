using System;
using Form.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Form.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			InitializeComponent ();
		    this.BindingContext = new AboutViewModel();
		}
	}
}