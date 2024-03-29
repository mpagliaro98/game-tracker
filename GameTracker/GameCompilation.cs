﻿using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class GameCompilation : GameObject
    {
        public override double Score
        {
            get
            {
                double sumOfWeights = Module.CategoryExtension.SumOfCategoryWeights();
                List<double> categoryAverages = new List<double>();
                IList<GameObject> gamesInComp = GamesInCompilation();
                IList<RatingCategory> ratingCategories = Module.CategoryExtension.GetRatingCategoryList();
                foreach (RatingCategory category in ratingCategories)
                {
                    categoryAverages.Add(gamesInComp.Select(obj => obj.CategoryExtension.ScoreOfCategory(category)).AverageIfEmpty(Settings.MinScore));
                }

                double total = 0;
                for (int i = 0; i < ratingCategories.Count(); i++)
                {
                    total += ratingCategories[i].Weight * categoryAverages[i] / sumOfWeights;
                }
                return total;
            }
        }

        public new CategoryExtensionGameCompilation CategoryExtension { get { return (CategoryExtensionGameCompilation)base.CategoryExtension; } }

        public override bool IsCompilation { get { return true; } }
        public override GameCompilation Compilation { get { return null; } set { throw new InvalidOperationException("Cannot set the compilation of a compilation"); } }

        public override DateTime ReleaseDate { get => GamesInCompilation().Select(g => g.ReleaseDate).Where(d => d > DateTime.MinValue).DefaultIfEmpty().Min(); set { } }
        public override DateTime AcquiredOn { get => GamesInCompilation().Select(g => g.AcquiredOn).Where(d => d > DateTime.MinValue).DefaultIfEmpty().Min(); set { } }
        public override DateTime StartedOn { get => GamesInCompilation().Select(g => g.StartedOn).Where(d => d > DateTime.MinValue).DefaultIfEmpty().Min(); set { } }
        public override DateTime FinishedOn { get => IsUnfinishable || !IsFinished ? DateTime.MinValue : GamesInCompilation().Select(g => g.FinishedOn).DefaultIfEmpty().Max(); set { } }
        public override bool IsUnfinishable { get => GamesInCompilation().Select(g => g.IsUnfinishable).DefaultIfEmpty().All(b => b); set { } }
        public override bool IsNotOwned { get => GamesInCompilation().Select(g => g.IsNotOwned).DefaultIfEmpty().All(b => b); set { } }

        public GameCompilation(SettingsGame settings, GameModule module) : base(settings, module, new CategoryExtensionGameCompilation(module.CategoryExtension, settings)) { }

        public GameCompilation(GameCompilation copyFrom) : base(copyFrom, new CategoryExtensionGameCompilation(copyFrom.CategoryExtension)) { }

        public IList<GameObject> GamesInCompilation()
        {
            return Module.GetModelObjectList(Settings).OfType<GameObject>().Where((obj) => obj.Compilation != null && obj.Compilation.Equals(this)).ToList() ?? new List<GameObject>();
        }

        public int NumGamesInCompilation()
        {
            return GamesInCompilation().Count;
        }
    }
}
