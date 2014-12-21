using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GMS.Framework.Utility
{
    public delegate void FileUpdate(object state);

    public class FileWatchHelper 
    {       

        /// <summary>
        /// The timer used to compress the notification events.
        /// </summary>
        private Timer m_timer;

        /// <summary>
        /// The default amount of time to wait after receiving notification before reloading the config file.
        /// </summary>
        private const int TimeoutMillis = 500;

        private FileWatchHelper(FileUpdate updateProcess, string filePath, string fileName)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = filePath;
            watcher.Filter = fileName;
            watcher.IncludeSubdirectories = true;

            // Set the notification filters
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // Add event handlers. OnChanged will do for all event handlers that fire a FileSystemEventArgs
            watcher.Changed += new FileSystemEventHandler(ConfigureAndWatchHandler_OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Create the timer that will be used to deliver events. Set as disabled
            m_timer = new Timer(new TimerCallback(updateProcess), filePath, Timeout.Infinite, Timeout.Infinite);

        }

        private FileWatchHelper(FileUpdate updateProcess, string filePath)
            : this(updateProcess, filePath, "" )
        {            
        }



        /// <summary>
        /// Event handler used by <see cref="ConfigureAndWatchHandler"/>.
        /// </summary>
        /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
        /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
        /// <remarks>
        /// <para>
        /// This handler reloads the configuration from the file when the event is fired.
        /// </para>
        /// </remarks>
        private void ConfigureAndWatchHandler_OnChanged(object source, FileSystemEventArgs e)
        {
            m_timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        /// <summary>
        /// Start a watch
        /// </summary>
        /// <param name="updateProcess"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        static public void StartWatching(FileUpdate updateProcess, string filePath)
        {
            //new FileWatchHelper(updateProcess, filePath, fileName);
            new FileWatchHelper(updateProcess, filePath);
        }
    }
}
