using BattleTextTokenizer;
using BattleTextTokenizer.Models;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BattlePage : ContentPage
    {
        private const string DefaultRulesPath = "Assets.BattleTextRules.txt";        

        private Tokenizer _battleTokenizer = new Tokenizer();

        public BattlePage()
        {
            InitializeComponent();                   

            // Load default tokenizer rules.
            Assembly assembly = typeof(BattlePage).GetTypeInfo().Assembly;
            string resourceName = $"{assembly.GetName().Name}.{DefaultRulesPath}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.WriteLine($"WARNING: Unable to laod default tokenizer from assembly at {resourceName}");
                }
                else
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();
                        LoadTokenizerRuleString(result);
                    }
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            NarrationBox.BattleTokenizer = _battleTokenizer; 
        }

        private bool isFirst = true;
        private async void Button1_Clicked(object sender, EventArgs args)
        {
            if (isFirst)
            {
                FileData file = await CrossFilePicker.Current.PickFile();
                if (file != null)
                {
                    isFirst = false;
                    string fileString = Encoding.UTF8.GetString(file.DataArray);
                    NarrationBox.QueueBattleText(fileString);
                }
            }
            NarrationBox.Next();
        }

        private void GoUp_Clicked(object sender, EventArgs args)
        {
            NarrationBox.PageUp();
        }

        private void GoDown_Clicked(object sender, EventArgs args)
        {
            NarrationBox.PageDown();
        }

        private async void LoadRules_Clicked(object sender, EventArgs args)
        {
            FileData file = await CrossFilePicker.Current.PickFile();
            if (file != null)
            {
                string fileString = Encoding.UTF8.GetString(file.DataArray);
                fileString = fileString.Replace(@"\\", @"\"); //We seem to get an extra set of backslashes somewhere, so get rid of those
                LoadTokenizerRuleString(fileString);
            }
        }

        private void LoadTokenizerRuleString(string rulesString)
        {
            rulesString = rulesString.Replace(@"\\", @"\");
            _battleTokenizer.TokenDefinitions = RulesParser.ParseFromString(rulesString).ToList();
        }
    }
}
