using BattleTextTokenizer;
using BattleTextTokenizer.Models;
using Optional;
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
using TextGameExperiment.Core.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextGameExperiment.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BattlePage : ContentPage
    {
        private const string DefaultRulesPath = "Assets.BattleTextRules.txt";

        //private readonly IFileService _fileService;

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
                    UpdateRoom(_currentRoom.Value.RelativeDialoguePath);
                }
            }
        }

        public ObservableCollection<Room> AvailableDestinations { get; set; } = new ObservableCollection<Room>();


        public BattlePage()
        {           
            InitializeComponent();               
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            // Load default tokenizer rules.         
            Option<string> tokenizerRulesContent = await FileService.ReadFileFromResourcesAsync(DefaultRulesPath);
            if (tokenizerRulesContent != null)
            {
                LoadTokenizerRuleString(tokenizerRulesContent.ValueOr(""));
            }

            NarrationBox.BattleTokenizer = _battleTokenizer;

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

            await startingRoom.Value.LoadAsync();
            CurrentRoom = startingRoom;

            NavigationButtonStack.ItemsSource = AvailableDestinations;
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

        private async void UpdateRoom(string relativeDialoguePath)
        {
            AvailableDestinations.Clear();
            foreach (GraphNode<Room> roomNode in _currentRoom.Neighbors)
            {
                await roomNode.Value.LoadAsync();
                AvailableDestinations.Add(roomNode.Value);
            }

            TitleLabel.Text = CurrentRoom.Value.Title;

            NarrationBox.ResetBattleText();
            NarrationBox.QueueBattleText(CurrentRoom.Value.Body);
        }

        private void LoadTokenizerRuleString(string rulesString)
        {
            rulesString = rulesString.Replace(@"\\", @"\");
            _battleTokenizer.TokenDefinitions = RulesParser.ParseFromString(rulesString).ToList();            
        }
    }
}
