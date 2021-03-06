using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework.Exceptions;

namespace GameTracker.Model
{
    public abstract class RatingModuleGameTemplate<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        : RatingModuleStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, ISavable, new()
        where TRange : ScoreRange, ISavable, new()
        where TSettings : SettingsScore, ISavable, new()
        where TStatus : CompletionStatus, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        protected IEnumerable<Platform> platforms = new List<Platform>();
        protected IEnumerable<GameCompilation> compilations = new List<GameCompilation>();
        protected LoadSaveEngineGame<TListedObj, TRange, TSettings, TStatus, TRatingCat> loadSaveEngine;

        public IEnumerable<Platform> Platforms => platforms;

        public IEnumerable<GameCompilation> GameCompilations => compilations;

        public virtual int LimitPlatforms => 1000;

        public override void Init()
        {
            LoadPlatforms();
            LoadCompilations();
            base.Init();
        }

        public override async Task InitAsync()
        {
            await LoadPlatformsAsync();
            await LoadCompilationsAsync();
            await base.InitAsync();
        }

        public override void LoadStatuses()
        {
            statuses = loadSaveEngine.LoadISavableList<TStatus>(loadSaveEngine.ID_STATUSES);
        }

        public override async Task LoadStatusesAsync()
        {
            statuses = await loadSaveEngine.LoadISavableListAsync<TStatus>(loadSaveEngine.ID_STATUSES);
        }

        public void LoadPlatforms()
        {
            platforms = loadSaveEngine.LoadISavableList<Platform>(loadSaveEngine.ID_PLATFORMS);
        }

        public async Task LoadPlatformsAsync()
        {
            platforms = await loadSaveEngine.LoadISavableListAsync<Platform>(loadSaveEngine.ID_PLATFORMS);
        }

        public void LoadCompilations()
        {
            compilations = loadSaveEngine.LoadISavableList<GameCompilation>(loadSaveEngine.ID_COMPILATIONS);
        }

        public async Task LoadCompilationsAsync()
        {
            compilations = await loadSaveEngine.LoadISavableListAsync<GameCompilation>(loadSaveEngine.ID_COMPILATIONS);
        }

        public override void LoadListedObjects()
        {
            listedObjs = loadSaveEngine.LoadISavableList<TListedObj>(loadSaveEngine.ID_LISTEDOBJECTS);
        }

        public override async Task LoadListedObjectsAsync()
        {
            listedObjs = await loadSaveEngine.LoadISavableListAsync<TListedObj>(loadSaveEngine.ID_LISTEDOBJECTS);
        }

        public override void LoadRatingCategories()
        {
            ratingCategories = loadSaveEngine.LoadISavableList<TRatingCat>(loadSaveEngine.ID_RATINGCATEGORIES);
        }

        public override async Task LoadRatingCategoriesAsync()
        {
            ratingCategories = await loadSaveEngine.LoadISavableListAsync<TRatingCat>(loadSaveEngine.ID_RATINGCATEGORIES);
        }

        public override void LoadRanges()
        {
            ranges = loadSaveEngine.LoadISavableList<TRange>(loadSaveEngine.ID_RANGES);
        }

        public override async Task LoadRangesAsync()
        {
            ranges = await loadSaveEngine.LoadISavableListAsync<TRange>(loadSaveEngine.ID_RANGES);
        }

        public override void LoadSettings()
        {
            settings = loadSaveEngine.LoadISavable<TSettings>(loadSaveEngine.ID_SETTINGS);
        }

        public override async Task LoadSettingsAsync()
        {
            settings = await loadSaveEngine.LoadISavableAsync<TSettings>(loadSaveEngine.ID_SETTINGS);
        }

        public override void SaveStatuses()
        {
            loadSaveEngine.SaveISavableList(statuses, loadSaveEngine.ID_STATUSES);
        }

        public override async Task SaveStatusesAsync()
        {
            await loadSaveEngine.SaveISavableListAsync(statuses, loadSaveEngine.ID_STATUSES);
        }

        public void SavePlatforms()
        {
            loadSaveEngine.SaveISavableList(platforms, loadSaveEngine.ID_PLATFORMS);
        }

        public async Task SavePlatformsAsync()
        {
            await loadSaveEngine.SaveISavableListAsync(platforms, loadSaveEngine.ID_PLATFORMS);
        }

        public void SaveGameCompilations()
        {
            DeleteEmptyGameCompilations();
            loadSaveEngine.SaveISavableList(compilations, loadSaveEngine.ID_COMPILATIONS);
        }

        public async Task SaveGameCompilationsAsync()
        {
            DeleteEmptyGameCompilations();
            await loadSaveEngine.SaveISavableListAsync(compilations, loadSaveEngine.ID_COMPILATIONS);
        }

        public override void SaveListedObjects()
        {
            loadSaveEngine.SaveISavableList(listedObjs, loadSaveEngine.ID_LISTEDOBJECTS);
        }

        public override async Task SaveListedObjectsAsync()
        {
            await loadSaveEngine.SaveISavableListAsync(listedObjs, loadSaveEngine.ID_LISTEDOBJECTS);
        }

        public override void SaveRatingCategories()
        {
            loadSaveEngine.SaveISavableList(ratingCategories, loadSaveEngine.ID_RATINGCATEGORIES);
        }

        public override async Task SaveRatingCategoriesAsync()
        {
            await loadSaveEngine.SaveISavableListAsync(ratingCategories, loadSaveEngine.ID_RATINGCATEGORIES);
        }

        public override void SaveRanges()
        {
            loadSaveEngine.SaveISavableList(ranges, loadSaveEngine.ID_RANGES);
        }

        public override async Task SaveRangesAsync()
        {
            await loadSaveEngine.SaveISavableListAsync(ranges, loadSaveEngine.ID_RANGES);
        }

        public override void SaveSettings()
        {
            loadSaveEngine.SaveISavable(settings, loadSaveEngine.ID_SETTINGS);
        }

        public override async Task SaveSettingsAsync()
        {
            await loadSaveEngine.SaveISavableAsync(settings, loadSaveEngine.ID_SETTINGS);
        }

        public virtual void TransferSaveFiles(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to)
        {
            loadSaveEngine.TransferSaveFiles(from, to);
        }

        public virtual async Task TransferSaveFilesAsync(IContentLoadSave<string, string> from, IContentLoadSave<string, string> to)
        {
            await loadSaveEngine.TransferSaveFilesAsync(from, to);
        }

        public override double GetScoreOfObject(TListedObj obj)
        {
            if (obj is GameCompilation)
            {
                var games = FindGamesInCompilation(obj as GameCompilation);
                return games.ForEach(rg => GetScoreOfObject(rg)).Average();
            }
            if (obj.IsRemaster && obj.UseOriginalGameScore)
            {
                var originalGame = FindListedObject(obj.RefOriginalGame);
                return GetScoreOfObject(originalGame);
            }
            else
            {
                return base.GetScoreOfObject(obj);
            }
        }

        public override double GetScoreOfCategory(TListedObj obj, TRatingCat cat)
        {
            if (obj is GameCompilation)
            {
                var games = FindGamesInCompilation(obj as GameCompilation);
                return games.ForEach(rg => GetScoreOfCategory(rg, cat)).Average();
            }
            if (obj.IsRemaster && obj.UseOriginalGameScore)
            {
                var originalGame = FindListedObject(obj.RefOriginalGame);
                return GetScoreOfCategory(originalGame, cat);
            }
            else
            {
                return base.GetScoreOfCategory(obj, cat);
            }
        }

        public Platform FindPlatform(ObjectReference objectKey)
        {
            return FindObject(platforms, objectKey);
        }

        public void AddPlatform(Platform obj)
        {
            ValidatePlatform(obj);
            AddToList(ref platforms, SavePlatforms, obj, LimitPlatforms);
        }

        public void UpdatePlatform(Platform obj, Platform orig)
        {
            ValidatePlatform(obj);
            UpdateInList(ref platforms, SavePlatforms, obj, orig);
        }

        public void DeletePlatform(Platform obj)
        {
            DeleteFromList(ref platforms, SavePlatforms, obj);
            listedObjs.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(obj))
                .ForEach(ro => ro.RemovePlatform());
            listedObjs.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatformPlayedOn.IsReferencedObject(obj))
                .ForEach(ro => ro.RemovePlatformPlayedOn());
            if (GlobalSettings.Autosave) SaveListedObjects();
        }

        public IEnumerable<Platform> SortPlatforms<TField>(Func<Platform, TField> keySelector, SortMode mode = SortMode.ASCENDING)
        {
            return SortList(platforms, keySelector, mode);
        }

        public GameCompilation FindGameCompilation(ObjectReference objectKey)
        {
            return FindObject(compilations, objectKey);
        }

        public void AddGameCompilation(GameCompilation obj)
        {
            ValidateGameCompilation(obj);
            AddToList(ref compilations, SaveGameCompilations, obj);
        }

        public void UpdateGameCompilation(GameCompilation obj, GameCompilation orig)
        {
            ValidateGameCompilation(obj);
            UpdateInList(ref compilations, SaveGameCompilations, obj, orig);
        }

        public void DeleteGameCompilation(GameCompilation obj)
        {
            DeleteFromList(ref compilations, SaveGameCompilations, obj);
            listedObjs.Where(ro => ro.RefCompilation.HasReference() && ro.RefCompilation.IsReferencedObject(obj))
                .ForEach(ro => ro.RemoveCompilation());
            if (GlobalSettings.Autosave) SaveListedObjects();
        }

        public override void ValidateListedObject(TListedObj obj)
        {
            base.ValidateListedObject(obj);
            if (obj.CompletionCriteria.Length > RatableGame.MaxLengthCompletionCriteria)
                throw new ValidationException("Completion criteria cannot be longer than " + RatableGame.MaxLengthCompletionCriteria.ToString());
            if (obj.CompletionComment.Length > RatableGame.MaxLengthCompletionComment)
                throw new ValidationException("Completion comment cannot be longer than " + RatableGame.MaxLengthCompletionComment.ToString());
            if (obj.TimeSpent.Length > RatableGame.MaxLengthTimeSpent)
                throw new ValidationException("Time spent cannot be longer than " + RatableGame.MaxLengthTimeSpent.ToString());
            try
            {
                GetScoreOfObject(obj);
            }
            catch (StackOverflowException)
            {
                throw new ValidationException("Cannot set the original game to a game that references this one");
            }
        }

        public virtual void ValidatePlatform(Platform obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > Platform.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + Platform.MaxLengthName.ToString());
            if (obj.Abbreviation.Length > Platform.MaxLengthAbbreviation)
                throw new ValidationException("Abbreviation cannot be longer than " + Platform.MaxLengthAbbreviation.ToString());
        }

        public virtual void ValidateGameCompilation(GameCompilation obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > GameCompilation.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + GameCompilation.MaxLengthName.ToString());
        }

        protected void DeleteEmptyGameCompilations()
        {
            FindEmptyGameCompilations().ForEach(c => DeleteGameCompilation(c));
        }

        protected IEnumerable<GameCompilation> FindEmptyGameCompilations()
        {
            return compilations.Where(c => NumGamesInCompilation(c) == 0);
        }

        public IEnumerable<TListedObj> FindGamesInCompilation(GameCompilation compilation)
        {
            return listedObjs.Where(rg => rg.RefCompilation.HasReference() && rg.RefCompilation.IsReferencedObject(compilation));
        }

        public int NumGamesInCompilation(GameCompilation compilation)
        {
            return FindGamesInCompilation(compilation).Count();
        }

        public IEnumerable<TListedObj> GetGamesOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform));
        }

        public IEnumerable<TListedObj> GetFinishableGamesOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform)
                && ((ro.RefStatus.HasReference() && !FindStatus(ro.RefStatus).ExcludeFromStats) || !ro.RefStatus.HasReference()));
        }

        public IEnumerable<TListedObj> GetFinishedGamesOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform))
                .Where(ro => ro.RefStatus.HasReference() && FindStatus(ro.RefStatus).UseAsFinished);
        }

        public IEnumerable<TListedObj> GetFinishedGamesExcludeStatsOnPlatform(Platform platform)
        {
            return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform))
                .Where(ro => ro.RefStatus.HasReference() && FindStatus(ro.RefStatus).UseAsFinished && !FindStatus(ro.RefStatus).ExcludeFromStats);
        }

        public int GetNumGamesByPlatform(Platform platform)
        {
            return GetGamesOnPlatform(platform).Count();
        }

        public int GetNumGamesFinishableByPlatform(Platform platform)
        {
            return GetFinishableGamesOnPlatform(platform).Count();
        }

        public double GetAverageScoreOfGamesByPlatform(Platform platform)
        {
            var games = GetFinishedGamesOnPlatform(platform);
            return games.Count() <= 0 ? Settings.MinScore : games.Average(ro => GetScoreOfObject(ro));
        }

        public double GetHighestScoreFromGamesByPlatform(Platform platform)
        {
            var games = GetFinishedGamesOnPlatform(platform);
            return games.Count() <= 0 ? Settings.MinScore : games.Max(ro => GetScoreOfObject(ro));
        }

        public double GetLowestScoreFromGamesByPlatform(Platform platform)
        {
            var games = GetFinishedGamesOnPlatform(platform);
            return games.Count() <= 0 ? Settings.MinScore : games.Min(ro => GetScoreOfObject(ro));
        }

        public double GetNumGamesFinishedByPlatform(Platform platform)
        {
            return GetFinishedGamesOnPlatform(platform)
                .Where(game => !FindStatus(game.RefStatus).ExcludeFromStats)
                .Count();
        }

        public double GetPercentageGamesFinishedByPlatform(Platform platform)
        {
            int numFinishable = GetNumGamesFinishableByPlatform(platform);
            if (numFinishable <= 0) numFinishable = 1;
            return GetNumGamesFinishedByPlatform(platform) / numFinishable * 100;
        }

        public IEnumerable<TListedObj> GetTopGamesByPlatform(Platform platform, int numToGet)
        {
            return GetFinishedGamesOnPlatform(platform)
                .OrderByDescending(ro => GetScoreOfObject(ro))
                .Take(numToGet);
        }

        public IEnumerable<TListedObj> GetBottomGamesByPlatform(Platform platform, int numToGet)
        {
            return GetFinishedGamesOnPlatform(platform)
                .OrderBy(ro => GetScoreOfObject(ro))
                .Take(numToGet);
        }

        public int GetRankOfScoreByPlatform(double score, Platform platform)
        {
            return GetRankOfScore(score, ListedObjects, game => game.RefPlatform.IsReferencedObject(platform), null);
        }

        public int GetRankOfScoreByPlatform(double score, Platform platform, TListedObj obj)
        {
            return GetRankOfScore(score, ListedObjects, game => game.RefPlatform.IsReferencedObject(platform), obj);
        }
    }
}
