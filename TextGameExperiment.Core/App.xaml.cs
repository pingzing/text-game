using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        public static RootNavigationPage NavigationRoot { get; } = new RootNavigationPage();

        public App()
        {
            InitializeComponent();

            NavigationRoot.PushAsync(new BattlePage());
            MainPage = NavigationRoot;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
