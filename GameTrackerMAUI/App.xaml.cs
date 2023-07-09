using GameTracker;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;

namespace GameTrackerMAUI
{
    public partial class App : Application
    {
        public static Logger Logger { get; private set; }

        public App()
        {
            InitializeComponent();

            IPathController pathController = new PathControllerMobile();
            IFileHandler fileHandlerLogger = new FileHandlerLocalAppData(pathController, LoggerThreaded.LOG_DIRECTORY);
            Logger = new Logger(new LoggerGameTracker(fileHandlerLogger));

            MainPage = new AppShell();
        }
    }
}