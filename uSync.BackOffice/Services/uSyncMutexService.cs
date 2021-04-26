﻿namespace uSync.BackOffice.Services
{
    /// <summary>
    ///  handles the mutex or lock for uSync events. 
    /// </summary>
    /// <remarks>
    ///  stops us tripping up over usync firing save events etc while importing
    /// </remarks>
    public class uSyncMutexService
    {
        /// <summary>
        ///  is uSync paused or not ? 
        /// </summary>
        public bool IsPaused { get; private set; }


        /// <summary>
        ///  pause the uSync triggering process
        /// </summary>
        public void Pause() => IsPaused = true;

        /// <summary>
        ///  unpause the uSync triggering process
        /// </summary>
        public void UnPause() => IsPaused = false;

        /// <summary>
        ///  get an import pause object (pauses the import until it is disposed)
        /// </summary>
        /// <remarks>
        ///  you should wrap code that might trigger umbraco events in using(var pause = _mutexService.ImportPause())
        ///  this will ensure that uSync doesn't then pickupt the imports as new things and saves them to disk.
        /// </remarks>
        public uSyncImportPause ImportPause()
            => new uSyncImportPause(this);
    }
}