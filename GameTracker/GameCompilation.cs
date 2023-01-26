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
                double sumOfWeights = module.CategoryExtension.SumOfCategoryWeights();
                List<double> categoryAverages = new List<double>();
                IList<GameObject> gamesInComp = GamesInCompilation();
                IList<RatingCategory> ratingCategories = module.CategoryExtension.GetRatingCategoryList();
                foreach (RatingCategory category in ratingCategories)
                {
                    categoryAverages.Add(gamesInComp.Select(obj => obj.CategoryExtension.ScoreOfCategory(category)).AverageIfEmpty(settings.MinScore));
                }

                double total = 0;
                for (int i = 0; i < ratingCategories.Count(); i++)
                {
                    total += (ratingCategories[i].Weight / sumOfWeights) * categoryAverages[i];
                }
                return total;
            }
        }

        public new CategoryExtensionGameCompilation CategoryExtension { get { return (CategoryExtensionGameCompilation)base.CategoryExtension; } }

        public override bool IsCompilation { get { return true; } }
        public override GameCompilation Compilation { get { return null; } set { throw new InvalidOperationException("Cannot set the compilation of a compilation"); } }

        public GameCompilation(SettingsGame settings, GameModule module) : base(settings, module, new CategoryExtensionGameCompilation(module.CategoryExtension, settings)) { }

        public IList<GameObject> GamesInCompilation()
        {
            return module.GetModelObjectList().OfType<GameObject>().Where((obj) => obj.Compilation != null && obj.Compilation.Equals(this)).ToList() ?? new List<GameObject>();
        }

        public int NumGamesInCompilation()
        {
            return GamesInCompilation().Count;
        }
    }
}
