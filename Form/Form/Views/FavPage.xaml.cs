using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Form.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FavPage : ItemsPage
	{
		public FavPage ()
		{
			InitializeComponent ();
		}

	    protected override void Close_Clicked(object sender, EventArgs e) { DisplayAlert("Alert", "You have been alerted", "OK"); }
	}
}