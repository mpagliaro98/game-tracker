using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class GameObject : RatedObjectStatusCategorical
    {
        public static int MaxLengthCompletionCriteria => 1000;
        public static int MaxLengthCompletionComment => 1000;
        public static int MaxLengthTimeSpent => 1000;
        public static int MaxLengthGameComment => 5000;

        public new CategoryExtensionGame CategoryExtension { get { return (CategoryExtensionGame)base.CategoryExtension; } }

        [Savable] public string CompletionCriteria { get; set; } = "";
        [Savable] public string CompletionComment { get; set; } = "";
        [Savable] public string TimeSpent { get; set; } = "";
        [Savable] public string GameComment { get; set; } = "";
        [Savable] public virtual DateTime ReleaseDate { get; set; } = DateTime.MinValue;
        [Savable] public virtual DateTime AcquiredOn { get; set; } = DateTime.MinValue;
        [Savable] public virtual DateTime StartedOn { get; set; } = DateTime.MinValue;
        [Savable] public virtual DateTime FinishedOn { get; set; } = DateTime.MinValue;
        [Savable] public virtual bool IsRemaster { get; set; } = false;
        [Savable] public virtual bool UseOriginalGameScore { get; set; } = false;
        [Savable(SaveOnly = true)] public virtual bool IsPartOfCompilation { get { return _compilation.HasValue(); } }
        [Savable(SaveOnly = true)] public virtual bool IsCompilation { get { return false; } }
        [Savable] public virtual bool IsUnfinishable { get; set; } = false;
        [Savable] public virtual bool IsNotOwned { get; set; } = false;
        [Savable(SaveOnly = true)] public virtual bool IsDLC { get { return false; } }

        public bool HasOriginalGame { get { return _originalGame.HasValue(); } }
        public virtual bool IsUsingOriginalGameScore { get { return IsRemaster && HasOriginalGame && UseOriginalGameScore; } }
        public string NameAndPlatform => Name + (PlatformEffective == null ? "" : " (" + (string.IsNullOrWhiteSpace(PlatformEffective.Abbreviation) ? PlatformEffective.Name : PlatformEffective.Abbreviation) + ")");
        public virtual string DisplayName => Name;

        public override double Score
        {
            get
            {
                return CalculateScoreRecursive(new List<UniqueID>());
            }
        }

        public double ScoreMinIfCyclical
        {
            get
            {
                try
                {
                    return Score;
                }
                catch (CyclicalReferenceException)
                {
                    return Settings.MinScore;
                }
            }
        }

        public override double ScoreDisplay
        {
            get
            {
                try
                {
                    return base.ScoreDisplay;
                }
                catch (CyclicalReferenceException ex)
                {
                    Module.Logger.Log(ex.GetType().Name + " in ScoreDisplay: " + ex.Message);
                    return Settings.MinScore;
                }
            }
        }

        public override ScoreRange ScoreRange
        {
            get
            {
                try
                {
                    return base.ScoreRange;
                }
                catch (CyclicalReferenceException)
                {
                    return null;
                }
            }
        }

        [Savable("Platform")] private UniqueID _platform = UniqueID.BlankID();
        public virtual Platform Platform
        {
            get
            {
                if (!_platform.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(Settings), _platform);
            }
            set
            {
                _platform = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("PlatformPlayedOn")] private UniqueID _platformPlayedOn = UniqueID.BlankID();
        public virtual Platform PlatformPlayedOn
        {
            get
            {
                if (!_platformPlayedOn.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(Settings), _platformPlayedOn);
            }
            set
            {
                _platformPlayedOn = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        public Platform PlatformEffective => Platform ?? PlatformPlayedOn;
        public Platform PlatformPlayedOnEffective => PlatformPlayedOn ?? Platform;

        [Savable("OriginalGame")] private UniqueID _originalGame = UniqueID.BlankID();
        public virtual GameObject OriginalGame
        {
            get
            {
                if (!_originalGame.HasValue()) return null;
                return (GameObject)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(Settings), _originalGame);
            }
            set
            {
                _originalGame = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("Compilation")] private UniqueID _compilation = UniqueID.BlankID();
        public virtual GameCompilation Compilation
        {
            get
            {
                if (!_compilation.HasValue()) return null;
                return (GameCompilation)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(Settings), _compilation);
            }
            set
            {
                _compilation = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        public bool IncludeInStats
        {
            get
            {
                // has a status with MarkAsFinished = true
                return ShowScore;
            }
        }

        public bool IsFinishable
        {
            get
            {
                return !IsUnfinishable && (!IsNotOwned || Settings.IncludeUnownedGamesInFinishCount);
            }
        }

        public bool IsFinished
        {
            get
            {
                // has a status with MarkAsFinished = true and is finishable
                return IsFinishable && StatusExtension.Status != null && StatusExtension.Status is StatusGame stat && stat.UseAsFinished;
            }
        }

        protected new SettingsGame Settings => (SettingsGame)base.Settings;
        protected new GameModule Module => (GameModule)base.Module;

        public GameObject(SettingsGame settings, GameModule module) : this(settings, module, new CategoryExtensionGame(module.CategoryExtension, settings)) { }

        protected GameObject(SettingsGame settings, GameModule module, CategoryExtensionGame categoryExtension) : base(settings, module, categoryExtension) { }

        public GameObject(GameObject copyFrom) : this(copyFrom, new CategoryExtensionGame(copyFrom.CategoryExtension)) { }

        protected GameObject(GameObject copyFrom, CategoryExtensionGame categoryExtension) : base(copyFrom, new StatusExtension(copyFrom.StatusExtension), categoryExtension)
        {
            CompletionCriteria = copyFrom.CompletionCriteria;
            CompletionComment = copyFrom.CompletionComment;
            TimeSpent = copyFrom.TimeSpent;
            GameComment = copyFrom.GameComment;
            ReleaseDate = copyFrom.ReleaseDate;
            AcquiredOn = copyFrom.AcquiredOn;
            StartedOn = copyFrom.StartedOn;
            FinishedOn = copyFrom.FinishedOn;
            IsRemaster = copyFrom.IsRemaster;
            UseOriginalGameScore = copyFrom.UseOriginalGameScore;
            IsUnfinishable = copyFrom.IsUnfinishable;
            IsNotOwned = copyFrom.IsNotOwned;
            _platform = UniqueID.Copy(copyFrom._platform);
            _platformPlayedOn = UniqueID.Copy(copyFrom._platformPlayedOn);
            _originalGame = UniqueID.Copy(copyFrom._originalGame);
            _compilation = UniqueID.Copy(copyFrom._compilation);
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (CompletionCriteria.Length > MaxLengthCompletionCriteria)
                throw new ValidationException("Completion criteria cannot be longer than " + MaxLengthCompletionCriteria.ToString() + " characters", CompletionCriteria);
            if (CompletionComment.Length > MaxLengthCompletionComment)
                throw new ValidationException("Completion comment cannot be longer than " + MaxLengthCompletionComment.ToString() + " characters", CompletionComment);
            if (TimeSpent.Length > MaxLengthTimeSpent)
                throw new ValidationException("Time spent cannot be longer than " + MaxLengthTimeSpent.ToString() + " characters", TimeSpent);
            if (GameComment.Length > MaxLengthGameComment)
                throw new ValidationException("Game comment cannot be longer than " + MaxLengthGameComment.ToString() + " characters", GameComment);
            if (OriginalGame != null && OriginalGame.Equals(this))
                throw new ValidationException("Cannot set the Original Game field to this game");
            try
            {
                var temp = Score;
            }
            catch (CyclicalReferenceException e)
            {
                throw new ValidationException("A game referenced by the Original Game references this one, creating a cyclical loop when trying to calculate score", e.Message, e);
            }
        }

        protected virtual void OnModelObjectDeleted(object sender, ModelObjectDeleteArgs args)
        {
            if (args.ObjectType == typeof(GameCompilation))
            {
                if (_compilation.Equals(args.DeletedObject.UniqueID))
                {
                    Compilation = null;
                    SaveWithoutValidation(Module, Settings, args.Connection);
                }
            }
            else if (args.ObjectType == typeof(GameObject))
            {
                if (_originalGame.Equals(args.DeletedObject.UniqueID))
                {
                    OriginalGame = null;
                    SaveWithoutValidation(Module, Settings, args.Connection);
                }
            }
        }

        protected virtual void OnPlatformDeleted(object sender, PlatformDeleteArgs args)
        {
            bool changed = false;
            if (_platform.Equals(args.DeletedObject.UniqueID))
            {
                Platform = null;
                changed = true;
            }
            if (_platformPlayedOn.Equals(args.DeletedObject.UniqueID))
            {
                PlatformPlayedOn = null;
                changed = true;
            }
            if (changed)
                SaveWithoutValidation(Module, Settings, args.Connection);
        }

        protected override void OnSettingsChanged(object sender, SettingsChangeArgs args)
        {
            base.OnSettingsChanged(sender, args);
            Settings settings = (Settings)sender;
            if (settings is SettingsGame settingsGame && settingsGame.TreatAllGamesAsOwned && IsNotOwned)
            {
                IsNotOwned = false;
                SaveWithoutValidation(Module, Settings, args.Connection);
            }
        }

        protected override void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn)
        {
            base.PostSave(module, isNew, conn);
            this.Module.DeleteEmptyCompilations(Settings, conn as ILoadSaveMethodGame);
        }

        protected override void PostDelete(TrackerModule module, ILoadSaveMethod conn)
        {
            base.PostDelete(module, conn);
            this.Module.DeleteEmptyCompilations(Settings, conn as ILoadSaveMethodGame);
        }

        protected override void AddEventHandlers()
        {
            base.AddEventHandlers();
            Module.ModelObjectDeleted += OnModelObjectDeleted;
            Module.PlatformDeleted += OnPlatformDeleted;
        }

        protected override void RemoveEventHandlers()
        {
            base.RemoveEventHandlers();
            Module.ModelObjectDeleted -= OnModelObjectDeleted;
            Module.PlatformDeleted -= OnPlatformDeleted;
        }

        private double CalculateScoreRecursive(IList<UniqueID> path)
        {
            if (path.Contains(UniqueID))
                throw new CyclicalReferenceException("Cyclical reference in OriginalGame field: " + string.Join(" -> ", path));
            if (IsUsingOriginalGameScore)
            {
                path.Add(UniqueID);
                return OriginalGame.CalculateScoreRecursive(path);
            }
            else
                return base.Score;
        }

        public IList<GameDLC> GetDLC()
        {
            return Module.GetModelObjectList(Settings)
                .OfType<GameObject>()
                .Where(o => o.IsDLC && (o as GameDLC).HasBaseGame && (o as GameDLC).BaseGame.Equals(this))
                .Cast<GameDLC>()
                .ToList();
        }

        private IList<Tuple<GameObject, int>> FindGamesWithSimilarName(int maxNumGames)
        {
            if (GetNameForSimilarNameCheck().Length <= 0) return new List<Tuple<GameObject, int>>();
            var similarNames = new List<Tuple<GameObject, int>>();
            var games = Module.GetGamesIncludeInStats(Settings);
            foreach (var game in games)
            {
                if (game.Equals(this)) continue;
                int dist = Util.LevenshteinDistance(GetNameForSimilarNameCheck().ToLower(), game.GetNameForSimilarNameCheck().ToLower());
                int threshold = Convert.ToInt32(Math.Floor(Math.Max(GetNameForSimilarNameCheck().Length, game.GetNameForSimilarNameCheck().Length) * 0.65)); // skip if names are more than 65% different
                if (dist >= threshold) continue;
                similarNames.Add(Tuple.Create(game, dist));
            }
            // improve threshold: if a large enough gap exists between two entries on the list relative to every other gap, use that as the threshold
            // if a large gap doesn't exist, default to something like 65%
            var result = similarNames.OrderBy(t => t.Item2).ToList();
#if DEBUG
            Debug.WriteLine("\nAll games under threshold for " + GetNameForSimilarNameCheck() + " (" + GetNameForSimilarNameCheck().Length.ToString() + ")");
            for (int i = 0; i < maxNumGames; i++)
            {
                if (i >= result.Count) break;
                var t = result[i];
                Debug.WriteLine("\t==> " + t.Item1.GetNameForSimilarNameCheck() + " (" + t.Item1.GetNameForSimilarNameCheck().Length.ToString() + ")" + " - " + t.Item2.ToString() + " (threshold: " + Convert.ToInt32(Math.Floor(Math.Max(GetNameForSimilarNameCheck().Length, t.Item1.GetNameForSimilarNameCheck().Length) * 0.65)).ToString() + ")");
            }
#endif
            return result.Take(maxNumGames).ToList();
        }

        private IList<GameObject> FindGamesWithSimilarNameAndScore(int maxNumGames, double compareScore, RatingCategory category = null)
        {
            var similarNames = FindGamesWithSimilarName(999);
            var matches = similarNames.OrderBy(g => (g.Item2 * 0.3) + Math.Abs(compareScore - (category == null ? g.Item1.ScoreDisplay : g.Item1.CategoryExtension.ScoreOfCategoryDisplay(category)))).Take(maxNumGames).ToList();
#if DEBUG
            Debug.WriteLine("Closest score matches for " + GetNameForSimilarNameCheck() + " (" + compareScore.ToString() + ")");
            for (int i = 0; i < maxNumGames; i++)
            {
                if (i >= matches.Count) break;
                var g = matches[i].Item1;
                Debug.WriteLine("\t==> " + g.GetNameForSimilarNameCheck() + " (" + (category == null ? g.ScoreDisplay : g.CategoryExtension.ScoreOfCategoryDisplay(category)).ToString() + ")" + " - " + Math.Abs(compareScore - (category == null ? g.ScoreDisplay : g.CategoryExtension.ScoreOfCategoryDisplay(category))).ToString() + "+" + (matches[i].Item2 * 0.3).ToString());
            }
            Debug.WriteLine("");
#endif
            return matches.Select(t => t.Item1).ToList();
        }

        private IList<GameObject> FindGamesWithHigherScore(int maxNumGames, double compareScore, RatingCategory category = null, IList<GameObject> exclude = null)
        {
            exclude ??= new List<GameObject>();
            var similarScores = new List<Tuple<GameObject, double>>();
            var games = Module.GetGamesIncludeInStats(Settings);
            foreach (var game in games)
            {
                if (game.Equals(this) || exclude.Contains(game)) continue;
                double score = category == null ? game.ScoreDisplay : game.CategoryExtension.ScoreOfCategoryDisplay(category);
                if (score > compareScore) similarScores.Add(Tuple.Create(game, score));
            }
            return similarScores.OrderBy(t => t.Item2).Select(t => t.Item1).Take(maxNumGames).ToList();
        }

        private IList<GameObject> FindGamesWithLowerScore(int maxNumGames, double compareScore, RatingCategory category = null, IList<GameObject> exclude = null)
        {
            exclude ??= new List<GameObject>();
            var similarScores = new List<Tuple<GameObject, double>>();
            var games = Module.GetGamesIncludeInStats(Settings);
            foreach (var game in games)
            {
                if (game.Equals(this) || exclude.Contains(game)) continue;
                double score = category == null ? game.ScoreDisplay : game.CategoryExtension.ScoreOfCategoryDisplay(category);
                if (score <= compareScore) similarScores.Add(Tuple.Create(game, score));
            }
            return similarScores.OrderByDescending(t => t.Item2).Select(t => t.Item1).Take(maxNumGames).ToList();
        }

        public IList<Tuple<string, GameObject>> SimilarScoreSuggestions(double compareScore, RatingCategory category = null)
        {
            const int maxSimilarName = 2;
            const int maxGames = 4;
            var similarNames = FindGamesWithSimilarNameAndScore(maxSimilarName, compareScore, category);
            int numHigher = Convert.ToInt32(Math.Ceiling((double)(maxGames - similarNames.Count) / 2));
            int numLower = Convert.ToInt32(Math.Floor((double)(maxGames - similarNames.Count) / 2));
            var higher = FindGamesWithHigherScore(numHigher, compareScore, category, similarNames);
            var lower = FindGamesWithLowerScore(numLower + (numHigher - higher.Count), compareScore, category, similarNames);
            var fullList = similarNames.Concat(higher).Concat(lower).ToList();

            var scoreList = fullList.Select(g => Tuple.Create(g, category == null ? g.ScoreDisplay : g.CategoryExtension.ScoreOfCategoryDisplay(category))).ToList();
            scoreList.Add(Tuple.Create(this, compareScore));

            var outputLines = new List<Tuple<string, GameObject>>();
            foreach (var game in scoreList.OrderByDescending(t => t.Item2))
            {
                outputLines.Add(Tuple.Create(game.Item1.GetNameForSimilarNameCheck() + " - " + game.Item2.ToString("0.##"), game.Item1));
            }
            return outputLines;
        }

        protected virtual string GetNameForSimilarNameCheck()
        {
            return Name;
        }
    }
}
