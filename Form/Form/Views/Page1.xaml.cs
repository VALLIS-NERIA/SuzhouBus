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
	public partial class Page1 : NavigationPage
	{
		public Page1 ()
		{
			InitializeComponent ();
		}
        public Page1(Page another):base(another) {
			InitializeComponent();
        }
    }
}