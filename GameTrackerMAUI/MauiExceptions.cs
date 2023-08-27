using GameTrackerMAUI.Services;
using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI
{
    public static class MauiExceptions
    {

#if WINDOWS
    private static Exception _lastFirstChanceException;
#endif

        public static void Init(RatableTracker.Interfaces.ILogger logger, ISavedState savedState, IPathController pathController)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Debug.WriteLine("AppDomain.CurrentDomain.UnhandledException");
                LogUnhandledException((Exception)args.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException", logger, savedState, pathController);
            };

            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                Debug.WriteLine("TaskScheduler.UnobservedTaskException");
                LogUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException", logger, savedState, pathController);
                args.SetObserved();
            };

#if IOS || MACCATALYST

            ObjCRuntime.Runtime.MarshalManagedException += (_, args) =>
            {
                Debug.WriteLine("ObjCRuntime.Runtime.MarshalManagedException");
                args.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
            };

#elif ANDROID

            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                Debug.WriteLine("Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser");
                LogUnhandledException(args.Exception, "Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser", logger, savedState, pathController);
            };

#elif WINDOWS

            AppDomain.CurrentDomain.FirstChanceException += (_, args) =>
            {
                _lastFirstChanceException = args.Exception;
            };

            Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, args) =>
            {
                var exception = args.Exception;
                if (exception.StackTrace is null)
                {
                    exception = _lastFirstChanceException;
                }
                Debug.WriteLine("Microsoft.UI.Xaml.Application.Current.UnhandledException");
                LogUnhandledException(exception, "Microsoft.UI.Xaml.Application.Current.UnhandledException", logger, savedState, pathController);
            };
#endif
        }

        private static void LogUnhandledException(Exception exception, string source, RatableTracker.Interfaces.ILogger logger, ISavedState savedState, IPathController pathController)
        {
            try
            {
                string message = RatableTracker.Util.Util.FormatUnhandledExceptionMessage(exception, source);
                logger.Log(message);
                savedState.Delete(pathController);
                logger.Log("Successfully deleted saved state");
            }
            catch (Exception ex)
            {
                logger.Log("EXCEPTION IN EXCEPTION HANDLER: " + ex.Message);
                throw;
            }
            finally
            {
                logger.Dispose();
            }
        }
    }
}
