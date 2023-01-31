﻿using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class GameObject : RatedObjectStatusCategorical
    {
        public static int MaxLengthCompletionCriteria => 1000;
        public static int MaxLengthCompletionComment => 1000;
        public static int MaxLengthTimeSpent => 1000;

        public new CategoryExtensionGame CategoryExtension { get { return (CategoryExtensionGame)base.CategoryExtension; } }

        [Savable("CompletionCriteria")] public string CompletionCriteria { get; set; } = "";
        [Savable("CompletionComment")] public string CompletionComment { get; set; } = "";
        [Savable("TimeSpent")] public string TimeSpent { get; set; } = "";
        [Savable("ReleaseDate")] public DateTime ReleaseDate { get; set; } = DateTime.MinValue;
        [Savable("AcquiredOn")] public DateTime AcquiredOn { get; set; } = DateTime.MinValue;
        [Savable("StartedOn")] public DateTime StartedOn { get; set; } = DateTime.MinValue;
        [Savable("FinishedOn")] public DateTime FinishedOn { get; set; } = DateTime.MinValue;
        [Savable("IsRemaster")] public virtual bool IsRemaster { get; set; } = false;
        [Savable("UseOriginalGameScore")] public virtual bool UseOriginalGameScore { get; set; } = false;
        [Savable("IsPartOfCompilation", SaveOnly = true)] public virtual bool IsPartOfCompilation { get { return _compilation.HasValue(); } }
        [Savable("IsCompilation", SaveOnly = true)] public virtual bool IsCompilation { get { return false; } }
        [Savable("IsUnfinishable")] public bool IsUnfinishable { get; set; } = false;

        public bool HasOriginalGame { get { return _originalGame.HasValue(); } }
        public virtual bool IsUsingOriginalGameScore { get { return IsRemaster && HasOriginalGame && UseOriginalGameScore; } }

        public override double Score
        {
            get
            {
                if (IsUsingOriginalGameScore)
                {
                    try
                    {
                        return OriginalGame.Score;
                    }
                    catch (StackOverflowException e)
                    {
                        Module.Logger.Log("GameObject Score " + e.GetType().Name + ": OriginalGame is set to a game that references this one");
                        return Settings.MinScore;
                    }
                }
                else
                    return base.Score;
            }
        }

        [Savable("Platform")] private UniqueID _platform = UniqueID.BlankID();
        public Platform Platform
        {
            get
            {
                if (!_platform.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(), _platform);
            }
            set
            {
                _platform = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("PlatformPlayedOn")] private UniqueID _platformPlayedOn = UniqueID.BlankID();
        public Platform PlatformPlayedOn
        {
            get
            {
                if (!_platformPlayedOn.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(), _platformPlayedOn);
            }
            set
            {
                _platformPlayedOn = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("OriginalGame")] private UniqueID _originalGame = UniqueID.BlankID();
        public GameObject OriginalGame
        {
            get
            {
                if (!_originalGame.HasValue()) return null;
                return (GameObject)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(), _originalGame);
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
                return (GameCompilation)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(), _compilation);
            }
            set
            {
                _compilation = value == null ? UniqueID.BlankID() : value.UniqueID;
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
            ReleaseDate = copyFrom.ReleaseDate;
            AcquiredOn = copyFrom.AcquiredOn;
            StartedOn = copyFrom.StartedOn;
            FinishedOn = copyFrom.FinishedOn;
            IsRemaster = copyFrom.IsRemaster;
            UseOriginalGameScore = copyFrom.UseOriginalGameScore;
            IsUnfinishable = copyFrom.IsUnfinishable;
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
            try
            {
                var temp = Score;
            }
            catch (StackOverflowException e)
            {
                throw new ValidationException("A game referenced by the Original Game references this one", e);
            }
        }

        private void OnModelObjectDeleted(object sender, ModelObjectDeleteArgs args)
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

        private void OnPlatformDeleted(object sender, PlatformDeleteArgs args)
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
    }
}
