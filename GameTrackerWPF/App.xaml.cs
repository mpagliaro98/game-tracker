using GameTracker;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Logger Logger { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IPathController pathController = new PathControllerWindows();
            IFileHandler fileHandlerLogger = new FileHandlerLocalAppData(pathController, LoggerThreaded.LOG_DIRECTORY);
            Logger = new Logger(new LoggerGameTracker(fileHandlerLogger));

            AppDomain.CurrentDomain.UnhandledException += (s, args) => LogUnhandledException((Exception)args.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            DispatcherUnhandledException += (s, args) =>
            {
                LogUnhandledException(args.Exception, "DispatcherUnhandledException");
                args.Handled = true;
            };
            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                LogUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");
                args.SetObserved();
            };
        }

        private static void LogUnhandledException(Exception exception, string source)
        {
            try
            {
                string message = RatableTracker.Util.Util.FormatUnhandledExceptionMessage(exception, source);
                Logger.Log(message);
                SavedState.DeleteSavedState(new PathControllerWindows());
                Logger.Log("Successfully deleted saved state");
            }
            catch (Exception ex)
            {
                Logger.Log("EXCEPTION IN EXCEPTION HANDLER: " + ex.Message);
                throw;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.Log("Application exiting cleanly");
            Logger.Dispose();
        }
    }
}
