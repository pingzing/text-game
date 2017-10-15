using BattleTextTokenizer;
using BattleTextTokenizer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TextGameExperiment.Core.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core.Controls
{
    public enum DialogueState
    {
        Halted,
        Writing,
        Stopped
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DialogueLabel : ContentView
    {
        private TimerExt _characterTickTimer;
        private ConcurrentQueue<TextToken> _tokenBuffer = new ConcurrentQueue<TextToken>();
        private TextToken _mostRecentToken = null;
        private int _currentPageIndex = 0;
        private double _currentPageMaxHeight = 0;
        private int _finalPageIndex = 0;

        // After we consume a $(stop) token, set this flag to true.
        // While it's set, we'll force scroll through any automatic page breaks.
        // It gets set back to false as soon we run print an actual, printable character to screen
        // again.
        // This is so you don't have to mash "next" if a $(stop) token shows up next to a bunch
        // of whitespace characters right at a page break.
        private bool _justConsumedStop = false;

        public Tokenizer BattleTokenizer { get; set; }

        public DialogueLabel()
        {
            InitializeComponent();
            _characterTickTimer = new TimerExt(CharacterTimerTick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(30));
        }

        private void CharacterTimerTick(object state)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                bool didReadChar = _tokenBuffer.TryDequeue(out TextToken readToken);
                if (didReadChar)
                {
                    ParseToken(readToken);
                }
                else
                {
                    BattleTextState = DialogueState.Halted;
                    _characterTickTimer.Stop();
                }
            });
        }

        private void ParseToken(TextToken readToken)
        {
            switch (readToken.TokenType)
            {
                case TokenType.ChangeTiming:
                    int newTime = Convert.ToInt32(readToken.Value);
                    _characterTickTimer.ChangeInterval(newTime);
                    BattleTextState = DialogueState.Writing;
                    break;
                case TokenType.Pause:
                    int pauseLength = Convert.ToInt32(readToken.Value);
                    _characterTickTimer.Delay(pauseLength);
                    BattleTextState = DialogueState.Writing;
                    break;
                case TokenType.Stop:
                    _characterTickTimer.Stop();
                    _justConsumedStop = true;
                    BattleTextState = DialogueState.Halted;
                    break;                
                case TokenType.String:
                case TokenType.Character:
                    BackingLabel.Text += readToken.Value;
                    if (BackingLabel.Height > _currentPageMaxHeight)
                    {
                        if (_justConsumedStop)
                        {
                            PageDown();
                            BattleTextState = DialogueState.Writing;
                            break;
                        }
                        else
                        {
                            _characterTickTimer.Stop();
                            BattleTextState = DialogueState.Halted;
                            break;
                        }
                    }
                    if (_mostRecentToken?.IsPrintable == true && !String.IsNullOrWhiteSpace(_mostRecentToken?.Value))
                    {
                        _justConsumedStop = false;
                    }
                    BattleTextState = DialogueState.Writing;
                    break;
                default:
                    Debug.WriteLine($"Found unhandled token: {readToken.Value} of type: {readToken.TokenType}. Skipping...");
                    break;
            }
            Debug.WriteLine("Parsed: " + readToken.Value);
            _mostRecentToken = readToken;
        }

        public async Task Next()
        {
            // TODO: smarter check here, maybe?
            if (BackingLabel.Height > _currentPageMaxHeight)
            {
                await PageDown();
            }
            _characterTickTimer.Start();
        }

        public void ResetBattleText()
        {
            _characterTickTimer.Stop();
            _tokenBuffer = new ConcurrentQueue<TextToken>();

            PioneerLabel.Text = "";
            BackingLabel.Text = "";
            _currentPageIndex = 0;            
            Scroller.ScrollToAsync(0, 0, false);
        }

        public async Task PageUp()
        {
            if (_currentPageIndex <= 0)
            {
                return;
            }
            _currentPageIndex--;
            _currentPageMaxHeight = (_currentPageIndex + 1) * Scroller.Height;
            await Scroller.ScrollToAsync(0, Scroller.Height * _currentPageIndex, true);
        }

        public async Task PageDown()
        {
            if (_currentPageIndex >= _finalPageIndex)
            {
                return;
            }
            _currentPageIndex++;
            _currentPageMaxHeight = (_currentPageIndex + 1) * Scroller.Height;
            await Scroller.ScrollToAsync(0, Scroller.Height * _currentPageIndex, true);
        }

        public void QueueBattleText(string text)
        {
            /* ------------------------------------------------------------------------------
             * This works in two stages: First, we take the text, and strip it of all tokens.
             * Then, we take the stripped text and display it in a "PioneerLabel". We do this
             * so that the ScrollViewer containing the text has the correct dimensions BEFORE
             * we begin writing text char-by-char.
             * 
             * Then, we take the current size of the dialog window and divide it up into pages.
             * Finally, we tokenize the text, fill the text buffer, and start the timer that 
             * drains the buffer.
             * --------------------------------------------------------------------------------*/
            string printableText = BattleTokenizer.StripUnprintableTokens(text);
            PioneerLabel.Text += printableText;
            double scrollViewportHeight = Scroller.Height;
            _currentPageMaxHeight = (_currentPageIndex + 1) * scrollViewportHeight;
            double totalTextHeight = Scroller.ContentSize.Height;
            _finalPageIndex = (int)Math.Ceiling(totalTextHeight / scrollViewportHeight) + 1;            

            foreach (TextToken token in BattleTokenizer.TokenizeText(text))
            {
                _tokenBuffer.Enqueue(token);
            }

            _characterTickTimer.Start();
        }       

        private static readonly BindablePropertyKey BattleTextStateKey = BindableProperty.CreateReadOnly
        (
           nameof(BattleTextState),
           typeof(DialogueState),
           typeof(DialogueLabel),
           DialogueState.Halted
        );

        public DialogueState BattleTextState
        {
            get => (DialogueState)GetValue(BattleTextStateProperty);
            private set => SetValue(BattleTextStateKey, value);
        }

        public static readonly BindableProperty BattleTextStateProperty = BattleTextStateKey.BindableProperty;
    }
}