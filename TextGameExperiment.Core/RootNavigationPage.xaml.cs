using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootNavigationPage : NavigationPage
    {
        public RootNavigationPage()
        {
            InitializeComponent();            
        }
    }
}