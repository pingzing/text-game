using BattleTextTokenizer;
using BattleTextTokenizer.Models;
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
    public partial class BattlePage : ContentPage
    {
        private Tokenizer _battleTokenizer;
        private readonly string NL = Environment.NewLine;
        private readonly string TestText;        

        public BattlePage()
        {
            InitializeComponent();       
            TestText = $"And just$(pause 350) $(str)WHERE$(str)$(pause 150) do you think you're going, $(pause 250)$(str)young$(str)$(pause 500)$(str) man?$(str)" +
                       $"\n$(chartime 130)...well, uh...$(chartime 1)$(pause 1000)work?$(pause 1500)" +
                       $"\n$(chartime 30)This is a long string that's operating at 30ms, so we can see it play it on a looooong string." +
                       $"\n$(chartime 15)This is a long string that's operating at 15ms, so we can see it play it on a looooong string." +                       
                       $"$(chartime 30)";
            _battleTokenizer = new Tokenizer(new List<TokenDefinition>
            {
                new TokenDefinition(TokenType.ChangeTiming, @"\$\(chartime (\d+)\)", 1), //Anything that looks like "text text text$(chartime 5)" where '5' can be any whole integer number
                new TokenDefinition(TokenType.Pause, @"\$\(pause (\d+)\)", 1), // Anything that looks like "text text text$(pause 500) text text", where '500' can be any whole integer number
                new TokenDefinition(TokenType.String, @"(\$\(str\))(.*?)(\$\(str\))", 1), // Anything bracketed by $(str), i.e. $(str)some text here$(str)
                new TokenDefinition(TokenType.Character, ".", 2) // Any single character
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(500);
            NarrationBox.QueueBattleText(
                _battleTokenizer.TokenizeText(TestText));
        }

        private void Button1_Clicked(object sender, EventArgs args)
        {
            NarrationBox.ClearBattleText();
            NarrationBox.QueueBattleText(
                _battleTokenizer.TokenizeText(TestText));
        }
    }
}
