using BattleTextTokenizer;
using BattleTextTokenizer.Models;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TextGameExperiment.Core.Models;
using TextGameExperiment.Core.Models.Graph;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BattlePage : ContentPage
    {
        private const string DefaultRulesPath = "Assets.BattleTextRules.txt";        

        private Tokenizer _battleTokenizer = new Tokenizer();
        private Graph<Room> _roomGraph;

        private GraphNode<Room> _currentRoom;
        public GraphNode<Room> CurrentRoom
        {
            get { return _currentRoom; }
            set
            {
                if (_currentRoom != value)
                {
                    _currentRoom = value;
                    OnPropertyChanged(nameof(CurrentRoom));
                    AvailableDestinations.Clear();
                    foreach (var roomNode in _currentRoom.Neighbors)
                    {
                        AvailableDestinations.Add(roomNode.Value);
                    }
                    string roomText = LoadTextFileFromResource(_currentRoom.Value.RelativeDialoguePath);
                    NarrationBox.ClearBattleText();
                    NarrationBox.QueueBattleText(roomText);
                }
            }
        }

        public ObservableCollection<Room> AvailableDestinations { get; set; } = new ObservableCollection<Room>();


        public BattlePage()
        {
            InitializeComponent();

            // Load default tokenizer rules.
            string tokenizerRulesContent = LoadTextFileFromResource(DefaultRulesPath);
            if (tokenizerRulesContent != null)
            {
                LoadTokenizerRuleString(tokenizerRulesContent);
            }

            NarrationBox.BattleTokenizer = _battleTokenizer;            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000);

            var startingRoom = new GraphNode<Room>(new Room { RelativeDialoguePath = "Assets.Dialogues.StartingRoom.txt" });
            var centerRoom = new GraphNode<Room>(new Room { RelativeDialoguePath = "Assets.Dialogues.CenterRoom.txt" });
            var leftroom = new GraphNode<Room>(new Room { RelativeDialoguePath = "Assets.Dialogues.LeftRoom.txt" });
            var rightRoom = new GraphNode<Room>(new Room { RelativeDialoguePath = "Assets.Dialogues.RightRoom.txt" });

            NodeCollection<Room> rooms = new NodeCollection<Room>();
            rooms.Add(startingRoom);
            rooms.Add(centerRoom);
            rooms.Add(leftroom);
            rooms.Add(rightRoom);
            _roomGraph = new Graph<Room>(rooms);
            _roomGraph.AddUndirectedEdge(startingRoom, leftroom, 1);
            _roomGraph.AddUndirectedEdge(startingRoom, centerRoom, 1);
            _roomGraph.AddUndirectedEdge(startingRoom, rightRoom, 1);
            CurrentRoom = startingRoom;

            NavigationButtonStack.ItemsSource = AvailableDestinations;
        }

        private string LoadTextFileFromResource(string assemblyRelativePath)
        {
            Assembly assembly = typeof(BattlePage).GetTypeInfo().Assembly;
            string resourceName = $"{assembly.GetName().Name}.{assemblyRelativePath}";
            
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.WriteLine($"WARNING: Unable to laod dialogue asset from assembly at {resourceName}");
                }
                else
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();
                        return result;
                    }
                }
            }

            return null;
        }

        private async void GoUp_Clicked(object sender, EventArgs args)
        {
            await NarrationBox.PageUp();
        }

        private async void GoDown_Clicked(object sender, EventArgs args)
        {
            await NarrationBox.PageDown();
        }

        private async void Next_Clicked(object sender, EventArgs args)
        {
            await NarrationBox.Next();
        }

        private void DestinationButton_Clicked(object sender, EventArgs args)
        {
            Room room = (sender as Button)?.BindingContext as Room;
            if (room != null)
            {
                CurrentRoom = _roomGraph.Nodes.FindByValue(room);
            }
        }

        private void LoadTokenizerRuleString(string rulesString)
        {
            rulesString = rulesString.Replace(@"\\", @"\");
            _battleTokenizer.TokenDefinitions = RulesParser.ParseFromString(rulesString).ToList();            
        }
    }
}
