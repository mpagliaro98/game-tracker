﻿using RatableTracker.Exceptions;
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

        public string CompletionCriteria { get; set; } = "";
        public string CompletionComment { get; set; } = "";
        public string TimeSpent { get; set; } = "";
        public DateTime ReleaseDate { get; set; } = DateTime.MinValue;
        public DateTime AcquiredOn { get; set; } = DateTime.MinValue;
        public DateTime StartedOn { get; set; } = DateTime.MinValue;
        public DateTime FinishedOn { get; set; } = DateTime.MinValue;
        public virtual bool IsRemaster { get; set; } = false;
        public virtual bool UseOriginalGameScore { get; set; } = false;
        public virtual bool IsPartOfCompilation { get { return _compilation.HasValue(); } }
        public virtual bool IsCompilation { get { return false; } }
        public bool IsUnfinishable { get; set; } = false;

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
                        module.Logger.Log("GameObject Score " + e.GetType().Name + ": OriginalGame is set to a game that references this one");
                        return settings.MinScore;
                    }
                }
                else
                    return base.Score;
            }
        }

        private UniqueID _platform = UniqueID.BlankID();
        public Platform Platform
        {
            get
            {
                if (!_platform.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(module.GetPlatformList(), _platform);
            }
            set
            {
                _platform = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        private UniqueID _platformPlayedOn = UniqueID.BlankID();
        public Platform PlatformPlayedOn
        {
            get
            {
                if (!_platformPlayedOn.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(module.GetPlatformList(), _platformPlayedOn);
            }
            set
            {
                _platformPlayedOn = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        private UniqueID _originalGame = UniqueID.BlankID();
        public GameObject OriginalGame
        {
            get
            {
                if (!_originalGame.HasValue()) return null;
                return (GameObject)RatableTracker.Util.Util.FindObjectInList(module.GetModelObjectList(), _originalGame);
            }
            set
            {
                _originalGame = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        private UniqueID _compilation = UniqueID.BlankID();
        public virtual GameCompilation Compilation
        {
            get
            {
                if (!_compilation.HasValue()) return null;
                return (GameCompilation)RatableTracker.Util.Util.FindObjectInList(module.GetModelObjectList(), _compilation);
            }
            set
            {
                _compilation = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        protected new SettingsGame settings => (SettingsGame)base.settings;
        protected new GameModule module => (GameModule)base.module;

        public GameObject(SettingsGame settings, GameModule module) : base(settings, module, new CategoryExtensionGame(module.CategoryExtension, settings)) { }

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
            catch (StackOverflowException)
            {
                throw new ValidationException("Cannot set the original game to a game that references this one", _originalGame);
            }
        }

        public override bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            bool changed = base.RemoveReferenceToObject(obj, type);
            if (type == typeof(Platform))
            {
                if (obj.Equals(Platform))
                {
                    Platform = null;
                    changed = true;
                }
                if (obj.Equals(PlatformPlayedOn))
                {
                    PlatformPlayedOn = null;
                    changed = true;
                }
            }
            else if (type == typeof(GameCompilation))
            {
                if (obj.Equals(Compilation))
                {
                    Compilation = null;
                    changed = true;
                }
            }
            else if (type == typeof(GameObject))
            {
                if (obj.Equals(OriginalGame))
                {
                    OriginalGame = null;
                    changed = true;
                }
            }
            return changed;
        }

        protected override void PostSave(TrackerModule module, bool isNew)
        {
            base.PostSave(module, isNew);
            this.module.DeleteEmptyCompilations();
        }

        protected override void PostDelete(TrackerModule module)
        {
            base.PostDelete(module);
            this.module.DeleteEmptyCompilations();
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("CompletionCriteria", new ValueContainer(CompletionCriteria));
            sr.SaveValue("CompletionComment", new ValueContainer(CompletionComment));
            sr.SaveValue("TimeSpent", new ValueContainer(TimeSpent));
            sr.SaveValue("ReleaseDate", new ValueContainer(ReleaseDate));
            sr.SaveValue("AcquiredOn", new ValueContainer(AcquiredOn));
            sr.SaveValue("StartedOn", new ValueContainer(StartedOn));
            sr.SaveValue("FinishedOn", new ValueContainer(FinishedOn));
            sr.SaveValue("IsRemaster", new ValueContainer(IsRemaster));
            sr.SaveValue("UseOriginalGameScore", new ValueContainer(UseOriginalGameScore));
            sr.SaveValue("IsPartOfCompilation", new ValueContainer(IsPartOfCompilation));
            sr.SaveValue("IsCompilation", new ValueContainer(IsCompilation));
            sr.SaveValue("IsUnfinishable", new ValueContainer(IsUnfinishable));
            sr.SaveValue("Platform", new ValueContainer(_platform));
            sr.SaveValue("PlatformPlayedOn", new ValueContainer(_platformPlayedOn));
            sr.SaveValue("OriginalGame", new ValueContainer(_originalGame));
            sr.SaveValue("Compilation", new ValueContainer(_compilation));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "CompletionCriteria":
                        CompletionCriteria = sr.GetValue(key).GetString();
                        break;
                    case "CompletionComment":
                        CompletionComment = sr.GetValue(key).GetString();
                        break;
                    case "TimeSpent":
                        TimeSpent = sr.GetValue(key).GetString();
                        break;
                    case "ReleaseDate":
                        ReleaseDate = sr.GetValue(key).GetDateTime();
                        break;
                    case "AcquiredOn":
                        AcquiredOn = sr.GetValue(key).GetDateTime();
                        break;
                    case "StartedOn":
                        StartedOn = sr.GetValue(key).GetDateTime();
                        break;
                    case "FinishedOn":
                        FinishedOn = sr.GetValue(key).GetDateTime();
                        break;
                    case "IsRemaster":
                        IsRemaster = sr.GetValue(key).GetBool();
                        break;
                    case "UseOriginalGameScore":
                        UseOriginalGameScore = sr.GetValue(key).GetBool();
                        break;
                    case "IsUnfinishable":
                        IsUnfinishable = sr.GetValue(key).GetBool();
                        break;
                    case "Platform":
                        _platform = sr.GetValue(key).GetUniqueID();
                        break;
                    case "PlatformPlayedOn":
                        _platformPlayedOn = sr.GetValue(key).GetUniqueID();
                        break;
                    case "OriginalGame":
                        _originalGame = sr.GetValue(key).GetUniqueID();
                        break;
                    case "Compilation":
                        _compilation = sr.GetValue(key).GetUniqueID();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
