using BattleTextTokenizer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TextGameExperiment.Core.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BattleTextLabel : ContentView
    {
        private TimerExt _characterTickTimer;
        private ConcurrentQueue<TextToken> _tokenBuffer = new ConcurrentQueue<TextToken>();

        public BattleTextLabel()
        {
            InitializeComponent();
            _characterTickTimer = new TimerExt(CharacterTimerTick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(30));            
        }

        private void CharacterTimerTick(object state)
        {
            bool didReadChar = _tokenBuffer.TryDequeue(out TextToken readToken);
            if (didReadChar)
            {
                Device.BeginInvokeOnMainThread(() => ParseToken(readToken));
            }
            else
            {
                _characterTickTimer.Stop();
            }
        }

        private void ParseToken(TextToken readToken)
        {
            switch (readToken.TokenType)
            {
                case TokenType.ChangeTiming:
                    int newTime = Convert.ToInt32(readToken.Value);
                    _characterTickTimer.ChangeInterval(newTime);
                    break;
                case TokenType.Pause:
                    int pauseLength = Convert.ToInt32(readToken.Value);
                    _characterTickTimer.Delay(pauseLength);
                    break;
                case TokenType.String:                    
                case TokenType.Character:
                    BackingLabel.Text += readToken.Value;                    
                    break;
            }
        }

        public void ClearBattleText()
        {
            BackingLabel.Text = "";
        }

        public void QueueBattleText(IEnumerable<TextToken> tokens)
        {
            foreach(TextToken token in tokens)
            {
                _tokenBuffer.Enqueue(token);
            }
            _characterTickTimer.Start();
        }
    }
}