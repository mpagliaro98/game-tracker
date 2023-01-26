﻿using RatableTracker.ListManipulation;
using RatableTracker.Model;
using RatableTracker.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class FilterGames : FilterRatedObjectStatusCategorical
    {
        public bool ShowCompilations { get; set; } = false;
        public Platform Platform { get; set; } = null;

        public new GameModule Module { get { return (GameModule)base.Module; } set { base.Module = value; } }
        public new SettingsGame Settings { get { return (SettingsGame)base.Settings; } set { base.Settings = value; } }

        public FilterGames() : base() { }

        public FilterGames(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override IList<RankedObject> ApplyFiltering(IList<RankedObject> list)
        {
            IList<RankedObject> newList = base.ApplyFiltering(list);
            if (ShowCompilations)
                newList = newList.OfType<GameObject>().Where(obj => !obj.IsPartOfCompilation || obj.IsCompilation).Cast<RankedObject>().ToList();
            else
                newList = newList.OfType<GameObject>().Where(obj => !obj.IsCompilation).Cast<RankedObject>().ToList();
            if (Platform != null)
                newList = newList.OfType<GameObject>().Where(obj => obj.Platform != null && obj.Platform.Equals(Platform)).Cast<RankedObject>().ToList();
            return newList;
        }
    }
}
