using RatableTracker.Events;
using RatableTracker.Model;
using RatableTracker.Util;

namespace GameTracker;

public class GameDLC : GameObject
{
    public override bool IsDLC => true;

    // disable all these features of regular games since they don't apply to DLC
    public override bool HasOriginalGame => false;
    public override bool IsRemaster { get => false; set => base.IsRemaster = false; }
    public override bool UseOriginalGameScore { get => false; set => base.UseOriginalGameScore = false; }
    public override GameObject OriginalGame { get => null; set => base.OriginalGame = null; }
    public override bool IsPartOfCompilation => false;
    public override GameCompilation Compilation { get => null; set => base.Compilation = null; }

    public string NameAndBaseGame => (HasBaseGame ? BaseGame.Name + ": " : "") + Name;
    public bool HasBaseGame { get { return _baseGame.HasValue(); } }
    public override string DisplayName => NameAndBaseGame;

    [Savable("BaseGame")] private UniqueID _baseGame = UniqueID.BlankID();
    public virtual GameObject BaseGame
    {
        get
        {
            if (!_baseGame.HasValue()) return null;
            return (GameObject)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(Settings), _baseGame);
        }
        set
        {
            _baseGame = value == null ? UniqueID.BlankID() : value.UniqueID;
        }
    }

    public override Platform Platform { get => HasBaseGame ? BaseGame.Platform : base.Platform; set => base.Platform = value; }
    public override Platform PlatformPlayedOn { get => HasBaseGame ? BaseGame.PlatformPlayedOn : base.PlatformPlayedOn; set => base.PlatformPlayedOn = value; }

    public GameDLC(SettingsGame settings, GameModule module) : base(settings, module, new CategoryExtensionGame(module.CategoryExtension, settings)) { }

    public GameDLC(GameObject copyFrom) : base(copyFrom, new CategoryExtensionGame(copyFrom.CategoryExtension)) { }

    public GameDLC(GameDLC copyFrom) : this(copyFrom as GameObject)
    {
        _baseGame = UniqueID.Copy(copyFrom._baseGame);
    }

    protected override void OnModelObjectDeleted(object sender, ModelObjectDeleteArgs args)
    {
        base.OnModelObjectDeleted(sender, args);
        if (args.ObjectType == typeof(GameObject))
        {
            if (_baseGame.Equals(args.DeletedObject.UniqueID))
            {
                BaseGame = null;
                SaveWithoutValidation(Module, Settings, args.Connection);
            }
        }
    }

    protected override string GetNameForSimilarNameCheck()
    {
        return NameAndBaseGame;
    }
}
