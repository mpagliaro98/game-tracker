using GameTracker;
using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI
{
    public class CategoryValueContainer : INotifyPropertyChanged
    {
        private readonly GameObject game;
        private readonly RatingCategory category;
        private double categoryValue = 0;
        private string _similarScoreBefore = "";
        private string _similarScoreGame = "";
        private string _similarScoreAfter = "";
        private bool calculateSimilar = false;
        private bool refreshing = false;
        private Mutex refreshMutex = new(false);

        public GameObject Game
        {
            get => game;
        }

        public RatingCategory Category
        {
            get => category;
        }

        public double CategoryValue
        {
            get => categoryValue;
            set
            {
                SetProperty(ref categoryValue, value);
                if (!calculateSimilar) return;
                refreshMutex.WaitOne();
                if (!refreshing)
                {
                    refreshMutex.ReleaseMutex();
                    var _ = RefreshSimilar();
                }
            }
        }

        public string SimilarScoreBefore => _similarScoreBefore;
        public string SimilarScoreGame => _similarScoreGame;
        public string SimilarScoreAfter => _similarScoreAfter;

        public CategoryValueContainer(GameObject game, RatingCategory category, bool calculateSimilar = false)
        {
            this.game = game;
            this.category = category;
            this.calculateSimilar = calculateSimilar;
        }

        public async Task RefreshSimilar()
        {
            refreshMutex.WaitOne();
            refreshing = true;
            refreshMutex.ReleaseMutex();
            var similar = await Task.Run(() => Game.SimilarScoreSuggestions(CategoryValue, Category));
            _similarScoreBefore = "";
            _similarScoreGame = "";
            _similarScoreAfter = "";
            bool afterGame = false;
            foreach (var t in similar)
            {
                if (t.Item2.Equals(Game))
                {
                    if (_similarScoreBefore.Length > 0) _similarScoreBefore += "\n";
                    _similarScoreGame = t.Item1 + "\n";
                    afterGame = true;
                }
                else if (afterGame)
                    _similarScoreAfter += (_similarScoreAfter.Length > 0 ? "\n" : "") + t.Item1;
                else
                    _similarScoreBefore += (_similarScoreBefore.Length > 0 ? "\n" : "") + t.Item1;
            }
            OnPropertyChanged(nameof(SimilarScoreBefore));
            OnPropertyChanged(nameof(SimilarScoreGame));
            OnPropertyChanged(nameof(SimilarScoreAfter));
            refreshMutex.WaitOne();
            refreshing = false;
            refreshMutex.ReleaseMutex();
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
